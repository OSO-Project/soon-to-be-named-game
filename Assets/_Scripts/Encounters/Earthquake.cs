using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : Encounter
{
    protected override void EncounterStart()
    {
        Debug.Log("Earthquake start!");
        GameEventManager.Instance.HandleEarthquakeEncounterStart(duration);
        base.EncounterStart();
    }

    protected override IEnumerator EncounterPlay()
    {
        Debug.Log("Earthquake");
        return base.EncounterPlay();
    }

    protected override void EncounterEnd()
    {
        Debug.Log("Earthquake end!");
        GameEventManager.Instance.HandleEarthquakeEncounterEnd();
        base.EncounterEnd();
    }
}
