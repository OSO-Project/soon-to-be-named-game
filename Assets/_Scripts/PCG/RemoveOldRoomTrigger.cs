using UnityEngine;

public class RemoveOldRoomTrigger : MonoBehaviour
{
    public RoomManager roomManager;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            roomManager.RemoveOldRoom();
            Destroy(gameObject);
        }
    }
}