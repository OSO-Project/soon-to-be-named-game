using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder; // Reference to the CameraHolder
    [SerializeField] private float smoothSpeed = 0.125f;

    private Vector3 _offset; // Offset between camera and camera holder

    void Start()
    {
        // Calculate initial offset
        _offset = transform.position - cameraHolder.position;
    }

    void LateUpdate()
    {
        // Smoothly interpolate the camera position and rotation
        Vector3 desiredPosition = cameraHolder.position + _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, cameraHolder.rotation, smoothSpeed);
    }
}
