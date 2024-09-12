using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTrigger : CleanTrigger
{
    protected override void TriggerActionIn(IDirtyObject dObject, Collider other)
    {
        if(!dObject.GetHidden())
        {
            GameEventManager.Instance.CleanItem(dObject.GetDirtValue());
            dObject.Hide();
        }
            
        
    }
    protected override void TriggerActionOut(IDirtyObject dObject, Collider other)
    {
        if (dObject.GetHidden())
        {
            GameEventManager.Instance.UnCleanItem(dObject.GetDirtValue());
            dObject.UnHide();
        }
            
       
    }
}
