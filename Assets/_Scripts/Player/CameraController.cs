using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder; // Reference to the CameraHolder
    [SerializeField] private float smoothSpeed = 0.125f;

    private Vector3 _offset; // Offset between camera and camera holder

    [Header("Earthquake Camera Shake Settings")]
    [SerializeField] private float shakeAmount = 0.7f;
    [SerializeField] private float decreaseFactor = 1.0f;

    private Vector3 originalPos;
    private float currentShakeDuration = 0f;

    void Start()
    {
        // Calculate initial offset
        _offset = transform.position - cameraHolder.position;

        originalPos = transform.localPosition;

        GameEventManager.Instance.OnEarthquakeEncounterStart += TriggerShake;
    }

    private void Update()
    {
        HandleCameraShake();
    }

    void LateUpdate()
    {
        // Smoothly interpolate the camera position and rotation
        Vector3 desiredPosition = cameraHolder.position + _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, cameraHolder.rotation, smoothSpeed);
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
