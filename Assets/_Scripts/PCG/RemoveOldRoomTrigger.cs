using UnityEngine;

public class RemoveOldRoomTrigger : MonoBehaviour
{
    public RoomManager roomManager;
    private static bool isFirstInstance = true;
    private bool hasTriggered = false;

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
        if (hasTriggered) return;

        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            roomManager.RemoveOldRoom();
            Debug.Log("Trigger for destroying");
            // Trigger new room (score)
            GameManager.Instance.ProceedToNextRoom();
            Destroy(gameObject);

            hasTriggered = true;
        }
    }
}