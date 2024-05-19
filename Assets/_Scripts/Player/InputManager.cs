using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager current;

    private PlayerInput _playerInput;
    private InputActionMap _currentMap;

    public Vector2 Move { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }

    public InputAction MoveAction;
    public InputAction JumpAction;
    public InputAction RunAction;

        _playerInput = GetComponent<PlayerInput>();
        _currentMap = _playerInput.currentActionMap;

        InitialiseActions();

    private void InitialiseActions()
    {
        MoveAction = _currentMap.FindAction("Move");
        JumpAction = _currentMap.FindAction("Jump");
        RunAction = _currentMap.FindAction("Run");
        JumpAction.canceled += OnJump;
        RunAction.performed += OnRun;
        RunAction.canceled += OnRun;
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

    private void OnEnable()
    {
        _currentMap.Enable();
    }

    private void OnDisable()
    {
        _currentMap.Disable();
    }
}