using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HighlightObject : MonoBehaviour
{
    private bool _isHighlighted;
    // Start is called before the first frame update
    void Start()
    {
        _isHighlighted = false;
    }

    public void Highlight()
    {
        var outline = GetComponent<Outline>();
        if (_isHighlighted)
        {
            if (outline == null)
            {
                outline = gameObject.AddComponent<Outline>();
                outline.OutlineColor = Color.white;
                outline.OutlineWidth = 5.0f;
            }
            outline.enabled = true;
        }
        else
        {
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
    }

    public void SetIsHighlighted(bool newVal)
    {
        _isHighlighted = newVal;
    }
}
