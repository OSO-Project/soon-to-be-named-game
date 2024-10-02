using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HoldToClean : MonoBehaviour, Interactable
{
    [SerializeField] protected float timeToClean = 3f;

    public static HoldToClean currentTarget = null;
    public float _lookDuration = 0f;
    public bool _isCleaned = false;
    public UnlockedToolsData PlayerData;

    public Action cleanSuccesfull;

    public HighlightObject _highlight;
    public Coroutine _cleanCoroutine;
    public ParticleSystem _dustParticle;
    [SerializeField] private GameObject progressBarCanvas;
    public Image progressBar;

    void Start()
    {
        _highlight = GetComponent<HighlightObject>();
        _dustParticle = GetComponentInChildren<ParticleSystem>();
        InputManager.Instance.CleanAction.performed += OnPressInteract;
    }

    private void OnEnable()
    {
        ToolsManager.toolSwap += OnToolSwap;
    }

    private void OnDisable()
    {
        ToolsManager.toolSwap -= OnToolSwap;
    }

    private void OnDestroy()
    {
        InputManager.Instance.CleanAction.performed -= OnPressInteract;
    }

    private void OnToolSwap()
    {
        if (UnityEngine.Object.ReferenceEquals(InteractionManager.Instance.GetCurrentInteractable(), this))
        {
            if (ToolsManager.Instance._currentlyHeld is not Wipe)
            {
                if (_isCleaned) return;
                StopAndResetProgress();
            }
            else
            {
                OnBeginLooking();
            }
        }
    }

    public void OnBeginLooking()
    {
        if (_isCleaned || ToolsManager.Instance._currentlyHeld is not Wipe) return;
        _highlight.SetIsHighlighted(true);
        currentTarget = this;
        progressBarCanvas.SetActive(true);
        UIManager.Instance.HintText.gameObject.SetActive(true);
        _highlight.Highlight(Color.white);
    }

    public void OnFinishLooking()
    {
        if (_isCleaned) return;
        _highlight.SetIsHighlighted(false);
        currentTarget = null;
        progressBarCanvas.SetActive(false);
        UIManager.Instance.HintText.gameObject.SetActive(false);
        StopAndResetProgress();
        _highlight.Highlight(Color.white);
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
            if (currentTarget == this && InputManager.Instance.CleanAction.ReadValue<float>() == 0f || ToolsManager.Instance._currentlyHeld is not Wipe)
            {
                StopAndResetProgress();
                yield break;
            }

            _lookDuration += Time.deltaTime;
            progressBar.fillAmount = _lookDuration / timeToClean;

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
                //GameEventManager.Instance.AddScore(50 * timeToClean);
                // cleanSuccesfull.Invoke();
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
        progressBar.fillAmount = 0f;

        if (_isCleaned || ToolsManager.Instance._currentlyHeld is not Wipe)
        {
            _highlight.SetIsHighlighted(false);
            progressBarCanvas.SetActive(false);
            UIManager.Instance.HintText.gameObject.SetActive(false);
            _highlight.Highlight(Color.white);
        }
    }
}