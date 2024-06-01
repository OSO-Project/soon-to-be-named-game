using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerInput _playerInput;
    private InputActionMap _currentMap;

    public Vector2 Move { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }

    public bool Interact { get; private set; }

    public InputAction MoveAction;
    public InputAction JumpAction;
    public InputAction RunAction;

    public InputAction InteractAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _playerInput = GetComponent<PlayerInput>();
        _currentMap = _playerInput.currentActionMap;

        InitialiseActions();
    }

    private void InitialiseActions()
    {
        MoveAction = _currentMap.FindAction("Move");
        JumpAction = _currentMap.FindAction("Jump");
        RunAction = _currentMap.FindAction("Run");
        InteractAction = _currentMap.FindAction("Interact");
        MoveAction.performed += OnMove;
        MoveAction.canceled += OnMove;
        JumpAction.performed += OnJump;
        JumpAction.canceled += OnJump;
        RunAction.performed += OnRun;
        RunAction.canceled += OnRun;
        InteractAction.performed += OnInteract;
        InteractAction.canceled += OnInteract;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        Run = context.ReadValueAsButton();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Interact = true;
        }
        else if (context.canceled)
        {
            Interact = false;
        }
    }

    private void OnEnable()
    {
        _currentMap.Enable();
    }

    private void OnDisable()
    {
        _currentMap.Disable();
    }
}