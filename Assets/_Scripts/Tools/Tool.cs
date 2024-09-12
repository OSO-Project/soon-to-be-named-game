using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Tool : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract void OnUse(InputAction.CallbackContext context);
    public abstract void Equip();
    public abstract void UnEquip();
}