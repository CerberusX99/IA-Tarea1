using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class RandomM : MonoBehaviour 
{
    public NavMeshAgent agent; // El agente que se moverá.
    public Transform player; // El jugador al que el agente seguirá.
    public Vector3 walkPoint; // El punto al que el agente se dirigirá.
    public bool walkPointSet; // Si el agente tiene un punto de destino establecido.
    public float walkPointRange; // El rango del punto de destino.
    public float timeBetweenAttacks; // El tiempo entre ataques.
    public bool alreadyAttacked; // Si el agente ya ha atacado.
    public float sightRange, attackRange; // El rango de visión y ataque.
    public bool playerInSightRange, playerInAttackRange; // Si el jugador está dentro del rango de visión o ataque.
    public AudioClip deathSound;
    public AudioSource audioSource;
    public string sceneToLoad;

    private void Awake()
    {
        player = GameObject.Find("AgentN").transform; // Encuentra al jugador.
        agent = GetComponent<NavMeshAgent>(); // Obtiene el componente del agente.
    }

 
private void Update()
{
    // Comprueba si el jugador está dentro del rango de visión o ataque.
    playerInSightRange = IsPlayerInSightRange();
    playerInAttackRange = IsPlayerInAttackRange();

    // Si el jugador no está en el rango de visión ni en el rango de ataque, patrulla.
    if (!playerInSightRange && !playerInAttackRange)
        Patroling();
    
    // Si el jugador está en el rango de visión pero no en el rango de ataque, persigue al jugador.
    if (playerInSightRange && !playerInAttackRange)
        ChasePlayer();
    
    // Si el jugador está en el rango de visión y en el rango de ataque, ataca al jugador.
    if (playerInSightRange && playerInAttackRange)
        AttackPlayer();
}

// Comprueba si el jugador está dentro del rango de visión del agente.
private bool IsPlayerInSightRange()
{
    Vector3 directionToPlayer = player.position - transform.position;
    if (directionToPlayer.magnitude <= sightRange)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
        {
            // Si el rayo choca con el jugador, devuelve verdadero.
            if (hit.transform == player)
            {
                return true;
            }
        }
    }
    // Si no se encuentra al jugador, devuelve falso.
    return false;
}

// Comprueba si el jugador está dentro del rango de ataque del agente.
private bool IsPlayerInAttackRange()
{
    Vector3 directionToPlayer = player.position - transform.position;
    // Si la distancia al jugador es menor o igual que el rango de ataque, devuelve verdadero.
    return directionToPlayer.magnitude <= attackRange;
}


    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint(); // Si el agente no tiene un punto de destino, busca uno.
        if (walkPointSet) agent.SetDestination(walkPoint); // Si el agente tiene un punto de destino, muévete hacia él.

        Vector3 distanceToWalkPoint = transform.position - walkPoint; // La distancia al punto de destino.

        // Si el agente está cerca del punto de destino, establece walkPointSet como falso.
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calcula un punto aleatorio dentro del rango.
        Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, NavMesh.AllAreas);

        // Establece el punto de destino como el punto más cercano en el NavMesh.
        walkPoint = hit.position;

        // Establece walkPointSet como verdadero.
        walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position); // Establece el destino al jugador.
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position); // Establece el destino a la posición del agente.

        transform.LookAt(player); // Mira hacia el jugador.

        if (!alreadyAttacked) // Si el agente no ha atacado.
        {
            // Código de ataque aquí.

            alreadyAttacked = true; // Establece alreadyAttacked como verdadero.
            Invoke(nameof(ResetAttack), timeBetweenAttacks); // Reinicia el ataque después de un tiempo.
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false; // Establece alreadyAttacked como falso.
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (deathSound != null )
            {
                audioSource.PlayOneShot(deathSound);
                Invoke("LoadNextScene", deathSound.length); 
            }
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}

