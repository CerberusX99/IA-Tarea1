using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovementController : MonoBehaviour
{
    private InputControls _inputMapping;
    private Camera _camera;
    private NavMeshAgent _agent;
    private Vector3 _moveTarget = Vector3.zero;
    private Quaternion _lookRotation = Quaternion.identity;
    private Vector3 _direction = Vector3.zero;
    private bool _needToRotate = false;
    private float _rotateSpeed = 10f;
    public float _walkSpeed = 2.5f;
    public float _runSpeed = 4f;
    public ParticleSystem WalkDecal;
    private MovementStates _currentMovement;
    private Animator animator;
    public MovementStates CurrentMovement
    {
        get => _currentMovement;
        set
        {
            switch (value)
            {
                case MovementStates.Walk:
                    _agent.speed = 2.5f;
                    break;
                case MovementStates.Run:
                    _agent.speed = 4f;
                    break;
            }

            _currentMovement = value;
        }
    }

    /// <summary>
    /// Returns true if the agent is in the middle of pathfinding or the agent has a remaining distance greater than 1/2 a meter (0.5f)
    /// </summary>
    public bool IsNavigating => _agent.pathPending || _agent.remainingDistance > .25f;

    private void Awake() => _inputMapping = new InputControls();

    void Start()
    {
        animator = GetComponent<Animator>();
        // Register listener events for inputs
        _inputMapping.Default.Walk.performed += Walk;
        _inputMapping.Default.Run.performed += Run;

        _camera = Camera.main;
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        // Confirm that the player is done navigating
        if (!_needToRotate && !IsNavigating && _currentMovement != MovementStates.None)
        {
            StopNavigation();
            animator.SetBool("Run", false);
        }
        // Navigation is likely starting
        else if (_needToRotate)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _rotateSpeed);

            if (Vector3.Dot(_direction, transform.forward) >= .99f)
            {
                _agent.SetDestination(_moveTarget);
                // AnimationController.Instance.CurrentState = CurrentMovement;
                animator.SetBool("Run",true);
                _needToRotate = false;
            }
        }
    }

    private void OnEnable() => _inputMapping.Enable();

    private void OnDisable() => _inputMapping.Disable();

    /// <summary>
    /// Called when the player does a double left mouse button click
    /// </summary>
    private void Run(CallbackContext context)
    {
        CurrentMovement = MovementStates.Run;
        // AnimationController.Instance.CurrentState = CurrentMovement;
        animator.SetBool("Run",true);

    }

    /// <summary>
    /// Called when the player does a single left mouse button click
    /// </summary>
    /// 

    private float _evadeDistance = 5f; // Definir evadeDistance como campo privado en tu clase
    private float _fleeForce = 5f; // Definir fleeForce como campo privado en tu clase

    private void Walk(CallbackContext context)
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navPos, .25f, 1 << 0))
            {
                // Stop navigating
                StopNavigation();

                _moveTarget = navPos.position;

                // Show the walk decal
                WalkDecal.transform.position = _moveTarget.WithNewY(0.1f);
                WalkDecal.Play();

                // Calculate rotation direction
                _direction = (_moveTarget.WithNewY(transform.position.y) - transform.position).normalized;
                _lookRotation = Quaternion.LookRotation(_direction, Vector3.up);
                _needToRotate = true;

                // Set the speed and ready the animation
                CurrentMovement = MovementStates.Walk;

                if (IsNavigating && Vector3.Dot(_direction, transform.forward) >= 0.25f)
                {
                    // Llama a ApplyArriveBehavior con evadeDistance y fleeForce
                    ApplyArriveBehavior(_moveTarget, _evadeDistance, _fleeForce);
                }
            }
        }
    }

    /// <summary>
    /// Stops the player from moving.
    /// </summary>
    private void StopNavigation()
    {
        _agent.SetDestination(transform.position);
        CurrentMovement = MovementStates.None;
        // AnimationController.Instance.CurrentState = CurrentMovement;
    }

    private void ApplyArriveBehavior(Vector3 targetPosition, float evadeDistance, float fleeForce)
    {
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        // Calculate the desired speed based on distance
        float desiredSpeed = Mathf.Clamp(distance / _agent.stoppingDistance, 0, _agent.speed);

        // Set the desired velocity towards the target
        Vector3 desiredVelocity = direction.normalized * desiredSpeed;

        // Calculate the steering force
        Vector3 steering = desiredVelocity - _agent.velocity;

        Collider[] obstacles = Physics.OverlapSphere(transform.position, evadeDistance);
        foreach (Collider obstacle in obstacles)
        {
            Vector3 toObstacle = obstacle.transform.position - transform.position;
            if (toObstacle.magnitude < evadeDistance)
            {
                // If an obstacle is within evadeDistance, apply flee behavior
                Vector3 fleeDirection = transform.position - obstacle.transform.position;
                steering += fleeDirection.normalized * fleeForce;
            }
        }

        // Apply the steering force to the agent
        _agent.velocity += steering;

        // Set the agent's destination to stop it when it gets close enough
        _agent.SetDestination(targetPosition);
    }
}
