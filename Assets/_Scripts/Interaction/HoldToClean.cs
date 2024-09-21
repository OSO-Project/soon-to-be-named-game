using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CleanItem : MonoBehaviour, Interactable, IDirtyObject
{
    [SerializeField] protected float timeToClean = 3f;

    public static CleanItem currentTarget = null;
    public float _lookDuration = 0f;
    public bool _isCleaned = false;
    public UnlockedToolsData PlayerData;

    public Action cleanSuccesfull;

    public HighlightObject _highlight;
    public Coroutine _cleanCoroutine;
    public ParticleSystem _dustParticle;


    void Start()
    {
        _highlight = GetComponent<HighlightObject>();
        _dustParticle = GetComponentInChildren<ParticleSystem>();
        InputManager.Instance.CleanAction.performed += OnPressInteract;
    }

    private void OnDestroy()
    {
        InputManager.Instance.CleanAction.performed -= OnPressInteract;
    }

    public void OnBeginLooking()
    {
        if (_isCleaned || ToolsManager.Instance._currentlyHeld is not Wipe) return;
        _highlight.SetIsHighlighted(true);
        currentTarget = this;
        UIManager.Instance.HintText.gameObject.SetActive(true);
        _highlight.Highlight();
    }

    public void OnFinishLooking()
    {
        if (_isCleaned) return;
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
                if (_cleanCoroutine == null)
                {
                    _cleanCoroutine = StartCoroutine(CleaningProcess());
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
            if (currentTarget == this && InputManager.Instance.CleanAction.ReadValue<float>() == 0f)
            {
                StopAndResetProgress();
                yield break;
            }

            _lookDuration += Time.deltaTime;
            UIManager.Instance.ProgressBar.fillAmount = _lookDuration / timeToClean;

            if (_lookDuration >= timeToClean)
            {
                _dustParticle.Stop();
                _isCleaned = true;
                if(gameObject.GetComponent<DecalProjector>() != null)
                {
                    Destroy(gameObject, _dustParticle.main.duration);
                }
                StopAndResetProgress();
                // move to another script
                GameEventManager.Instance.CleanItem(GetDirtValue());
                cleanSuccesfull.Invoke();
                yield break;
            }

            yield return null;
        }
    }

    private void StopAndResetProgress()
    {
        if (_cleanCoroutine != null)
        {
            StopCoroutine(_cleanCoroutine);
            _cleanCoroutine = null;
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

    public void BoostHoldToCleanSpeed(float duration, float multiplier)
    {
        ChangeHoldToCleanSpeedForDuration(duration, multiplier);
    }

    public void SlowHoldToCleanSpeed(float duration, float multiplier)
    {
        ChangeHoldToCleanSpeedForDuration(duration, multiplier);
    }

    private IEnumerator ChangeHoldToCleanSpeedForDuration(float duration, float multiplier)
    {
        float originalSpeed = timeToClean;
        timeToClean *= multiplier;
        yield return new WaitForSeconds(duration);
        timeToClean = originalSpeed;
    }

    public int GetDirtValue()
    {
        return (int) timeToClean / 2;
    }

    public void Hide()
    {
        return;
    }

    public void UnHide()
    {
        return;
    }

    public bool GetHidden()
    {
        return false;
    }
}

