using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmokeEncounter : Encounter
{
    private HashSet<ISmokable> smokablesInArea = new HashSet<ISmokable>();
    public override bool CanStart()
    {
        // Add condition to check if the encounter can start
        return true;
    }

    public override void StartEncounter()
    {
        //StartCoroutine(StartSmoke());
        List<GameObject> encounterEnders = FindObjectsOfType<MonoBehaviour>()
            .OfType<ISmokeEnder>()
            .Select(ender => (ender as MonoBehaviour).gameObject)
            .ToList();

        foreach (GameObject ender in encounterEnders)
        {
            if (!ender.GetComponent<HoldToCleanEncounter>())
            {
                ender.AddComponent<HoldToCleanEncounter>();
                ender.AddComponent<HighlightObject>();
            }
        }

        foreach (ISmokable smokable in smokablesInArea)
        {
            smokable.AddSmokeDirt();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger area implements ISmokable
        ISmokable smokable = other.GetComponent<ISmokable>();
        if (smokable != null)
        {
            smokablesInArea.Add(smokable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the object from the set when it exits the trigger area
        ISmokable smokable = other.GetComponent<ISmokable>();
        if (smokable != null)
        {
            smokablesInArea.Remove(smokable);
        }
    }

    public override void StopEncounter()
    {
        // remove components from enders
        if (encounterEnders.Count != 0)
        {
            foreach (GameObject ender in encounterEnders)
            {
                Destroy(ender.GetComponent<HoldToCleanEncounter>());
                Destroy(ender.GetComponent<HighlightObject>());
            }

            // clear the enders list
            encounterEnders.Clear();
        }

        smokablesInArea.Clear();
        return;
    }
}
