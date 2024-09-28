using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRoomDoor : MonoBehaviour
{
    public GameObject door;

    private static bool doorShown = false;

    void Start()
    {
        if (!doorShown)
        {
            ShowDoor();
            doorShown = true;
        }
        else
        {
            HideDoor();
        }
    }

    void ShowDoor()
    {
        door.SetActive(true);
    }

    void HideDoor()
    {
        door.SetActive(false);
    }
}
