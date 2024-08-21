using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShortCircuitEncounter : Encounter
{
    public override bool CanStart()
    {
        // Add condition to check if the encounter can start
        return true;
    }

    public override void StartEncounter()
    {
        Debug.Log("ShortCircuitEncounter started!");

        // maybe move elsewhere for performance
        List<GameObject> encounterEnders = FindObjectsOfType<MonoBehaviour>()
            .OfType<IShortCircuitEnder>()
            .Select(ender => (ender as MonoBehaviour).gameObject)
            .ToList();

        // move declaration to Encounter
        // add components to enders
        foreach (GameObject ender in encounterEnders)
        {
            if (!ender.GetComponent<HoldToCleanEncounter>())
            {
                ender.AddComponent<HoldToCleanEncounter>();
                ender.AddComponent<HighlightObject>();
            }
        }

        IDisableChildren[] disableChildrenObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDisableChildren>().ToArray();
        foreach (IDisableChildren disableChildren in disableChildrenObjects)
        {
            disableChildren.DisableAllChildren();
        }
    }

    public override void StopEncounter()
    {
        Debug.Log($"{gameObject.name} is stopped");

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

        IDisableChildren[] disableChildrenObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDisableChildren>().ToArray();
        foreach (IDisableChildren disableChildren in disableChildrenObjects)
        {
            disableChildren.EnableAllChildren();
        }

        return;
    }
}
