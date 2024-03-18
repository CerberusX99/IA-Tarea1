using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Se define un nodo en el grafo con un identificador único,
una referencia al nodo padre (para la reconstrucción
del camino) y una posición en el espacio de Unity. */
public class Node
{
    public string ID; //identificador unico del nodo
    public Node parent; // Nodo padre en el camino encontrado
    public Vector3 position; // Posición en el espacio de Unity

    //constructor de la clase Node
    public Node(string in_Id, Vector3 in_position)
    {
        ID = in_Id;
        parent = null;
        position = in_position;
    }
}

//Estructura para reprensentar una arista en el grafo
public struct Edge
{
    public Node a;
    public Node b;

    public Edge(Node in_a, Node in_b)
    {
        a = in_a;
        b = in_b;
    }
}

/* Este metodo enumera los posibles estados de un
   nodo durante la busqueda.Unknown se utiliza para 
   inicializar el estado, Open para marcar un nodo como 
   visitado pero aun no explorado, y Closed para marcar un
   nodo como visitado y explorado.*/
public enum NodeState
{
    Unknown = 0, // Estado inicial desconocido
    Open,  // Nodo marcado como abierto pero no explorado
    Closed,// Nodo marcado como explorado
    MAX
}

/* Se define el grafo y el algoritmo BFS para encontrar un
 camino desde un nodo de origen a un nodo objetivo*/
public class BaseGraph : MonoBehaviour
{
    //GameObjects para representar los nodos en Unity
    public GameObject NA;
    public GameObject NB;
    public GameObject NC;
    public GameObject ND;
    public GameObject NE;
    public GameObject NF;
    public GameObject NG;
    public GameObject NH;
     private List<Node> pathToFollow = new List<Node>();
    public Color targetColor = Color.red; // Color para resaltar el camino encontrado
    public float colorChangeDelay = 0.5f; // Retraso entre cambios de color

    // Listas y diccionarios para almacenar nodos, aristas y estados de nodos
    public List<Edge> Edges = new List<Edge>();
    public List<Node> Nodes = new List<Node>();
    public Dictionary<Node, NodeState> NodeStateDict = new Dictionary<Node, NodeState>();

    // Cola para los nodos abiertos y conjunto para los nodos cerrados
    public Queue<Node> OpenQueue = new Queue<Node>();
    public HashSet<Node> ClosedSetList = new HashSet<Node>();

    // Método para crear un grafo de prueba y realizar la búsqueda BFS
    private void GrafoDePrueba()
    {
        // Crear nodos y definir posiciones
        Node A = new Node("A", new Vector3(0, 0, 0));
        Node B = new Node("B", new Vector3(1, 0, 0));
        Node C = new Node("C", new Vector3(2, 0, 0));
        Node D = new Node("D", new Vector3(2, 1, 0));
        Node E = new Node("E", new Vector3(1, 1, 0));
        Node F = new Node("F", new Vector3(0, 1, 0));
        Node G = new Node("G", new Vector3(0, 2, 0));
        Node H = new Node("H", new Vector3(1, 2, 0));

        // Agregar nodos
        Nodes.Add(A);
        Nodes.Add(B);
        Nodes.Add(C);
        Nodes.Add(D);
        Nodes.Add(E);
        Nodes.Add(F);
        Nodes.Add(G);
        Nodes.Add(H);

        // Inicializar los estados de los nodos como desconocidos
        NodeStateDict.Add(A, NodeState.Unknown);
        NodeStateDict.Add(B, NodeState.Unknown);
        NodeStateDict.Add(C, NodeState.Unknown);
        NodeStateDict.Add(D, NodeState.Unknown);
        NodeStateDict.Add(E, NodeState.Unknown);
        NodeStateDict.Add(F, NodeState.Unknown);
        NodeStateDict.Add(G, NodeState.Unknown);
        NodeStateDict.Add(H, NodeState.Unknown);

        // Crear aristas
        Edge AB = new Edge(A, B);
        Edge AE = new Edge(A, E);
        Edge BC = new Edge(B, C);
        Edge BD = new Edge(B, D);
        Edge EF = new Edge(E, F);
        Edge EG = new Edge(E, G);
        Edge EH = new Edge(E, H);

        // Agregar aristas al grafo
        Edges.Add(AB);
        Edges.Add(AE);
        Edges.Add(BC);
        Edges.Add(BD);
        Edges.Add(EF);
        Edges.Add(EG);
        Edges.Add(EH);

      // Iniciar BFS desde el nodo H hacia el nodo D
        NodeStateDict[H] = NodeState.Open;
        bool pathExists = IterativeBFS(H, D);
        if (pathExists)
        {
            Debug.Log("Sí hay un camino de H a D.");
            StorePathToFollow(D); // Almacenar el camino encontrado
            PrintPath(D);
        }
        else
            Debug.Log("No hay camino de H a D.");
    }
  private void StorePathToFollow(Node target)
{
    pathToFollow.Clear();
    Node currentNode = target;
    while (currentNode != null)
    {
        pathToFollow.Add(currentNode);
        currentNode = currentNode.parent;
    }
    pathToFollow.Reverse(); // Invertir el camino para que sea en el orden correcto
}

    // Método para obtener el camino que el agente debe seguir
    public List<Node> GetPathToFollow()
    {
        return pathToFollow;
    }

    // Método para realizar la búsqueda BFS de manera iterativa
    /* *Se implementa el algoritmo de busqueda en anchura de manera
       iterativa.
       *Utiliza una cola para almacenar los nodos abiertos y un
       conjunto para los nodos cerrados
       * Explora los nodos vecinos del nodo actual hasta encontrar
         encontrar el nodo objetivo o agotar todos los nodos.*/
    public bool IterativeBFS(Node Origin, Node Target)
    {
        OpenQueue.Enqueue(Origin);

        while (OpenQueue.Count != 0)
        {
            Node currentNode = OpenQueue.Dequeue();
            List<Edge> currentNeighbors = FindNeighbors(currentNode);

            foreach (Edge e in currentNeighbors)
            {
                Node NonCurrentNode = currentNode != e.a ? e.a : e.b;
                if (ClosedSetList.Contains(NonCurrentNode))
                    continue;

                if (NonCurrentNode == Target)
                {
                    NonCurrentNode.parent = currentNode;
                    return true;
                }
                else
                {
                    OpenQueue.Enqueue(NonCurrentNode);
                    NonCurrentNode.parent = currentNode;
                }
            }

            ClosedSetList.Add(currentNode);
        }

        return false;
    }

    // Metodo para imprimir el camino encontrado
    /* *Reconstruye y muestra el camino desde el nodo 
      objetivo hasta el nodo de inicio.
       *Utiliza el método ColorPath() para 
        resaltar visualmente el camino en Unity. */
    public void PrintPath(Node target)
    {
        List<Node> pathToGoal = new List<Node>();
        Node currentNode = target;
        while (currentNode != null)
        {
            pathToGoal.Add(currentNode);
            currentNode = currentNode.parent;
        }
        pathToGoal.Reverse(); // Invertir el camino para imprimirlo en orden correcto

        StartCoroutine(ColorPath(pathToGoal));
    }




    // Corrutina para cambiar el color de los nodos en el camino
    /* Cambia el color de los nodos en el camino para resaltarlos
       visualmente en Unity.
       *Utiliza una corrutina para aplicar un retraso entre 
       cada cambio de color.*/
    IEnumerator ColorPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Debug.Log("El nodo: " + node.ID + " fue parte del camino a la meta.");
            ColorNode(node);  // Cambiar el color del nodo en Unity
            yield return new WaitForSeconds(colorChangeDelay);
        }
    }

    // Método para encontrar los nodos vecinos de un nodo dado
    /* Encuentra los nodos vecinos de un nodo dado en el grafo.*/
    public List<Edge> FindNeighbors(Node in_node)
    {
        List<Edge> out_list = new List<Edge>();
        foreach (Edge myEdge in Edges)
        {
            if (myEdge.a == in_node || myEdge.b == in_node)
            {
                out_list.Add(myEdge);
            }
        }
        return out_list;
    }

    void Start()
    {
        GrafoDePrueba();
    }

    void Update()
    {

    }

    void ColorNode(Node node)
    {
        switch (node.ID)
        {
            case "A":
                NA.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "B":
                NB.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "C":
                NC.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "D":
                ND.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "E":
                NE.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "F":
                NF.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "G":
                NG.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "H":
                NH.GetComponent<Renderer>().material.color = targetColor;
                break;
            default:
                break;
        }
    }
}
