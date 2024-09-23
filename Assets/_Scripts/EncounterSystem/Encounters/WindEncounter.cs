using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindEncounter : Encounter
{
    [SerializeField] private List<Transform> windowTransforms;   // The position of the open window
    [SerializeField] private float windForce = 10f;      // Force of the wind
    [SerializeField] private float pushInterval = 5f;    // Interval to push objects (in seconds)
    [SerializeField] private Vector3 pushDirection = Vector3.forward;  // Direction to push items away from the window
    [SerializeField] private List<Collider> windAreas;           // The area of effect for the wind

    private List<OpenCloseWindow> windows;
    private GameObject currentRoom;

    private HashSet<IPhysics> pushableObjectsInArea = new HashSet<IPhysics>();
    private Coroutine windCoroutine;


    private void Start()
    {
        GameEventManager.Instance.OnWindowClose += StopEncounter;
        GameManager.Instance.OnNextRoom += NewCurrentRoom;
    }

    public override bool CanStart()
    {
        windows = currentRoom.GetComponentsInChildren<OpenCloseWindow>().ToList();
        //Debug.Log($"windows: {windows.Count}");
        foreach (var window in windows)
        {
            if (window.isOpen)
            {
                //Debug.Log($"windows yes");
                return true;
            }
        }
        return false;
    }

    public override void StartEncounter()
    {
        //Debug.Log("Wind Encounter started!");
        foreach (var window in windows)
        {
            if (window.isOpen)
            {
                windowTransforms.Add(window.transform);
                Collider col = window.transform.Find("WindHitbox").GetComponent<Collider>();
                if (col != null)
                {
                    windAreas.Add(col);
                    //Debug.Log("added col");
                }
            }
        }

        // Start the coroutine to push objects every few seconds
        windCoroutine = StartCoroutine(PushObjectsPeriodically());
    }

    public override void StopEncounter()
    {
        //Debug.Log("Wind Encounter stopped!");

        // Stop the coroutine if it's running
        if (windCoroutine != null)
        {
            StopCoroutine(windCoroutine);
            windCoroutine = null;
        }

        // Clear all pushable objects
        pushableObjectsInArea.Clear();
    }

    private IEnumerator PushObjectsPeriodically()
    {
        while (true)
        {
            PushObjects();
            yield return new WaitForSeconds(pushInterval);

        }
        

    }

    private void PushObjects()
    {
        foreach (var pushable in pushableObjectsInArea)
        {
            foreach (var windArea in windAreas)
            {
                // Check if the pushable object is within the specific wind area
                if (windArea.bounds.Contains(pushable.Rigidbody.transform.position))
                {
                    Debug.Log("push");
                    //Vector3 forceDirection = (windowTransform.position - pushable.Rigidbody.transform.position).normalized; // Direction towards the window
                    Vector3 windowTransform = windowTransforms[windAreas.IndexOf(windArea)].position;
                    Vector3 forceDirection = (windowTransform - pushable.Rigidbody.transform.position).normalized; // Direction towards the window
                    pushable.Rigidbody.AddForce(forceDirection * windForce, ForceMode.Impulse);
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the area implements IPushable
        IPhysics pushable = other.GetComponent<IPhysics>();
        if (pushable != null)
        {
            Debug.Log("entered");
            pushableObjectsInArea.Add(pushable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the object from the list when it leaves the area
        IPhysics pushable = other.GetComponent<IPhysics>();
        if (pushable != null)
        {
            pushableObjectsInArea.Remove(pushable);
        }
    }

    private void NewCurrentRoom(GameObject cr)
    {
        currentRoom = cr;
    }
}
