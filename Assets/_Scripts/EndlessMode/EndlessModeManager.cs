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
    public List<DirtyObject> dirtyObjects; // List of all dirty objects in the room
    public List<DirtyObject> dirtyObjectsPrev; // List of all dirty objects in the room
    [SerializeField] private float totalDirt; // Total dirt in the room
    public float currentDirt; // Current amount of dirt left in the room
    public float cleanliness; // Cleanliness percentage of the room

    public event Action OnRoomFinished;
    // Start is called before the first frame update
    void Start()
    {
        // subscribe updating cleanliness to OnAddScore 
        GameEventManager.Instance.OnAddScore += UpdateCleanliness;

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
        if (possibleRooms.Length == 0)
        {
            Debug.LogError("No possible rooms available.");
            return;
        }

        // Randomly select a room from the possibleRooms array
        int randomIndex = UnityEngine.Random.Range(0, possibleRooms.Length);
        GameObject selectedRoom = possibleRooms[randomIndex];

        // Set the selected room as the roomPrefab for RoomManager
        roomManager.roomPrefab = selectedRoom;
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
        dirtyObjects = dirtyObjectsPrev;

        // calculate dirtiness
        foreach (DirtyObject obj in dirtyObjects)
        {
            totalDirt += obj.getDirtValue();
        }
        currentDirt = totalDirt;
        cleanliness = 0f; // Initially, the room is 0% clean
    }
    // Save dirty objects from newly spawned room
    public void SetNewRoomDirt(List<DirtyObject> dObj)
    {
        dirtyObjectsPrev = dObj;
/*        Debug.Log($"drt: {dirtyObjects == null}");
        foreach (var item in dirtyObjectsPrev)
        {
            Debug.Log($"{item}");
        }*/
        // if first room then initialize dirt for it
        if (dirtyObjects == null)
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
