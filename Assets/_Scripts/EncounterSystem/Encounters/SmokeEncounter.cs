using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmokeEncounter : Encounter
{
    private HashSet<ISmokable> smokablesInArea = new HashSet<ISmokable>();
    [SerializeField] private ParticleSystem smokeParticleSystem;
    public override bool CanStart()
    {
        // Add condition to check if the encounter can start
        return true;
    }

    public override void StartEncounter()
    {
        smokeParticleSystem = GameObject.Find("SmokeSpawner").GetComponentInChildren<ParticleSystem>();
        StartSmoke();
        List<GameObject> encounterEnders = FindObjectsOfType<MonoBehaviour>()
            .OfType<ISmokeEnder>()
            .Select(ender => (ender as MonoBehaviour).gameObject)
            .ToList();

        foreach (GameObject ender in encounterEnders)
        {
/*            if (!ender.GetComponent<HoldToCleanEncounter>())
            if (!ender.GetComponent<CleanItemEncounter>())
            {
                ender.AddComponent<CleanItemEncounter>();
                ender.AddComponent<HighlightObject>();
            }*/
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
        StopSmoke();
        // remove components from enders
        if (encounterEnders.Count != 0)
        {
            foreach (GameObject ender in encounterEnders)
            {
                Destroy(ender.GetComponent<CleanItemEncounter>());
                Destroy(ender.GetComponent<HighlightObject>());
            }

            // clear the enders list
            encounterEnders.Clear();
        }

        smokablesInArea.Clear();
        return;
    }

    private void StartSmoke()
    {
        smokeParticleSystem?.Play();
    }

    private void StopSmoke()
    {
        smokeParticleSystem?.Stop();
    }
}
