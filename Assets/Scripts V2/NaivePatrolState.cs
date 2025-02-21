using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaivePatrolState : NaiveFSMState
{
    private PatrolAgentFSM PatrolFSMRef = null;

    // Variables No-exclusivas del estado
    // Cono de visi�n
    // Las dej� comentadas como ejemplo de que son las no-exclusivas del estado, y que me las llev� a la m�quina de estados.
    //public float VisionAngle;
    //public float VisionDistance;
    //public bool DetectedPlayer;

    public float VisionAngle;
    public float VisionDistance;

    //Declaramos un objeto para poder controlar la rotacion del patrullero
    public GameObject agentTransform;

    // Variables Exclusivas de este estado.
    // 
    private float RotationAngle;
    private float TimeBeforeRotating = 2f;
    private float AccumulatedTimeBeforeRotating;
    private float TimeDetectingPlayerBeforeEnteringAlert;
    private float AccumulatedTimeDetectingPlayerBeforeEnteringAlert;

    public void Init(float in_VisionDistance, float in_VisionAngle, float in_RotationAngle,
        float in_TimeBeforeRotating, float in_TimeDetectingPlayerBeforeEnteringAlert)
    {
        RotationAngle = in_RotationAngle;
        TimeBeforeRotating = in_TimeBeforeRotating;
        TimeDetectingPlayerBeforeEnteringAlert = in_TimeDetectingPlayerBeforeEnteringAlert;
        // Estas dos de aqu� las sacamos de la FSM
        VisionAngle = in_VisionAngle;
        VisionDistance = in_VisionDistance;
    }

    // Ojo: no hagan esto, porque conlleva a situaciones molestas y propensas a errores humanos.
    // Referencia al estado de alerta (OJO, ESTO CAUSAR� ALTA DEPENDENCIA ENTRE LAS CLASES)
    // NaiveAlertState _AlertState;

    public NaivePatrolState(NaiveFSM FSM)
    {
        Name = "Guard";
        _FSM = FSM;
        PatrolFSMRef = ((PatrolAgentFSM)_FSM);
    }


    public override void Enter()
    {
        PatrolFSMRef.PlayNotDetectedMusic();
        // 
        base.Enter();
        //Establecemos la animacion a usar
        PatrolFSMRef._Animator.SetBool("Patrullando", true);
         
        
        PatrolFSMRef._Animator.SetBool("Alerta", false);
        agentTransform = GameObject.Find("Guard");
        Debug.Log("Entr� al estado de Patrullaje.");
        // Ac� ya puedo hacer lo que esta clase hija espec�ficamente tiene que hacer
        AccumulatedTimeBeforeRotating = 0.0f;
        AccumulatedTimeDetectingPlayerBeforeEnteringAlert = 0.0f;
    }

    // Update is called once per frame
    public override void Update()
    {
        // Debug.Log("Update del estado patrullaje.");

        // Detectar al infiltrador
        // Lo detectamos?
        if (CheckFOV())
        {
            // Si la funci�n del cono de visi�n nos regresa verdadero
            // Acumulamos tiempo
            // Lo acumulamos en la variable que declaramos para llevar el tiempo acumulado "AccumulatedTimeDetectingPlayerBeforeEnteringAlert"
            AccumulatedTimeDetectingPlayerBeforeEnteringAlert += Time.deltaTime;
            // si el tiempo acumulado es mayor a la cantidad de tiempo que establecimos, cambiamos al estado de alerta
            if (TimeDetectingPlayerBeforeEnteringAlert <= AccumulatedTimeDetectingPlayerBeforeEnteringAlert)
            {
                // Si s�, cambiamos al estado de alerta.
                // (PatrolAgentFSM)_FSM -> casteamos la m�quina de estados base, al tipo espec�fico de la FSM que es nuestra due�a.
                // esto nos permite acceder a las variables que tiene esa clase espec�fica, en este caso, al estado de Alerta al que
                // queremos pasar, que lo obtenemos a trav�s de "AlertStateRef".
                NaiveAlertState AlertStateInstance = PatrolFSMRef.AlertStateRef;
                //Desactivamos la animacion al pasar al estado de alerta
                PatrolFSMRef._Animator.SetBool("Patrullando", false);
                _FSM.ChangeState(AlertStateInstance);
                return; // Damos return siempre despu�s del change state, para evitar que cualquier otra cosa 
                // del update se fuera a ejecutar.
            }
        }

        // D�nde pondr�amos la parte de rotar al patrullero cada cierto tiempo?
        //Le a�adimos tiempo a la variable de el tiempo acumulado antes de rotar
        AccumulatedTimeBeforeRotating += Time.deltaTime;
        //una vez que el tiempo sea mayor o igual a la cantidad deseada rotaremos el agente
        if (AccumulatedTimeBeforeRotating >= 4f)
        {
            RotateAgent();
            // Reiniciamos el contador de tiempo
            AccumulatedTimeBeforeRotating = 0.0f; 
            Debug.Log("Rotando");
        }


    }

    private void RotateAgent()
    {
        // Rotar el agente seg�n el �ngulo de rotaci�n definido
        agentTransform.transform.Rotate(Vector3.up, RotationAngle);
        //PatrolFSMRef._Animator.SetBool("Patrullando", true);
        
    }

    // Aqu� estamos omitiendo a prop�sito la funci�n Exit()
    // solo para demostrar que podemos hacerlo.

    private bool CheckFOV()
    {
        // �nicamente mandamos a llamar la funci�n de la FSM pero con nuestros par�metros espec�ficos de este estado.
        return PatrolFSMRef.CheckFieldOfVision(VisionDistance, VisionAngle);
    }
}


