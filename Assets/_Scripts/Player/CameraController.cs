using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder; // Reference to the CameraHolder
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private PlayerController playerController;
    private Vector3 _offset; // Offset between camera and camera holder

    [Header("Earthquake Camera Shake Settings")]
    [SerializeField] private float shakeAmount = 0.7f;
    [SerializeField] private float decreaseFactor = 1.0f;

    private Vector3 originalPos;
    private Vector3 cameraHolderOriginalPos;
    private float currentShakeDuration = 0f;

    [Header("Head Bobbing Settings")]
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float runBobAmount = 0.1f;
    [SerializeField] private float crouchBobAmount = 0.03f;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float runBobSpeed = 18f;
    [SerializeField] private float crouchBobSpeed = 8f;

    private float bobTimer = 0f;

    [Header("Landing Effect Settings")]
    [SerializeField] private float landingOffset = 0.1f;
    [SerializeField] private float landingSmoothTime = 0.2f;

    private bool isLanding = false;
    private float landingVelocity = 0f;

    public static CameraShakeInstance EarthquakeCustom
    {
        get
        {
            CameraShakeInstance c = new CameraShakeInstance(0.6f, 3.5f, 2f, 10f);
            c.PositionInfluence = Vector3.one * 0.25f;
            c.RotationInfluence = new Vector3(1, 1, 4);
            return c;
        }
    }

    CameraShakeInstance shake;


    void Start()
    {
        // Calculate initial offset
        _offset = transform.position - cameraHolder.position;

        originalPos = transform.localPosition;
        cameraHolderOriginalPos = cameraHolder.localPosition;

        GameEventManager.Instance.OnEarthquakeEncounterStart += HandleCameraShake;

        playerController.OnLand += StartLandingEffect;
    }

    void LateUpdate()
    {
        // Smoothly interpolate the camera position and rotation
        Vector3 desiredPosition = cameraHolder.position + _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, cameraHolder.rotation, smoothSpeed);

        HandleHeadBobbing();
    }

    public void TriggerShake(float duration)
    {
        currentShakeDuration = duration;
    }

    // Camera shake for earthquake encounter
    private void HandleCameraShake(float duration)
    {
        /*if (currentShakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            currentShakeDuration = 0f;
            transform.localPosition = originalPos;
        }*/
        //shake = CameraShaker.Instance.StartShake(0.6f, 3.5f, 2f);
        CameraShaker.Instance.Shake(EarthquakeCustom);
        //shake.StartFadeOut(duration);
        //;
    }

    private void HandleHeadBobbing()
    {
        if (isLanding) return;
        if (playerController.IsWalking())
        {
            bobTimer += Time.deltaTime * walkBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * walkBobAmount;
            cameraHolder.localPosition = new Vector3(cameraHolderOriginalPos.x, cameraHolderOriginalPos.y + bobOffset, cameraHolderOriginalPos.z);
        }
        else if (playerController.IsRunning())
        {
            bobTimer += Time.deltaTime * runBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * runBobAmount;
            cameraHolder.localPosition = new Vector3(cameraHolderOriginalPos.x, cameraHolderOriginalPos.y + bobOffset, cameraHolderOriginalPos.z);
        }
        else if (playerController.IsCrouching())
        {
            bobTimer += Time.deltaTime * crouchBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * crouchBobAmount;
            cameraHolder.localPosition = new Vector3(cameraHolderOriginalPos.x, cameraHolderOriginalPos.y + bobOffset, cameraHolderOriginalPos.z);
        }
        else
        {
            bobTimer = 0f;
            cameraHolder.localPosition = cameraHolderOriginalPos;
        }
    }

    private void StartLandingEffect()
    {
        if (!isLanding)
        {
            StartCoroutine(LandingEffect());
        }
    }

    private IEnumerator LandingEffect()
    {
        isLanding = true;
        float startY = cameraHolder.localPosition.y;
        float targetY = cameraHolderOriginalPos.y - landingOffset;

        Debug.Log("Landing effect started");

        // Move camera downwards
        while (Mathf.Abs(cameraHolder.localPosition.y - targetY) > 0.01f)
        {
            cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, Mathf.SmoothDamp(cameraHolder.localPosition.y, targetY, ref landingVelocity, landingSmoothTime), cameraHolder.localPosition.z);
            yield return null;
        }

        // Ensure it reaches the targetY position
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, targetY, cameraHolder.localPosition.z);

        // Move camera back to original position
        while (Mathf.Abs(cameraHolder.localPosition.y - cameraHolderOriginalPos.y) > 0.01f)
        {
            cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, Mathf.SmoothDamp(cameraHolder.localPosition.y, cameraHolderOriginalPos.y, ref landingVelocity, landingSmoothTime), cameraHolder.localPosition.z);
            yield return null;
        }

        // Ensure it reaches the original position
        cameraHolder.localPosition = cameraHolderOriginalPos;
        isLanding = false;

        Debug.Log("Landing effect ended");
    }
}
