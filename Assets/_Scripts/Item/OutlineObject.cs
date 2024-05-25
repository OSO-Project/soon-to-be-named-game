using UnityEngine;

public class OutlineObject : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;

    void Update()
    {
        // Cast a ray from the camera in the direction the player is facing
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Check for collisions with objects
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object is different from the selection
            if (hit.transform != selection)
            {
                // Remove outline from the previous selection
                if (selection != null)
                {
                    var outline = selection.GetComponent<Outline>();
                    if (outline != null)
                        outline.enabled = false;
                }

                // Update the selection to the new hit object
                selection = hit.transform;

                // Add outline to the new selection
                var newOutline = selection.GetComponent<Outline>();
                if (newOutline == null)
                    newOutline = selection.gameObject.AddComponent<Outline>();

                newOutline.enabled = true;
                newOutline.OutlineColor = Color.red;
                newOutline.OutlineWidth = 5.0f;
            }

            // Set the highlighted object to the hit object
            highlight = hit.transform;
        }
        else
        {
            // If no object is hit, remove outline from the previous selection
            if (selection != null)
            {
                var outline = selection.GetComponent<Outline>();
                if (outline != null)
                    outline.enabled = false;
            }

            // Clear the highlight
            highlight = null;
        }

        // Handle selection
        if (Input.GetMouseButtonDown(0))
        {
            // Check if there is a highlighted object
            if (highlight != null)
            {
                // Remove outline from the previous selection
                if (selection != null)
                {
                    var outline = selection.GetComponent<Outline>();
                    if (outline != null)
                        outline.enabled = false;
                }

                // Update the selection to the highlighted object
                selection = highlight;

                // Add outline to the new selection
                var newOutline = selection.GetComponent<Outline>();
                if (newOutline == null)
                    newOutline = selection.gameObject.AddComponent<Outline>();

                newOutline.enabled = true;
                newOutline.OutlineColor = Color.magenta;
                newOutline.OutlineWidth = 7.0f;
            }
            else
            {
                // If no object is highlighted, clear the selection
                if (selection != null)
                {
                    var outline = selection.GetComponent<Outline>();
                    if (outline != null)
                        outline.enabled = false;
                    selection = null;
                }
            }
        }
    }
}
