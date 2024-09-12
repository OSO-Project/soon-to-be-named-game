using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger in");
        if (other.TryGetComponent(out IDirtyObject dObject))
        {
            Debug.Log("dirty trigger in");
            TriggerActionIn(dObject, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("trigger out");
        if (other.TryGetComponent(out IDirtyObject dObject))
        {
            Debug.Log("dirty trigger out");
            TriggerActionOut(dObject, other);
        }
    }

    protected virtual void TriggerActionIn(IDirtyObject dObject, Collider other) { }
    protected virtual void TriggerActionOut(IDirtyObject dObject, Collider other) { }
}
