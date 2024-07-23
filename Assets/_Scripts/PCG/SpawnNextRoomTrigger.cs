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
            roomManager.SpawnNextRoom(nextRoomPosition.position);
            Destroy(gameObject);
        }
    }
}