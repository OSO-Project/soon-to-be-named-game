using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoldToCleanTest: MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    public float InteractionDistance = 5f;
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private float timeToClean = 3f;

    private static Image progressBar;
    private static TMP_Text hintText;
    private Renderer _objectRenderer;
    private Color _originalColor;
    private static HoldToCleanTest currentTarget = null;
    private float _lookDuration = 0f;
    private bool _isCleaned = false;
    private PlayerController _playerController;

    void Start()
    {
        _objectRenderer = GetComponent<Renderer>();
        _originalColor = _objectRenderer.material.color;
        _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        // Initialize static UI elements if not already set
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

    void Update()
    {
        DetectObject();
        HandleInteraction();
    }

    void DetectObject()
    {
        if (_isCleaned) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, InteractionDistance))
        {
            if (hit.transform == transform)
            {
                currentTarget = this;
                hintText.gameObject.SetActive(true);
                return;
            }
        }

        if (currentTarget == this)
        {
            currentTarget = null;
            hintText.gameObject.SetActive(false);
            ResetProgress();
        }
    }

    void HandleInteraction()
    {
        if (_isCleaned) return;

        if (currentTarget == this && Input.GetMouseButton(0))
        {
            if (_playerController != null)
            {
                _playerController.CanMove = true; // Disable player movement
            }

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
            hintText.gameObject.SetActive(false);
        }
        if (_playerController != null)
        {
            _playerController.CanMove = true; // Disable player movement
        }
    }
}
