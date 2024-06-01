using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Mouse sensitivity")]
    [SerializeField] private SettingsInfo settingsInfo;
    [SerializeField] private Transform playerBody;
    private float _xRotation = 0f;

    [Header("Earthquake Camera Shake Settings")]
    [SerializeField] private float shakeAmount = 0.7f;
    [SerializeField] private float decreaseFactor = 1.0f;

    private Vector3 originalPos;
    private float currentShakeDuration = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalPos = transform.localPosition;

        GameEventManager.Instance.OnEarthquakeEncounterStart += TriggerShake;
    }

    void Update()
    {
        MovementCamera();
        HandleCameraShake();
    }

    private void MovementCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * settingsInfo.mouseSens * 10 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * settingsInfo.mouseSens * 10 * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void TriggerShake(float duration)
    {
        currentShakeDuration = duration;
    }

    // Camera shake for earthquake encounter
    private void HandleCameraShake()
    {
        if (currentShakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            currentShakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }
}
