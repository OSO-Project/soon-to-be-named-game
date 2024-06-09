using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("PlayerRotate Properties")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float rotationLimit;

    private float _vertRot;

    [Header("Mouse sensitivity")]
    [SerializeField] private SettingsInfo settingsInfo;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        Rotate();
    }
    public virtual void Rotate()
    {
        _vertRot -= GetVerticalValue();
        _vertRot = _vertRot <= -rotationLimit ? -rotationLimit :
                  _vertRot >= rotationLimit ? rotationLimit :
                  _vertRot;

        RotateVertical();
        RotateHorizontal();
    }

    protected float GetVerticalValue() => Input.GetAxis("Mouse Y") * settingsInfo.mouseSens;
    protected float GetHorizontalValue() => Input.GetAxis("Mouse X") * settingsInfo.mouseSens;
    protected virtual void RotateVertical() => cameraHolder.localRotation = Quaternion.Euler(_vertRot, 0f, 0f);
    protected virtual void RotateHorizontal() => transform.Rotate(Vector3.up * GetHorizontalValue());
}
