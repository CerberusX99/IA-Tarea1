using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public string ID;
    public Node parent;
    public Vector3 position; // Posición en el espacio de Unity

    public Node(string in_Id, Vector3 in_position)
    {
        ID = in_Id;
        parent = null;
        position = in_position;
    }
}

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

public enum NodeState
{
    Unknown = 0,
    Open,
    Closed,
    MAX
}

public class BaseGraph : MonoBehaviour
{
    public GameObject NA;
    public GameObject NB;
    public GameObject NC;
    public GameObject ND;
    public GameObject NE;
    public GameObject NF;
    public GameObject NG;
    public GameObject NH;
    public Color targetColor = Color.red;
    public float colorChangeDelay = 0.5f; // Retraso entre cambios de color
    public List<Edge> Edges = new List<Edge>();
    public List<Node> Nodes = new List<Node>();
    public Dictionary<Node, NodeState> NodeStateDict = new Dictionary<Node, NodeState>();
    public Queue<Node> OpenQueue = new Queue<Node>();
    public HashSet<Node> ClosedSetList = new HashSet<Node>();

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

        Edges.Add(AB);
        Edges.Add(AE);
        Edges.Add(BC);
        Edges.Add(BD);
        Edges.Add(EF);
        Edges.Add(EG);
        Edges.Add(EH);

        // Iniciar BFS
        NodeStateDict[H] = NodeState.Open;
        bool pathExists = IterativeBFS(H, D);
        if (pathExists)
        {
            Debug.Log("Sí hay un camino de H a D.");
            PrintPath(D);
        }
        else
            Debug.Log("No hay camino de H a D.");
    }

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

    public void PrintPath(Node target)
    {
        List<Node> pathToGoal = new List<Node>();
        Node currentNode = target;
        while (currentNode != null)
        {
            pathToGoal.Add(currentNode);
            currentNode = currentNode.parent;
        }
        pathToGoal.Reverse(); // Reverse the path to print in correct order

        StartCoroutine(ColorPath(pathToGoal));
    }

    IEnumerator ColorPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Debug.Log("El nodo: " + node.ID + " fue parte del camino a la meta.");
            ColorNode(node);
            yield return new WaitForSeconds(colorChangeDelay);
        }
    }

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
