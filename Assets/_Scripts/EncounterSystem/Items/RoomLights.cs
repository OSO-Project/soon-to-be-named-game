using UnityEngine;

public class RoomLights : MonoBehaviour, IDisableChildren
{

    private void Start()
    {
        GameEventManager.Instance.OnLightsControlClick += EnableAllChildren;
    }

    public void DisableAllChildren()
    {
        Debug.Log("xdd light");
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