using UnityEngine;

public class RemoveOldRoomTrigger : MonoBehaviour
{
    public RoomManager roomManager;
    private static bool isFirstInstance = true;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        if (isFirstInstance)
        {
            Destroy(gameObject);

            isFirstInstance = false;
        }
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