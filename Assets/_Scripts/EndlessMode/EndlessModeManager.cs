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

    public event Action OnRoomFinished;
    // Start is called before the first frame update
    void Start()
    {
        SelectNextRoomToSpawn();
        objectsSpawnMultiplier = 0.1f;
        roomManager.OnTransitionComplete += RoomFinished;
        OnRoomFinished += roomCount.MoveToNextRoom;
        OnRoomFinished += IncreaseMultiplier;
        OnRoomFinished += SelectNextRoomToSpawn;
        OnRoomFinished += () => gameTimer.AddTime(60);
        StartCoroutine(TeleportPlayerOnStartPoint());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RoomFinished()
    {
        Debug.Log("RoomFinished");
        OnRoomFinished?.Invoke();
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
}
