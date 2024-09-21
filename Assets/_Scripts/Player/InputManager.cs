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
    public bool Crouch { get; private set; }
    public bool Clean { get; private set; }

    public bool Interact { get; private set; }
    public bool OpenClose { get; private set; }
    public bool GrabDrop { get; private set; }
    public bool Throw { get; private set; }
    public bool UseTool { get; private set; }
    public bool EquipWipe { get; private set; }
    public bool EquipVacuum { get; private set; }
    public bool EquipTrashBag { get; private set; }
    public bool EquipMagicVacuum { get; private set; }

    public InputAction MoveAction;
    public InputAction JumpAction;
    public InputAction RunAction;
    public InputAction CrouchAction;

    public InputAction CleanAction;
    public InputAction GrabDropAction;
    public InputAction ThrowAction;
    public InputAction OpenCloseAction;

    public InputAction UseToolAction;
    public InputAction EquipWipeAction;
    public InputAction EquipVacuumAction;
    public InputAction EquipTrashBagAction;
    public InputAction EquipMagicVacuumAction;

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
        CrouchAction = _currentMap.FindAction("Crouch");
        CleanAction = _currentMap.FindAction("Clean");
        ThrowAction = _currentMap.FindAction("Throw");
        OpenCloseAction = _currentMap.FindAction("OpenClose");
        GrabDropAction = _currentMap.FindAction("GrabDrop");
        UseToolAction = _currentMap.FindAction("UseTool");
        EquipWipeAction = _currentMap.FindAction("EquipWipe");
        EquipVacuumAction = _currentMap.FindAction("EquipVacuum");
        EquipTrashBagAction = _currentMap.FindAction("EquipTrashBag");
        EquipMagicVacuumAction = _currentMap.FindAction("EquipMagicVacuum");
        InteractAction = _currentMap.FindAction("Interact");
        InteractAction.performed += OnInteract;
        InteractAction.canceled += OnInteract;
        MoveAction.performed += OnMove;
        MoveAction.canceled += OnMove;
        JumpAction.performed += OnJump;
        JumpAction.canceled += OnJump;
        RunAction.performed += OnRun;
        RunAction.canceled += OnRun;
        CrouchAction.performed += OnCrouch;
        CrouchAction.canceled += OnCrouch;
        CleanAction.performed += OnClean;
        CleanAction.canceled += OnClean;
        ThrowAction.performed += OnThrow;
        ThrowAction.canceled += OnThrow;
        OpenCloseAction.performed += OnOpenClose;
        OpenCloseAction.canceled += OnOpenClose;
        GrabDropAction.performed += OnGrabDrop;
        GrabDropAction.canceled += OnGrabDrop;
        UseToolAction.performed += OnUseTool;
        UseToolAction.canceled += OnUseTool;
        EquipWipeAction.performed += context =>
        {
            Debug.Log("EquipWipeAction performed");
            ToolsManager.Instance.EquipTool(1);
        };
        EquipVacuumAction.performed += context =>
        {
            Debug.Log("EquipVacuumAction performed");
            ToolsManager.Instance.EquipTool(2);
        };
        EquipTrashBagAction.performed += context =>
        {
            Debug.Log("EquipTrashBagAction performed");
            ToolsManager.Instance.EquipTool(3);
        };
        EquipMagicVacuumAction.performed += context =>
        {
            Debug.Log("EquipMagicVacuumAction performed");
            ToolsManager.Instance.EquipTool(4);
        };
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

    private void OnCrouch(InputAction.CallbackContext context)
    {
        Crouch = context.ReadValueAsButton();
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        Throw = context.ReadValueAsButton();
    }

    private void OnClean(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Clean = true;
        }
        else if (context.canceled)
        {
            Clean = false;
        }
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

    private void OnGrabDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GrabDrop = true;
        }
        else if (context.canceled)
        {
            GrabDrop = false;
        }
    }

    private void OnOpenClose(InputAction.CallbackContext context)
    {
        OpenClose = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        _currentMap.Enable();
    }

    private void OnDisable()
    {
        _currentMap.Disable();
    }

    private void OnUseTool(InputAction.CallbackContext context)
    {
        UseTool = context.ReadValueAsButton();
    }

    private void UpdateInputs()
    {
        Move = MoveAction.ReadValue<Vector2>();
        Jump = JumpAction.WasPressedThisFrame();
        Run = RunAction.IsPressed();
        Clean = CleanAction.IsPressed();
        Crouch = CrouchAction.IsPressed();
        Throw = ThrowAction.IsPressed();
        OpenClose = OpenCloseAction.IsPressed();
        GrabDrop = GrabDropAction.IsPressed();

    }
}