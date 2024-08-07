using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLights : MonoBehaviour, IDisableChildren
{

    private void Start()
    {
        GameEventManager.Instance.OnLightsControlClick += EnableAllChildren;
    }

    public void DisableAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void EnableAllChildren()
    {
        Debug.Log("turn on lights");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
