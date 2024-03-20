using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public Transform agent; // El agente que tiene esta visi�n de cono
    public float visionAngle = 45f; // �ngulo de visi�n del cono
    public float visionDistance = 10f; // Distancia m�xima de visi�n

    public bool isDetected = false; // Indica si el agente ha detectado algo

    // M�todo para detectar objetos dentro del cono de visi�n
    private void DetectObjects()
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
        }
    }

    // M�todo para dibujar el cono de visi�n y mostrar visualmente la detecci�n
    private void OnDrawGizmos()
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

    // M�todo que se llama en cada frame
    private void Update()
    {
        DetectObjects(); // Detectamos objetos en el cono de visi�n
    }
}
