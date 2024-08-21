using UnityEngine;

public class SpawnNextRoomTrigger : MonoBehaviour
{
    public RoomManager roomManager;
    public Transform nextRoomPosition;


    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            Debug.Log("Player is inside of trigger area");
            //Debug.Log("Global Rotation: " + nextRoomPosition.rotation.eulerAngles);
            Vector3 spawnPosition = nextRoomPosition.position;
            roomManager.SpawnNextRoom(spawnPosition, nextRoomPosition.rotation);
            Destroy(gameObject);
        }
    }
}