using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum MovementStates
{
    None,
    Walk,
    Run
}

public class PM : MonoBehaviour
{
    private InputControls _inputMapping;
    private Camera _camera;
    private Vector3 _moveTarget = Vector3.zero;
    private Quaternion _lookRotation = Quaternion.identity;
    private Vector3 _direction = Vector3.zero;
    private bool _needToRotate = false;
    private float _rotateSpeed = 50f;
    public float _walkSpeed = 2.5f;
    public float _runSpeed = 4f;
    public ParticleSystem WalkDecal;
    private MovementStates _currentMovement;

    public MovementStates CurrentMovement
    {
        get => _currentMovement;
        set
        {
            switch (value)
            {
                case MovementStates.Walk:
                    // Adjust speed directly here
                    break;
                case MovementStates.Run:
                    // Adjust speed directly here
                    break;
            }

            _currentMovement = value;
        }
    }

    private void Awake() => _inputMapping = new InputControls();

    void Start()
    {
        // Register listener events for inputs
        _inputMapping.Default.Walk.performed += Walk;
        _inputMapping.Default.Run.performed += Run;

        _camera = Camera.main;
    }

    private void Update()
    {
        if (!_needToRotate)
        {
            transform.Translate(_direction * Time.deltaTime , Space.World);

            if (Vector3.Distance(transform.position, _moveTarget) <= 0.1f)
            {
                _needToRotate = true;
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _rotateSpeed);

            if (Quaternion.Angle(transform.rotation, _lookRotation) <= 1f)
            {
                _needToRotate = false;
            }
        }
    }

    private void OnEnable() => _inputMapping.Enable();

    private void OnDisable() => _inputMapping.Disable();

    private void Run(CallbackContext context)
    {
        CurrentMovement = MovementStates.Run;
    }

    private void Walk(CallbackContext context)
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            _moveTarget = hit.point;

            WalkDecal.transform.position = _moveTarget.WithNewY(0.1f);
            WalkDecal.Play();

            _direction = (_moveTarget - transform.position).normalized;
            _lookRotation = Quaternion.LookRotation(_direction, Vector3.up);
            _needToRotate = true;

            CurrentMovement = MovementStates.Walk;
        }
    }
}
