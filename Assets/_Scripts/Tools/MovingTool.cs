using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovingTool : MonoBehaviour, ITool
{
    public string Name { get; private set; } = "MovingTool";
    public GameObject toolPrefab;
    public Transform holdPosition; // The position where the tool is held by the player
    public LayerMask groundLayer; // The layer representing the ground
    public LayerMask obstacleLayer; // The layer representing obstacles
    public float maxPlacementDistance = 5f; // Maximum distance from the player where the tool can be placed
    public TMP_Text messageText; // UI Text element to display messages
    private GameObject currentTool; // The tool currently held by the player
    public bool isHoldingTool = true;

    private MeshCollider toolCollider; // Reference to the MeshCollider component

    void Start()
    {
        // Instantiate the tool and set it as a child of the hold position
        currentTool = Instantiate(toolPrefab, holdPosition.position, holdPosition.rotation);
        currentTool.transform.SetParent(holdPosition);
        toolCollider = currentTool.GetComponentInChildren<MeshCollider>();
        if (toolCollider != null)
        {
            toolCollider.enabled = false;
        }
    }

    void Update()
    {
        Use();
    }

    void TryPlaceTool()
    {
        // Perform a raycast to check for a valid placement point
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxPlacementDistance, groundLayer))
        {
            // Check if there's enough space to place the tool
            if (!Physics.CheckBox(hit.point, currentTool.transform.localScale / 2, Quaternion.identity, obstacleLayer))
            {
                PlaceTool(hit.point);
            }
            else
            {
                ShowMessage("Cannot place here. Obstacle in the way!");
            }
        }
        else
        {
            ShowMessage("Too far from the ground to place the tool!");
        }
    }

    void PlaceTool(Vector3 placementPosition)
    {
        // Detach the tool from the player and place it on the ground
        currentTool.transform.SetParent(null);
        currentTool.transform.position = placementPosition;
        currentTool.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

        if (toolCollider != null)
        {
            toolCollider.enabled = true;
        }

        // Make the tool static
        Rigidbody rb = currentTool.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        isHoldingTool = false;
        ShowMessage("Tool placed successfully.");
    }

    void PickUpTool()
    {
        // Raycast to find any object in front of the player
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxPlacementDistance))
        {
            // Check if the hit object is a child of the current tool
            if (hit.collider != null && hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject == currentTool)
            {
                // Pick up the tool (which is the parent of the hit object)
                currentTool.transform.SetParent(holdPosition);
                currentTool.transform.localPosition = Vector3.zero;
                currentTool.transform.localRotation = Quaternion.identity;

                // Disable the MeshCollider to prevent unwanted collisions
                toolCollider = currentTool.GetComponentInChildren<MeshCollider>();
                if (toolCollider != null)
                {
                    toolCollider.enabled = false;
                }

                // Make the tool movable again
                Rigidbody rb = currentTool.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }

                isHoldingTool = true;
                ShowMessage("Tool picked up.");
            }
            else
            {
                ShowMessage("No tool to pick up.");
            }
        }
        else
        {
            ShowMessage("No object detected in front of you.");
        }
    }

    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            CancelInvoke("ClearMessage");
            Invoke("ClearMessage", 2f); // Clear the message after 2 seconds
        }
    }

    void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void Use()
    {
        if (isHoldingTool)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                TryPlaceTool();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                PickUpTool();
            }
        }
    }
}
