using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Dirt and cleaning Room Settings")]
    public List<DirtyObject> dirtyObjects; // List of all dirty objects in the room
    private float totalDirt; // Total dirt in the room
    private float currentDirt; // Current amount of dirt left in the room
    public float cleanliness; // Cleanliness percentage of the room

    public event Action OnTransitionComplete;
    void Start()
    {
        SpawnInitialRoom();
        
    }


    void InitializeRoom()
    {
        totalDirt = 0f;
        foreach (DirtyObject obj in dirtyObjects)
        {
            totalDirt += obj.dirtValue;
        }
        currentDirt = totalDirt;
        cleanliness = 0f; // Initially, the room is 0% clean
    }

    // Update the cleanliness after cleaning or getting dirty
    public void UpdateCleanliness(float dirtRemoved)
    {
        currentDirt -= dirtRemoved;
        cleanliness = 1f - (currentDirt / totalDirt);
        cleanliness = Mathf.Clamp(cleanliness, 0f, 1f); // Ensure cleanliness is between 0 and 1
    }

    // Example function to make an object dirty again
    public void MakeObjectDirty(DirtyObject obj)
    {
        currentDirt += obj.dirtValue;
        cleanliness = 1f - (currentDirt / totalDirt);
        cleanliness = Mathf.Clamp(cleanliness, 0f, 1f);
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
        if (roomPrefab.name != "Room1-61") position.y += 15.0f;
        nextRoom = Instantiate(roomPrefab, position, Quaternion.identity);
        if (roomPrefab.name != "Room1-61") StartCoroutine(RotateTheRoom(rotation));
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
        yield return new WaitForSeconds(1);
        //Destroy(currentRoom);
        Transform roomForRandomGen = currentRoom.transform.Find("RoomForRandomGen");
        if (roomForRandomGen != null)
        {
            Destroy(roomForRandomGen.gameObject);
        }
        Transform exitDoor = currentRoom.transform.Find("ExitDoor");
        exitDoor.gameObject.GetComponentInChildren<OpenCloseDoor>().CloseAndLockDoor();
        exitDoor.parent.name = "EnterDoorLocked";
        exitDoor.parent.SetParent(nextRoom.transform.Find("RoomForRandomGen"));
        currentRoom = nextRoom;
        OnTransitionComplete?.Invoke();
    }
}
