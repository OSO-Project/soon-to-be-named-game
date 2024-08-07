using UnityEditor;
using UnityEngine;

public class Wipe : MonoBehaviour, ITool
{
    public string Name {get; private set;} = "Wipe";
    private int _dirtLevel = 0;
    public bool isWipeSuccessful()
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
    public void Use()
    {
        Debug.Log("Wipe Used");
        _dirtLevel += 10;
    }

    public void Submerge()
    {
        Debug.Log("Wipe Submerged");
        _dirtLevel = 0;
    }
}
