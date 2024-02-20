using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
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
    public void OnDrawGizmos()
    {
        // Dibujamos el cono de visi�n
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * visionDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * visionDistance);

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
            if (detectedTime >= maxDetectedTime)
            {
                Debug.Log("Attack");
                AttackS();
                //detectedTime = 0f;
            }
            else
            {
                AlertS();
            }
        }
    
    }

    
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

public void AttackS()
{
    if (attackTime <= 5f)
    {
        // Move towards the target position during the attack time
        agent3.SetDestination(agent2.transform.position);
        attackTime += Time.deltaTime;
        isAttacking = true; 
    }
    else
    {
        // If the attack time exceeds 5 seconds, return to the original position
        
        //attackTime = 0f;
        isAttacking = false;
        StartCoroutine(MoveToStartPosition(startPosition));
    }
}


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

   
}