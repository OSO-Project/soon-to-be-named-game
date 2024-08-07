using UnityEngine;

public abstract class Encounter : MonoBehaviour
{
    public float duration;

    public abstract bool CanStart();
    public abstract void StartEncounter();
    public abstract void StopEncounter();
}