using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class Wipe : Tool
{
    public override string Name => "Wipe";
    private int _dirtLevel = 0;

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
        if (_dirtLevel <= 50)
        {
            return true;
        }

        // Calculate the chance of success based on the dirt level
        // The chance decreases linearly from 100% at dirt level 0 to 50% at dirt level 100
        float successChance = 100 - (_dirtLevel / 2.0f);

        // Ensure the success chance is within the range [0, 100]
        successChance = Mathf.Clamp(successChance, 0, 100);

        // Generate a random number between 0 and 100
        float randomValue = Random.Range(0f, 100f);

        // Return true if the random value is less than or equal to the success chance
        return randomValue <= successChance;
    }
    public override void OnUse(InputAction.CallbackContext context)
    {
        Debug.Log("Wipe Used");
        if (IsWipeSuccessful())
        {
            // Perform Clean
            _dirtLevel += 10;
        }
        else
        {
            _dirtLevel += 40;
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
