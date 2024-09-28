using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindEncounter : Encounter
{
    [SerializeField] private List<Transform> windowTransforms;   // The position of the open window
    [SerializeField] private float windForce = 100f;      // Force of the wind
    [SerializeField] private float pushInterval = 5f;    // Interval to push objects (in seconds)
    //[SerializeField] private Vector3 pushDirection = Vector3.forward;  // Direction to push items away from the window
    [SerializeField] private List<Collider> windAreas;           // The area of effect for the wind
    [SerializeField] private HashSet<Collider> windAreasHash = new HashSet<Collider>();

    private Vector3 temp;
    private Vector3 temp2;

    private List<OpenCloseWindow> windows;
    private GameObject currentRoom;

    private HashSet<IPhysics> pushableObjectsInRoom = new HashSet<IPhysics>();
    [SerializeField] private List<GameObject> obj = new List<GameObject>();
    private Coroutine windCoroutine;


    private void Start()
    {
        GameEventManager.Instance.OnWindowClose += StopEncounter;
        GameManager.Instance.OnNextRoom += NewCurrentRoom;
        GameEventManager.Instance.OnWindowOpen += AddWindows;
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
        AddWindows();
        pushableObjectsInRoom = currentRoom.GetComponentsInChildren<IPhysics>().ToHashSet();
        obj = currentRoom
            .GetComponentsInChildren<IPhysics>()
            .Select(physicsObject => ((MonoBehaviour)physicsObject).gameObject) 
            .ToList();

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
        pushableObjectsInRoom.Clear();
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
        foreach (var pushable in pushableObjectsInRoom)
        {
            Debug.Log($"push: {pushable}");
            Debug.Log($"obj1: {pushable.Rigidbody == null}");
            foreach (var windArea in windAreasHash)
            {
                // Check if the pushable object is within the specific wind area
                Debug.Log($"obj: {pushable.Rigidbody.position}");
                if (windArea.bounds.Contains(pushable.Rigidbody.position))
                {
                    Debug.Log("push");
                    obj.Add(pushable.Rigidbody.gameObject);
                    //Vector3 forceDirection = (windowTransform.position - pushable.Rigidbody.transform.position).normalized; // Direction towards the window
                    //Vector3 windowTransform = windowTransforms[windAreas.IndexOf(windArea)].position;
                    Vector3 windowForward = windArea.transform.forward;
                    //Vector3 forceDirection = (windowTransform - pushable.Rigidbody.position).normalized;
                    Vector3 forceDirection = windowForward + Vector3.up.normalized * 0.1f;
                    temp = forceDirection;
                    temp2 = pushable.Rigidbody.position;
                    pushable.Rigidbody.AddForce(forceDirection * windForce, ForceMode.Impulse);
                }
            }

        }
    }

    private void NewCurrentRoom(GameObject cr)
    {
        currentRoom = cr;
    }

    private void AddWindows()
    {
        foreach (var window in windows)
        {
            if (window.isOpen)
            {
                windowTransforms.Add(window.transform);
                Collider col = window.GetWindHitbox().GetComponent<Collider>();
                if (col != null)
                {
                    //windAreas.Add(col);
                    windAreasHash.Add(col);
                    
                    Debug.Log("added col");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        foreach (var area in windAreas)
        {
            //Gizmos.matrix = area.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(area.bounds.center, area.bounds.size);
            //Gizmos.DrawLine(area.transform.position, Vector3.forward);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(temp2, temp2 + temp * 2f);
    }
}
