using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;
        GameEventManager.Instance.OnEarthquakeEncounterEnd += StartFalling;
    }

    void OnDestroy()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnEarthquakeEncounterEnd -= StartFalling;
        }
    }

    // redesign to earthquake start?
    private void StartFalling()
    {
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals("Ground"))
        {
            Debug.Log("xdd");
            rb.isKinematic = true;
        }
    }

}
