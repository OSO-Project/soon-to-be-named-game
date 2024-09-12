using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessModeManager : MonoBehaviour
{
    public GameObject[] possibleRooms;
    public CurrentRoom roomCount;
    public GameTimer gameTimer;
    public RoomManager roomManager;
    public float objectsSpawnMultiplier;

    // Tracking dirtiness
    [Header("Dirt and cleaning Room Settings")]
    public List<IDirtyObject> IDirtyObjects; // List of all dirty objects in the room
    public List<IDirtyObject> IDirtyObjectsPrev; // List of all dirty objects in the room
    [SerializeField] private float totalDirt; // Total dirt in the room
    public float currentDirt; // Current amount of dirt left in the room
    public float cleanliness; // Cleanliness percentage of the room

    public event Action OnRoomFinished;

    public List<GameObject> availableRooms = new List<GameObject>();
    public GameObject lastRoom;
    void Start()
    {
        // subscribe updating cleanliness to OnAddScore 
        GameEventManager.Instance.OnAddScore += UpdateCleanliness;

        // Initialize available rooms
        availableRooms.AddRange(possibleRooms);

        SelectNextRoomToSpawn();
        objectsSpawnMultiplier = 0.1f;
        roomManager.OnTransitionComplete += RoomFinished;
        OnRoomFinished += roomCount.MoveToNextRoom;
        OnRoomFinished += IncreaseMultiplier;
        OnRoomFinished += SelectNextRoomToSpawn;
        OnRoomFinished += () => gameTimer.AddTime(60);
        StartCoroutine(TeleportPlayerOnStartPoint());
    }
    private void RoomFinished()
    {
        Debug.Log("RoomFinished");
        // Initialize dirt for new room
        InitializeRoomDirt();
        //OnRoomFinished?.Invoke();
    }
    public void IncreaseMultiplier()
    {
        objectsSpawnMultiplier += 0.1f;
    }

    private void SelectNextRoomToSpawn()
    {
        if (availableRooms.Count == 0)
        {
            // Reset the list of available rooms if all rooms have been used
            availableRooms.AddRange(possibleRooms);
        }

        // Randomly select a room that is not the same as the last room
        GameObject selectedRoom;
        do
        {
            int randomIndex = UnityEngine.Random.Range(0, availableRooms.Count);
            selectedRoom = availableRooms[randomIndex];
        } while (selectedRoom == lastRoom);

        // Remove the selected room from the available list
        availableRooms.Remove(selectedRoom);

        // Set the selected room as the roomPrefab for RoomManager
        roomManager.roomPrefab = selectedRoom;

        // Store the last selected room
        lastRoom = selectedRoom;
    }

    IEnumerator TeleportPlayerOnStartPoint()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.Find("PlayerSpawnPoint").transform.position;
    }

    private void InitializeRoomDirt()
    {
        totalDirt = 0f;

        // set saved dirty objects as current
        IDirtyObjects = IDirtyObjectsPrev;

        // calculate dirtiness
        foreach (IDirtyObject obj in IDirtyObjects)
        {
            totalDirt += obj.GetDirtValue();
        }
        currentDirt = totalDirt;
        cleanliness = 0f; // Initially, the room is 0% clean
    }
    // Save dirty objects from newly spawned room
    public void SetNewRoomDirt(List<IDirtyObject> dObj)
    {
        IDirtyObjectsPrev = dObj;
/*        Debug.Log($"drt: {IDirtyObjects == null}");
        foreach (var item in IDirtyObjectsPrev)
        {
            Debug.Log($"{item}");
        }*/
        // if first room then initialize dirt for it
        if (IDirtyObjects == null)
        {
            InitializeRoomDirt();
        }
    }

    // Update the cleanliness after cleaning or getting dirty
    public void UpdateCleanliness(int dirtRemoved)
    {
        currentDirt -= dirtRemoved;
        cleanliness = 1f - (currentDirt / totalDirt);
        cleanliness = Mathf.Clamp(cleanliness, 0f, 1f); // Ensure cleanliness is between 0 and 1
    }
}
