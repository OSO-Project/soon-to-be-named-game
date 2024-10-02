using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class Vacuum : Tool
{
    public override string Name => "Vacuum";

    private List<GameObject> touchedObjects = new List<GameObject>();

    void Start()
    {
        InputManager.Instance.UseToolAction.performed += OnUse;
    }
    private void OnDestroy()
    {
        InputManager.Instance.UseToolAction.performed -= OnUse;
    }

    void OnTriggerEnter(Collider other)
    {
        if (ToolsManager.Instance._currentlyHeld is Vacuum)
        {
            Cleanable cleanable = other.gameObject.GetComponent<Cleanable>();
            if (cleanable != null)
            {
                if (cleanable.IsClean == true)
                {
                    return;
                }
                if (ToolsManager.Instance._currentlyHeld is Vacuum)
                {
                    cleanable.ToggleObjectUI(true, CleanableType.Vacuumable);
                }
                touchedObjects.Add(other.gameObject);
                if (touchedObjects.Count > 0)
                {
                    UIManager.Instance.HintText.gameObject.SetActive(true);
                }
            }
        }  
    }

    void OnTriggerExit(Collider other)
    {
        if (ToolsManager.Instance._currentlyHeld is Vacuum)
        {
            Cleanable cleanable = other.gameObject.GetComponent<Cleanable>();
            if (cleanable != null)
            {
                if (ToolsManager.Instance._currentlyHeld is Vacuum)
                {
                    cleanable.ToggleObjectUI(false, CleanableType.Vacuumable);
                }
                touchedObjects.Remove(other.gameObject);
                if (touchedObjects.Count == 0)
                {
                    UIManager.Instance.HintText.gameObject.SetActive(false);
                }
            }
        }
        
    }

    public override void OnUse(InputAction.CallbackContext context)
    {
        foreach (GameObject obj in touchedObjects)
        {
            obj.gameObject.GetComponent<Cleanable>().HandleToolUse();
        }
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
