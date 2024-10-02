using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Water : MonoBehaviour, Interactable
{
    private Canvas _progressBarCanvas = null;
    private HighlightObject _highlight;
    [SerializeField] protected float _timeToClean = 3f;
    private Wipe _wipe;
    public Image _progressBar;
    private ProgressBarFillMechanic progressBarFill;
    public static Water currentTarget = null;
    
    void Start()
    {
        ToolsManager.toolSwap += OnToolSwap;
        _wipe = FindAnyObjectByType<Wipe>();
        progressBarFill = new ProgressBarFillMechanic(_progressBar, this, _timeToClean);
    }

    void OnValidate()
    {
        if (!gameObject.CompareTag("Interactable"))
        {
            // If not, set the tag to "Interactable"
            gameObject.tag = "Interactable";
            Debug.Log("Tag 'Interactable' was not set and has been assigned.");
        }
        // Attach HighlightObject component if it does not exist
        if (_highlight == null)
        {
            _highlight = gameObject.GetComponent<HighlightObject>();
            if (_highlight == null)
            {
                _highlight = gameObject.AddComponent<HighlightObject>();
                Debug.Log("HighlightObject component was missing and has been added.");
            }
        }
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Canvas>() != null)
            {
                _progressBarCanvas = child.GetComponent<Canvas>();
            }
        }
    }

    public void OnBeginLooking()
    {
        if (ToolsManager.Instance._currentlyHeld is not Wipe || _wipe._dirtLevel == 0) return;
        ToggleObjectUI(true);
    }

    public void OnFinishLooking()
    {
        ToggleObjectUI(false);
    }

    private void ToggleObjectUI(bool state)
    {
        _highlight.SetIsHighlighted(state);
        if (state)
        {
            currentTarget = this;
        }
        else
        {
            currentTarget = null;
        }
        _progressBarCanvas.gameObject.SetActive(state);
        UIManager.Instance.HintText.gameObject.SetActive(state);
        if (!state)
        {
            progressBarFill.StopAndResetProgress();
        }
        _highlight.Highlight(Color.blue);
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        // if (currentTarget == this)
        // {
        //     if (ctx.performed)
        //     {
        //         Debug.Log("cleaning started");
        //         progressBarFill.StartProgress(OnSubmergeComplete);
        //     }
        // }
        // Interaction handled by the Wipe Tool.
    }

    public void HandleToolUse()
    {
        if (currentTarget == this)
        {
            Debug.Log("cleaning started");
            progressBarFill.StartProgress(OnSubmergeComplete);
        }
    }

    private void OnSubmergeComplete()
    {
        _wipe.Submerge();
        progressBarFill.StopAndResetProgress();
        ToggleObjectUI(false);
        Debug.Log("Wipe cleared. Dirtiness level set to " + _wipe._dirtLevel);
    }

    private void OnToolSwap()
    {
        if (ReferenceEquals(InteractionManager.Instance.GetCurrentInteractable(), this))
        {
            if (ToolsManager.Instance._currentlyHeld is not Wipe)
            {
                progressBarFill.StopAndResetProgress();
                ToggleObjectUI(false);
            }
            else
            {
                OnBeginLooking();
            }
        }
    }
}
