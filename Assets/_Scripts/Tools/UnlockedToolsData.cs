using UnityEngine;

[CreateAssetMenu(fileName = "New UnlockedToolsData", menuName = "UnlockedToolsData")]
public class UnlockedToolsData : ScriptableObject
{
    // use enum later on for the string, use ITool even further
    public string currentlyHeld;
    public bool wipeUnlocked;
    public bool vaccuumUnlocked;
    public bool bucketUnlocked;
}