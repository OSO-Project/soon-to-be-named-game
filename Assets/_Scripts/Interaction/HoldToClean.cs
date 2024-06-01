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

    private Renderer _objectRenderer;
    private static HoldToClean currentTarget = null;
    private float _lookDuration = 0f;
    private bool _isCleaned = false;

    private HighlightObject _highlight;
    private Coroutine cleanCoroutine;
    void Start()
    {
        _objectRenderer = GetComponent<Renderer>();
        _highlight = GetComponent<HighlightObject>();
    }

    public void OnBeginLooking()
    {
        if (_isCleaned) return;
        _highlight.SetIsHighlighted(true);
        currentTarget = this;
        UIManager.Instance.HintText.gameObject.SetActive(true);
        _highlight.Highlight();
    }

    public void OnFinishLooking()
    {
        _highlight.SetIsHighlighted(false);
        currentTarget = null;
        UIManager.Instance.HintText.gameObject.SetActive(false);
        StopAndResetProgress();
        _highlight.Highlight();
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        if (_isCleaned) return;

        if (currentTarget == this)
        {
            if (ctx.performed)
            {
                if (cleanCoroutine == null)
                {
                    cleanCoroutine = StartCoroutine(CleaningProcess());
                }
            }
            else if (ctx.canceled)
            {
                StopAndResetProgress();
            }
        }
    }

    private IEnumerator CleaningProcess()
    {
        while (true)
        {
            if (currentTarget == this && InputManager.Instance.InteractAction.ReadValue<float>() == 0f)
            {
                StopAndResetProgress();
                yield break;
            }

            _lookDuration += Time.deltaTime;
            UIManager.Instance.ProgressBar.fillAmount = _lookDuration / timeToClean;

            if (_lookDuration >= timeToClean)
            {
                _objectRenderer.material.color = targetColor;
                _isCleaned = true;
                StopAndResetProgress();
                yield break;
            }

            yield return null;
        }
    }

    private void StopAndResetProgress()
    {
        if (cleanCoroutine != null)
        {
            StopCoroutine(cleanCoroutine);
            cleanCoroutine = null;
        }

        _lookDuration = 0f;
        UIManager.Instance.ProgressBar.fillAmount = 0f;

        if (_isCleaned)
        {
            _highlight.SetIsHighlighted(false);
            UIManager.Instance.HintText.gameObject.SetActive(false);
            _highlight.Highlight();
        }
    }

}

