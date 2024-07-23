using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject roomPrefab;
    public Transform initSpawnPoint;
    private GameObject currentRoom;
    private GameObject nextRoom;

    void Start()
    {
        SpawnInitialRoom();
    }

    void SpawnInitialRoom()
    {
        currentRoom = Instantiate(roomPrefab, new Vector3(initSpawnPoint.transform.position.x, initSpawnPoint.transform.position.y, initSpawnPoint.transform.position.z), Quaternion.identity);
    }

    public void SpawnNextRoom(Vector3 position)
    {
        nextRoom = Instantiate(roomPrefab, position, Quaternion.identity);
        Debug.Log("SpawnNextRoom");
    }

    public void RemoveOldRoom()
    {
        StartCoroutine(TransitionToNextRoom());
    }

    IEnumerator TransitionToNextRoom()
    {
        yield return new WaitForSeconds(2);
        Destroy(currentRoom);
        currentRoom = nextRoom;
    }
}
