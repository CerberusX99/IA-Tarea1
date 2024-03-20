using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentT2 : MonoBehaviour
{
    public float speed = 5f; // Velocidad del agente
    public float rotationSpeed = 2f; // Velocidad de rotación del agente
    private List<GameObject> targetCubes = new List<GameObject>(); // Lista de cubos objetivo que representan los nodos
    private int currentTargetIndex = 0; // Índice del cubo objetivo actual al que el agente se dirige
    private bool reachedDestination = false; // Variable para rastrear si el agente ha alcanzado el destino

    void Start()
    {
        // Obtener la lista de cubos objetivo que representan los nodos del componente BaseGraph
        BaseGraph baseGraph = FindObjectOfType<BaseGraph>();
        if (baseGraph != null)
        {
            targetCubes = baseGraph.GetTargetCubes();
        }
    }

    void Update()
    {
        // Verificar si el agente ha alcanzado el destino y detener el movimiento
        if (reachedDestination)
        {
            return;
        }

        // Verificar si hay cubos objetivo en la lista de cubos a seguir
        if (targetCubes.Count > 0)
        {
            // Obtener la dirección hacia el cubo objetivo actual
            Vector3 direction = (targetCubes[currentTargetIndex].transform.position - transform.position).normalized;

            // Rotar hacia la dirección del cubo objetivo actual
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            // Moverse hacia adelante en la dirección del cubo objetivo actual
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Verificar si el agente está lo suficientemente cerca del cubo objetivo actual
            if (Vector3.Distance(transform.position, targetCubes[currentTargetIndex].transform.position) < 0.1f)
            {
                // Avanzar al siguiente cubo objetivo en la lista
                currentTargetIndex++;

                // Verificar si el agente ha alcanzado el último cubo objetivo
                if (currentTargetIndex >= targetCubes.Count)
                {
                    // Llegó al destino, por lo que puede dejar de moverse
                    reachedDestination = true;
                    targetCubes.Clear();
                }
            }
        }
    }
}
