using UnityEngine;

public class RoomExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //GameManager.Instance.ProceedToNextRoom();
        }
    }
}