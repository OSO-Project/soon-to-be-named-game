using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TrashBag : Tool
{
    public override string Name => "TrashBag";
    public Transform bagTransform; // Reference to the bag's transform
    public Transform spawnTrashPoint;
    public float bagGrowthFactor = 0.1f; // How much the bag grows per object
    public int maxCapacity = 10; // Maximum number of objects the bag can hold
    public float throwForce = 300f; // Force with which objects are thrown out

    // To store original objects instead of a single prefab
    public List<GameObject> storedObjects = new List<GameObject>();
    private Vector3 originalScale;

    void Start()
    {
        originalScale = bagTransform.localScale; // Store the original scale of the bag
        InputManager.Instance.UseToolAction.performed += OnUse;
    }
    private void OnDestroy()
    {
        InputManager.Instance.UseToolAction.performed -= OnUse;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button to suck objects
        {
            TrySuckObject();
        }
    }

    void TrySuckObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Suckable") && storedObjects.Count < maxCapacity)
            {
                GameObject obj = hit.collider.gameObject;

                // Instantiate a copy of the original object to store in the list
                GameObject storedCopy = Instantiate(obj);
                storedCopy.SetActive(false); // Hide the copy
                storedObjects.Add(storedCopy);

                Destroy(obj); // Destroy the original object in the scene

                // Increase the size of the bag
                bagTransform.localScale += Vector3.one * bagGrowthFactor;
            }
        }
    }

    void EmptyBag()
    {
        if (storedObjects.Count > 0)
        {
            foreach (GameObject storedObj in storedObjects)
            {
                if (storedObj != null)
                {
                    storedObj.SetActive(true); // Reactivate the stored copy
                    storedObj.transform.position = spawnTrashPoint.position;
                    storedObj.transform.rotation = Quaternion.identity;

                    Rigidbody rb = storedObj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(spawnTrashPoint.up * throwForce); // Throw objects out of the bag
                    }
                }
            }

            storedObjects.Clear();
            bagTransform.localScale = originalScale; // Reset the size of the bag
        }
    }

    public override void OnUse(InputAction.CallbackContext context)
    {
        EmptyBag();
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