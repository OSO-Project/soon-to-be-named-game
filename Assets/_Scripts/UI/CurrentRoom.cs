using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoom : MonoBehaviour
{
    public float currentRoom = 1;
    void Start()
    {
        currentRoom = 1;
    }
    public void MoveToNextRoom()
    {
        currentRoom++;
        UIManager.Instance.CurrentRoom.text = "Current Room: " + currentRoom;
    }
}
