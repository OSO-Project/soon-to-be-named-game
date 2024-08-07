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

        IDisableChildren[] disableChildrenObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDisableChildren>().ToArray();
        foreach (IDisableChildren disableChildren in disableChildrenObjects)
        {
            disableChildren.DisableAllChildren();
        }
    }

    public override void StopEncounter()
    {
        throw new System.NotImplementedException();
    }
}
