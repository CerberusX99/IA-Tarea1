
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public float followSpeed = 15f;
    public float slowdownDistance = 1f;
    public Transform targetObject; // Referencia del GameObject
    Vector3 velocity = Vector3.zero;

    public NavMeshAgent agent3;
    public Transform agent; // El agente que tiene esta visi�n de cono
    public float visionAngle = 45f; // �ngulo de visi�n del cono
    public float visionDistance = 10f; // Distancia m�xima de visi�n

    public bool isDetected = false; // Indica si el agente ha detectado algo
     public float rotationSpeed = 50f; // speed of rotation
    // Start is called before the first frame update
    public GameObject agent2;
    public GameObject guard;
    public bool isAttacking = false;   
    private bool isMovingToSavedPosition = false;

    // Movement speed when moving to saved position
    public float movementSpeed = 0.1f;
    // Time the guard has been in detected state
    private float detectedTime = 0f;
    // Maximum time the guard can be in detected state before transitioning to another state
    public float maxDetectedTime = 1f; 
    public float rotationTime = 0f;
    public Vector3 startPosition;
    public float attackTime = 0f;
    public AudioClip deathSound;
    public AudioSource audioSource;

    private void Awake()
    {
        agent= agent2.transform;
    }
    void Start()
    {
        //startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
         DetectObjects(); // Detectamos objetos en el cono de visi�n
         StateC();
    }

    //Esta funcion determina si el guardia ha detectado algun objeto en su cono de
    //vision, tambien calcula si el angulo entre la direccion hacia adelante del guardia y
    //la direccion al objeto que esta dentro del rango y si la distancia al objeto está
    // dentro de su rango de visión
     public void DetectObjects()
    {
        isDetected = false; // Reiniciamos la detecci�n

        // Obtenemos el vector que apunta desde el agente hacia adelante
        Vector3 directionToAgent = agent.position - transform.position;

        // Calculamos el �ngulo entre la direcci�n al agente y la direcci�n hacia adelante del cono
        float angleToAgent = Vector3.Angle(transform.forward, directionToAgent);

        // Si el �ngulo est� dentro del rango del cono y el agente est� dentro de la distancia de visi�n
        if (angleToAgent < visionAngle / 2 && directionToAgent.magnitude < visionDistance)
        {
            // El agente ha sido detectado
            isDetected = true;
           // Debug.Log("Detectado");
        }
    }

    // Gizmos se utiliza para dibujar el campo de visión del guardia.
    // Un cono amarilla que representa el campo de vision del guardia
    // Rayos que representan los limites del campo de visión
    // Un cono rojo si el guardia ha detectado algo, o verde si no ha detectado nada.

    public void OnDrawGizmos()
    {
        // Dibujamos el cono de visi�n
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        //Gizmos.DrawRay(transform.position, Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * visionDistance);
        //Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * visionDistance);
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward * visionDistance);

        // Si el agente est� detectando algo, cambiamos el color del cono a rojo
        if (isDetected)
        {
            Gizmos.color = Color.red;
        }
        else // Si no est� detectando nada, el color del cono es verde
        {
            Gizmos.color = Color.green;
        }

        // Dibujamos el cono de visi�n
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * visionDistance);
    }
    //La funcion maneja los diferentes estados del guardia;
    // Si no ah detectado ningun objeto y no se está moviendo a la posición guardada o atacando, 
    // el guardia se encuentra en su estado normal. Si detecta un objeto, entra un estado de alerta 
    // y comienza a seguirlo.
    public void StateC()
    {
        if(isDetected == false && isMovingToSavedPosition == false && isAttacking == false)
        {
            
            NormalS();
            detectedTime = 0f;
        }
        else if(isDetected == true)
        {
            detectedTime += Time.deltaTime;
            if (detectedTime >= maxDetectedTime && isAttacking == false)
            {
                
                AttackS();
                detectedTime = 0f;
            }
            else if (isAttacking == false)
            {
                AlertS();
            }
        }
    
    }


    //La función estado normal, el guardia rota sobre su eje vertical para buscar
    //sus objetivos. La rotación se realiza cada cierto tiempo y se controla mediante
    //la variable rotationTime. La amplitud de la rotación y la velocidad se pueden
    // ajustar mediante las variables rotationSpeed y visionAgle.
    public void NormalS()
{
    
    Debug.Log("Normal");
    visionAngle = 45f;
    rotationTime += Time.deltaTime;
    
    // Rotate the guard every 5 seconds
    if (rotationTime >= 2f)
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Reset the rotation time after the rotation
        if (rotationTime >= 5f) // Rotate for 5 seconds, then wait for 5 seconds
        {
            rotationTime = 0f;
        }
    }
}

    // En la función el guardia esta en el estado de alerta donde aumenta su ángulo de
    //visión para estar más alerta.

    public void AlertS()
    {
        if(isAttacking == false){
            Debug.Log("Alert");
        if(isDetected == true)
        {
            SavePosition();
            visionAngle = 90f;
            LoadPosition();
        }
        }
    }

    //En esta función el guardia persigue al agente y lo ataca al ser detectado. Se mueve hacia
    //la posición del agente a una posición determinada y ya alacanzado el objetivo ataca
    //durante un tiempo determinado.

    public void AttackS()
{
    if(attackTime<=5f){
      
            Debug.Log("Attack");
            attackTime ++;
            isAttacking = true;
            Vector3 targetPosition = targetObject.position; // Use the position of the target object
            Vector3 playerDistance = targetPosition - transform.position;
            Vector3 desiredVelocity = playerDistance.normalized * followSpeed;
            Vector3 steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;
            float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowdownDistance);
            velocity *= slowDownFactor;

            transform.position += (Vector3)velocity * Time.deltaTime;
          
    }
    else {
        attackTime = 0f;
        Debug.Log("Bai");
        StartCoroutine(MoveToStartPosition(startPosition));
        isAttacking = false;
        
    }

}

    //Las funciones SavePosition y Load Position se encargan de guardar y cargar la posición actual
    //del guardia en el juego.

    public void SavePosition()
    {
        float x = agent2.transform.position.x;
        float y = agent2.transform.position.y; 
        float z = agent2.transform.position.z;
        PlayerPrefs.SetFloat("x", x);
        PlayerPrefs.SetFloat("y", y);
        PlayerPrefs.SetFloat("z", z);
        PlayerPrefs.SetInt("Saved", 1);
    }

    public void LoadPosition()
    {
        if (PlayerPrefs.GetInt("Saved") == 1)
        {
            float x = PlayerPrefs.GetFloat("x");
            float y = PlayerPrefs.GetFloat("y");
            float z = PlayerPrefs.GetFloat("z");
            Vector3 savedPosition = new Vector3(x, y, z);
            StartCoroutine(MoveToSavedPosition(savedPosition));
        }
    }

    //MoveToSavedPosition y MoveToStartPosition las funciones se utilizan para mover
    // al guardia a una posición específica en el juego 
    private IEnumerator MoveToSavedPosition(Vector3 targetPosition)
{
    isMovingToSavedPosition = true;

    int maxIterations = 1000; // Maximum number of iterations to prevent infinite loop
    int iterations = 0; // Counter for iterations

    while (Vector3.Distance(transform.position, targetPosition) > 0.01f && iterations < maxIterations)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        yield return null;
        iterations++;
    }
    
    isMovingToSavedPosition = false;
    if (isDetected==false)
    {
        StartCoroutine(MoveToStartPosition(startPosition));
        isAttacking = false;
    }
}

private IEnumerator MoveToStartPosition(Vector3 targetPosition)
{
    isMovingToSavedPosition = true;

    int maxIterations = 1000; // Maximum number of iterations to prevent infinite loop
    int iterations = 0; // Counter for iterations

    while (Vector3.Distance(transform.position, targetPosition) > 0.01f && iterations < maxIterations)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        yield return null;
        iterations++;
    }

    isMovingToSavedPosition = false;
}


    //la función se activa cuando el guardia entra en contacto con otro objeto.
    //En este caso, si el objeto es el jugador, reproduce un sonido de muerte.)
    public void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.tag == "Player")
    {
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
    }
   
}
}