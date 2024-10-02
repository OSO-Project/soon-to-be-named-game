using UnityEngine.InputSystem;
using UnityEngine;

public class Wipe : Tool
{
    public override string Name => "Wipe";
    public int _dirtLevel = 0;

    void Start()
    {
        InputManager.Instance.UseToolAction.performed += OnUse;
    }
    private void OnDestroy()
    {
        InputManager.Instance.UseToolAction.performed -= OnUse;
    }
    public bool IsWipeSuccessful()
    {
        // Return true immediately if dirt level is 50 or less
        if (_dirtLevel <= 49)
        {
            _dirtLevel += 10;
            return true;
        }

        // Calculate the chance of success based on the dirt level
        // The chance decreases linearly from 100% at dirt level 0 to 50% at dirt level 100
        float successChance = 100 - (_dirtLevel / 2.0f);

        // Ensure the success chance is within the range [0, 100]
        successChance = Mathf.Clamp(successChance, 0, 100);

        // Generate a random number between 0 and 100
        float randomValue = Random.Range(0f, 100f);

        Debug.Log(randomValue + " " + successChance);
        // Return true if the random value is less than or equal to the success chance
        if (randomValue <= successChance)
        {
            _dirtLevel += 10;
            return true;
        }
        else
        {
            _dirtLevel += 30;
            return false;
        } 
    }
    public override void OnUse(InputAction.CallbackContext ctx)
    {
        if (InteractionManager.Instance.GetCurrentInteractable() is Cleanable _cleanable)
        {
            if (_cleanable.cleanableType == CleanableType.Wipable)
            {
                _cleanable.HandleToolUse();
            }
        }
        else if (InteractionManager.Instance.GetCurrentInteractable() is Water _water)
        {
            _water.HandleToolUse();
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
    public void Submerge()
    {
        Debug.Log("Wipe Submerged");
        _dirtLevel = 0;
    }
}
