using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Mouse sensitivity")]
    [SerializeField] private SettingsInfo settingsInfo;
    [SerializeField] private Transform playerBody;
    private float _xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovementCamera();
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
}
