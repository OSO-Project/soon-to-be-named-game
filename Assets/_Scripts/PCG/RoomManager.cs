using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomManager : MonoBehaviour
{
    [Header("Room Settings")]
    public GameObject roomPrefab;
    public Transform initSpawnPoint;
    private GameObject currentRoom;
    private GameObject nextRoom; 

    public event Action OnTransitionComplete;
    void Start()
    {
        SpawnInitialRoom();        
    }

    /// <summary>
    /// METHODS FOR SPAWNING AND REMOVING ROOMS
    /// </summary>
    void SpawnInitialRoom()
    {
        currentRoom = Instantiate(roomPrefab, new Vector3(initSpawnPoint.transform.position.x, initSpawnPoint.transform.position.y, initSpawnPoint.transform.position.z), Quaternion.identity);
    }

    public void SpawnNextRoom(Vector3 position, Quaternion rotation)
    {
        position.y += 15.0f;
        nextRoom = Instantiate(roomPrefab, position, Quaternion.identity);
        StartCoroutine(RotateTheRoom(rotation));
        Debug.Log("SpawnNextRoom");
    }

    public void RemoveOldRoom()
    {
        StartCoroutine(TransitionToNextRoom());
    }

    IEnumerator RotateTheRoom(Quaternion rotation)
    {
        yield return new WaitForSeconds(0.5f);
        Quaternion additionalRotation = Quaternion.Euler(0, 180, 0);
        Quaternion combinedRotation = rotation * additionalRotation;
        nextRoom.transform.rotation = combinedRotation;
        Vector3 newRoomPosition = nextRoom.transform.position;
        newRoomPosition.y -= 15.0f;
        nextRoom.transform.position = newRoomPosition;
    }

    IEnumerator TransitionToNextRoom()
    {
        Transform exitDoor = currentRoom.transform.Find("ExitDoor");
        exitDoor.gameObject.GetComponentInChildren<OpenCloseDoor>().CloseAndLockDoor();
        exitDoor.parent.name = "EnterDoorLocked";
        exitDoor.parent.SetParent(nextRoom.transform.Find("RoomForRandomGen"));
        yield return new WaitForSeconds(0.5f);
        //Destroy(currentRoom);
        Transform roomForRandomGen = currentRoom.transform.Find("RoomForRandomGen");
        if (roomForRandomGen != null)
        {
            Destroy(roomForRandomGen.gameObject);
        }
        Debug.Log(currentRoom.name + " Destroyed!");

        currentRoom = nextRoom;
        OnTransitionComplete?.Invoke();
    }
}
