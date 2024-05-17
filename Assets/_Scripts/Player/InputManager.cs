using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager current;

    private PlayerInput _playerInput;
    private InputActionMap _currentMap;

    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }

    public InputAction MoveAction;
    public InputAction JumpAction;

    private void Awake()
    {
        current = this;

        _playerInput = GetComponent<PlayerInput>();
        _currentMap = _playerInput.currentActionMap;

        InitialiseActions();
    }

    private void InitialiseActions()
    {
        MoveAction = _currentMap.FindAction("Move");
        JumpAction = _currentMap.FindAction("Jump");

        MoveAction.performed += OnMove;
        MoveAction.canceled += OnMove;
        JumpAction.performed += OnJump;
        JumpAction.canceled += OnJump;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
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