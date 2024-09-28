using System.Collections.Generic;
using UnityEngine;

public abstract class Encounter : MonoBehaviour
{
    public float duration;
    public Sprite encounterIcon;
    public string encounterText;
    public bool isActive = false;
    //public List<GameObject> encounterEnders;
    public abstract bool CanStart();
    public abstract void StartEncounter();
    public abstract void StopEncounter();
}