using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentT2 : MonoBehaviour
{
    public float speed = 5f; // Velocidad del agente
    public float rotationSpeed = 2f; // Velocidad de rotación del agente
    private List<Node> pathToFollow = new List<Node>(); // Lista de nodos que el agente debe seguir
    private int currentNodeIndex = 0; // Índice del nodo actual al que el agente se dirige

    void Start()
    {
        // Obtener la lista de nodos que el agente debe seguir del componente BaseGraph
        BaseGraph baseGraph = FindObjectOfType<BaseGraph>();
        if (baseGraph != null)
        {
            pathToFollow = baseGraph.GetPathToFollow();
        }
    }

    void Update()
    {
        // Verificar si hay nodos en la lista de nodos a seguir
        if (pathToFollow.Count > 0)
        {
            // Obtener la dirección hacia el nodo actual
            Vector3 direction = (pathToFollow[currentNodeIndex].position - transform.position).normalized;

            // Rotar hacia la dirección del nodo actual
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            // Moverse hacia adelante en la dirección del nodo actual
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Verificar si el agente está lo suficientemente cerca del nodo actual
           // Verificar si el agente está lo suficientemente cerca del nodo actual
if (Vector3.Distance(transform.position, pathToFollow[currentNodeIndex].position) < 0.1f)
{
    // Avanzar al siguiente nodo en la lista
    currentNodeIndex++;

    // Verificar si el agente ha alcanzado el último nodo
    if (currentNodeIndex >= pathToFollow.Count)
    {
        // Llegó al destino, por lo que puede dejar de moverse
        pathToFollow.Clear();
    }
}

        }
    }
}