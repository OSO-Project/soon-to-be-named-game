using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class Vacuum : Tool
{
    public override string Name => "Vacuum";


    void Start()
    {
        InputManager.Instance.UseToolAction.performed += OnUse;
    }
    private void OnDestroy()
    {
        InputManager.Instance.UseToolAction.performed -= OnUse;
    }

    public override void OnUse(InputAction.CallbackContext context)
    {

    }

    
    public override void Equip()
    {
        return;
    }
    public override void UnEquip()
    {
        return;
    }
}
