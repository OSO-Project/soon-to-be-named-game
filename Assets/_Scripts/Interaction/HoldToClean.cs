using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToClean : MonoBehaviour, Interactable
{
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private float timeToClean = 3f;

    private static Image progressBar;
    private static TMP_Text hintText;
    private Renderer _objectRenderer;
    private static HoldToClean currentTarget = null;
    private float _lookDuration = 0f;
    private bool _isCleaned = false;

    private HighlightObject _highlight;

    void Start()
    {
        _objectRenderer = GetComponent<Renderer>();
        _highlight = GetComponent<HighlightObject>();
        //Initialize static UI elements if not already set
        if (progressBar == null)
        {
            progressBar = GameObject.Find("ProgressBar").GetComponent<Image>();
            progressBar.fillAmount = 0f;
        }

        if (hintText == null)
        {
            hintText = GameObject.Find("HintText").GetComponent<TMP_Text>();
            hintText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInteraction();
    }

    public void OnBeginLooking()
    {
        if (_isCleaned) return;
        _highlight.SetIsHighlighted(true);
        currentTarget = this;
        hintText.gameObject.SetActive(true);
        _highlight.Highlight();
    }

    public void OnFinishLooking()
    {
        _highlight.SetIsHighlighted(false);
        currentTarget = null;
        hintText.gameObject.SetActive(false);
        ResetProgress();
        _highlight.Highlight();
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnPressInteract");
    }

    void HandleInteraction()
    {
        if (_isCleaned) return;

        if (currentTarget == this && Input.GetMouseButton(0))
        {
            _lookDuration += Time.deltaTime;
            progressBar.fillAmount = _lookDuration / timeToClean;

            if (_lookDuration >= timeToClean)
            {
                _objectRenderer.material.color = targetColor;
                _isCleaned = true;
                ResetProgress();
            }
        }
        else if (currentTarget == this)
        {
            ResetProgress();
        }
    }

    void ResetProgress()
    {
        _lookDuration = 0f;
        progressBar.fillAmount = 0f;

        if (_isCleaned)
        {
            _highlight.SetIsHighlighted(false);
            hintText.gameObject.SetActive(false);
            _highlight.Highlight();
        }
    }

}

