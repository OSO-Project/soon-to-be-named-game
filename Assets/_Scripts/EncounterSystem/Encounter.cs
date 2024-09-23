using System.Collections.Generic;
using UnityEngine;

public abstract class Encounter : MonoBehaviour
{
    public float duration;
    public Sprite encounterIcon;
    public string encounterText;
    //public List<GameObject> encounterEnders;
    public abstract bool CanStart();
    public abstract void StartEncounter();
    public abstract void StopEncounter();
    public virtual void StopEncounter(bool check)
    {
    }
}