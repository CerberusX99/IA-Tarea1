using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;



public class SB_Seek : MonoBehaviour
{
    public enum SteeringBehavior
    {
        None,  // 0
        Seek,   // 1
        Flee,  // 2
        Pursuit, // 3
        Evade,  // 4
        MAX     // 5
    };

    public SteeringBehavior currentBehavior = SteeringBehavior.Seek;

    // Vector tridimensional para la posici�n del mouse en el mundo
    Vector3 mouseWorldPos = Vector3.zero;

    public float maxSpeed = 1.0f;

    // Qu� tanto tiempo queremos que pase antes de aplicar toda la steering force.
    public float maxSteeringForce = 1.0f;

    // Rigidbody ya trae dentro:
    // Vector tridimensional para la aceleraci�n.
    // Vector tridimensional para representar esa velocidad
    private Rigidbody rb;

    // Variable donde guardamos la referencia al GameObject que es nuestro objetivo.
    private GameObject TargetGameObject;
    // Referencia al Rigidbody del TargetGameObject.
    private Rigidbody rbTargetGameObject;

    // Si nuestro objetivo est� a X de distancia,
    // y nuestro agente se mueve a Y de distancia por segundo,
    // cu�ntos segundos nos toma llegar hasta X?
    // 0 + Y/s = X
    // s = X/Y
    // 5/s = 20
    // s = 20/5 = 4

    // Start is called before the first frame update
    void Start()
    {
        print("Funcion Start");
        rb = GetComponent<Rigidbody>();

        TargetGameObject = FindAnyObjectByType<SimpleAgent>().gameObject;
        rbTargetGameObject = TargetGameObject.GetComponent<Rigidbody>();
    }

    //void myFunction()
    //{
    //    print("1");
    //    print("2");
    //    print("3");

    //    // myFunction();
    //}

    // Update is called once per frame
    void Update()
    {
        // Lo que est� dentro de la funci�n update, se va a ejecutar cada que se pueda.
        // print("Funcion update");

        // Input.mousePosition // Nos da coordenadas en pixeles.
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        mouseWorldPos.z = 0;  // la sobreescribimos con 0 para que no afecte los c�lculos en 2D.


        //velocity = 

        // Vector3.Angle

        // print(mousePos);

        // �Qu� son coordenadas de Pixel?
        // 1920 (en X) * 1080 (en Y)
        // Esto nos da las coordenadas Normalizadas.
        // Normalizado es que va de 0 a 1
        // 0 de pixeles es el 0 de lo normalizado.
        // 1920 de pixeles el 1 de lo normalizado.
        // Qu� n�mero de pixel estar�a en el 0.5 de lo normalizado? El 960.
        // 1920 * 0.5 = 960
        // 1920 * 0.2 = 384

        // Aplicamos m�todo de Punta Menos Cola
        // Ya tenemos la posici�n del Mouse, Y ya tenemos la posici�n del agente.
        // Podemos decir la posici�n del mouse es la Punta,
        // �cu�l es nuestra posici�n del mouse en el C�digo? la variable "mousePos".
        // y la posici�n de nuestro agente es la cola.
        // �Cu�l es la posici�n de nuestro agente en el C�digo?
        // es la variable position del componente Transform de este GameObject
        // es decir: "transform.position".
        //                  Punta           Menos       Cola
        // Vector3 Distance = mouseWorldPos      -     transform.position;
        // print(Distance);


        // print(Time.deltaTime);

        // Nuestra velocidad es igual a nuestra velocidad actual + (la aceleraci�n * tiempo transcurrido)
        // velocity = velocity + (acceleration * Time.deltaTime);

        // Nuestra nueva posici�n es igual a nuestra posici�n actual + (la velocidad * tiempo transcurrido)
        // transform.position = transform.position + (velocity * Time.deltaTime);

        // Velocidad est� definido como: Distancia / Tiempo

        // Distancia + Distancia/Tiempo  (Esto de aqu� no proceder�a)

        // Distancia + (Distancia/Tiempo)*Tiempo

        // Aceleraci�n Distancia/Tiempo^2
    }

    void FixedUpdate()
    {
        // Fixed: Fijo
        // Solo se ejecuta un n�mero fijo de veces por segundo.
        // Generalmente, ese es n�mero es 60 (o 30).
        // print("Funcion fixedUpdate");

        // La declaramos aqu� para poder usarla DENTRO del switch, pero que siga viva al salir del switch.
        Vector3 Distance = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;

        // Seg�n el valor de la variable currentBehavior, es cu�l Steering Behavior vamos a ejecutar.
        switch (currentBehavior)
        {
            case SteeringBehavior.None:
                {
                    return;
                    // break;
                }
            case SteeringBehavior.Seek:
                {
                    // En qu� direcci�n vamos a hacer que se mueva nuestro agente? En la direcci�n en la que est� el mouse.
                    // Cuando hablemos de direcci�n, queremos vectores normalizados (es decir, de magnitude 1).
                    // Distance = mouseWorldPos - transform.position;
                    steeringForce = Seek(mouseWorldPos);
                    break;
                }
            case SteeringBehavior.Flee:
                {
                    // En qu� direcci�n vamos a hacer que se mueva nuestro agente? En la direcci�n en la que est� el mouse.
                    // Cuando hablemos de direcci�n, queremos vectores normalizados (es decir, de magnitude 1).
                    // Distance = transform.position - mouseWorldPos;
                    steeringForce = Flee(mouseWorldPos);
                    break;
                }
            case SteeringBehavior.Pursuit:
                {
                    steeringForce = Pursuit(TargetGameObject.transform.position, rbTargetGameObject.velocity);
                }
                break;
            case SteeringBehavior.Evade:
                {
                    steeringForce = Evade(TargetGameObject.transform.position, rbTargetGameObject.velocity);
                }
                break;
            case SteeringBehavior.MAX:
                break;
        }


        // Direcci�n actual de movimiento de nuestro agente:
        // La vamos a obtener a partir de la velocidad que trae.
        // Vector3 currentDirection = rb.velocity.normalized;  // Nos da la direcci�n. Es un vector de magnitud 1.
        // float currentMagnitude = rb.velocity.magnitude;



        // Aqu� la limitamos a que sea la m�nima entre la fuerza que marca el algoritmo y la m�xima
        // que deseamos que pueda tener.
        steeringForce = Vector3.Min(steeringForce, steeringForce.normalized * maxSteeringForce);

        rb.AddForce(steeringForce, ForceMode.Acceleration);
    }

    private Vector3 GetSteeringForce(Vector3 DistanceVector)
    {
        Vector3 desiredDirection = DistanceVector.normalized;  // queremos la direcci�n de ese vector, pero de magnitud 1.

        // queremos ir para esa direcci�n lo m�s r�pido que se pueda.
        Vector3 desiredVelocity = desiredDirection * maxSpeed;

        // La diferencia entre la velocidad que tenemos actualmente y la que queremos tener.
        Vector3 steeringForce = desiredVelocity - rb.velocity;

        return steeringForce;
    }

    private Vector3 Seek(Vector3 TargetPosition)
    {
        return GetSteeringForce(TargetPosition - transform.position);
    }

    private Vector3 Flee(Vector3 TargetPosition)
    {
        return GetSteeringForce(transform.position - TargetPosition);
    }

    private Vector3 Pursuit(Vector3 TargetPosition, Vector3 TargetVelocity)
    {
        Vector3 predictedPosition = PredictPosition(TargetPosition, TargetVelocity);

        return Seek(predictedPosition);
    }

    private Vector3 Evade(Vector3 TargetPosition, Vector3 TargetVelocity)
    {
        Vector3 predictedPosition = PredictPosition(TargetPosition, TargetVelocity);

        return Flee(predictedPosition);
    }

    private Vector3 PredictPosition(Vector3 TargetPosition, Vector3 TargetVelocity)
    {
        // Pursuit no es mucho m�s que hacerle Seek a la posici�n futura del objetivo.
        // Primero calculamos el tiempo T que nos tomar�a llegar al TargetPosition.
        Vector3 Distance = transform.position - TargetPosition;
        // con esa distancia, podemos saber cu�nto tiempo nos tomar� recorrer esa 
        // distancia usando nuestra m�xima velocidad.
        // TiempoT = Distancia/MaxSpeed
        float predictedTime = Distance.magnitude / maxSpeed;
        // usamos Distance.magnitude porque queremos cu�nto mide el vector, no hacia d�nde (o no hacia qu� direcci�n).
        // Ahora s� podemos predecir la posici�n futura de nuestro TargetObject.
        // Su posici�n futura es: Su posici�n actual + su velocidad * cu�nto tiempo transcurre.
        Vector3 predictedPosition = TargetPosition + TargetVelocity * predictedTime;

        return predictedPosition;
    }

    // Pursuit
    // Idea general es "no vayas ahorita hacia donde est� tu objetivo,
    // ve hacia donde va a estar despu�s de cierto tiempo."
    // queremos saber la posici�n actual de ese target.
    // queremos saber la velocidad actual de ese target.
    // �cu�l va a ser el tiempo en el que queremos predecir su posici�n?
    // 


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, mouseWorldPos);

        // Esto es para verificar que Pursuit est� funcionando adecuadamente.
        // Comprobamos que s� lo hace.
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position,
          //  PredictPosition(TargetGameObject.transform.position, rbTargetGameObject.velocity));


    }
}
