using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashTrigger : CleanTrigger
{
    protected override void TriggerActionIn(IDirtyObject dObject, Collider other)
    {
        GameEventManager.Instance.CleanItem(dObject.GetDirtValue());
        Destroy(other);
    }
}
