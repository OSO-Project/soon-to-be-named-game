using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

public class MagicVacuumCleaner : Tool
{
    public override string Name => "MagicVacuumCleaner";

    public Transform tubeEnd;  // The point where objects will be sucked into
    public float suckRange = 10f;  // The range of the vacuum cleaner
    public float moveSpeed = 5f;  // The speed at which objects move towards the tube
    public float rotationSpeed = 1f;  // The speed at which objects rotate
    public int maxItems = 5;  // Maximum number of items that can be held
    public float orbitRadius = 0.5f;  // Radius of the orbit around the tube end
    public float orbitSpeed = 50f;  // Speed of orbiting around the tube end
    public float throwForce = 500f;  // The force with which objects are thrown
    public bool isVacuumOn = false;  // Toggle the vacuum on and off

    public Transform playerCamera;

    private List<Rigidbody> suckedObjects = new List<Rigidbody>();

    private void Start()
    {
        playerCamera = FindObjectOfType<Camera>().transform;
        isVacuumOn = false;
        InputManager.Instance.UseToolAction.performed += OnUse;
    }

    private void OnDestroy()
    {
        InputManager.Instance.UseToolAction.performed -= OnUse;
    }

    void Update()
{
    if (isVacuumOn)
    {
        SuckObjects();
        MoveAndRotateObjects();
    }

    if (Input.GetKeyDown(KeyCode.C))  // 'C' for throwing objects
    {
        ThrowObjects();
    }
}

    void SuckObjects()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(tubeEnd.position, suckRange);
        foreach (Collider col in objectsInRange)
        {
            if (col.CompareTag("Suckable") && suckedObjects.Count < maxItems)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null && !suckedObjects.Contains(rb))
                {
                    suckedObjects.Add(rb);
                    rb.useGravity = false;
                }
            }
        }
    }

    void MoveAndRotateObjects()
    {
        // Iterate through the suckedObjects list in reverse to safely remove null references
        for (int i = suckedObjects.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = suckedObjects[i];

            if (rb == null)
            {
                // Remove the object from the list if it has been destroyed
                suckedObjects.RemoveAt(i);
                continue;
            }

            // Calculate the target position in a circular pattern around the tube end
            float angle = Time.time * orbitSpeed + (i * Mathf.PI * 2 / suckedObjects.Count);
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * orbitRadius;
            Vector3 targetPosition = tubeEnd.position + offset;

            // Move the object smoothly towards the target position
            rb.transform.position = Vector3.Lerp(rb.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Smoothly rotate the object towards the tube end to simulate the pull of the black hole
            Quaternion targetRotation = Quaternion.LookRotation(tubeEnd.position - rb.transform.position);
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void ThrowObjects()
    {
        isVacuumOn = false;

        Vector3 throwDirection = playerCamera.forward;  // Use the player's camera forward direction

        foreach (Rigidbody rb in suckedObjects)
        {
            if (rb != null)
            {
                rb.useGravity = true;
                rb.velocity = Vector3.zero;  // Reset velocity before applying force
                rb.AddForce(throwDirection * throwForce);  // Apply force in the player's aiming direction
            }
        }

        // Clear the list after throwing the objects
        suckedObjects.Clear();

        // Optionally, you can re-enable the vacuum after a delay or based on player input
        Invoke("EnableVacuum", 0.5f);  // Example: Re-enable after 0.5 seconds
    }

    // Optional: Method to re-enable the vacuum after a delay
    void EnableVacuum()
    {
        isVacuumOn = true;
    }

    void DropObjects()
    {
        foreach (Rigidbody rb in suckedObjects)
        {
            if (rb != null)
            {
                rb.useGravity = true;
                rb.velocity = Vector3.zero;  // Stop any residual movement
            }
        }
        suckedObjects.Clear();
    }

    public override void OnUse(InputAction.CallbackContext context)
    {
        isVacuumOn = !isVacuumOn;
        if (!isVacuumOn)
        {
            DropObjects();
        }
    }

    public override void Equip()
    {
        return;
    }
    public override void UnEquip()
    {
        return;
    }
}
