using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyObject : MonoBehaviour
{

    public float dirtValue; // How much dirt this object contributes
    public bool isClean; // Is the object clean or dirty?


    private RoomManager roomManager;
    // Start is called before the first frame update
    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    public void CleanObject()
    {
        if (!isClean)
        {
            isClean = true;
            roomManager.UpdateCleanliness(dirtValue);
        }
    }

    public void MakeDirty()
    {
        if (isClean)
        {
            isClean = false;
            roomManager.MakeObjectDirty(this);
        }
    }

}
