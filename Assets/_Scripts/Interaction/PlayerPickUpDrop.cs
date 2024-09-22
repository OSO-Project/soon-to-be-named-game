using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private float pickUpDistance;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private float maxThrowChargeTime = 3f;

    private CarriableObject carriableObject;
    private float throwStartTime;
    private bool isThrowing;

    private void Start()
    {
        InputManager.Instance.GrabDropAction.performed += GrabDrop;
        InputManager.Instance.ThrowAction.performed += StartThrow;
        InputManager.Instance.ThrowAction.canceled += PerformThrow;
    }

    private void OnDestroy()
    {
        InputManager.Instance.GrabDropAction.performed -= GrabDrop;
        InputManager.Instance.ThrowAction.performed -= StartThrow;
        InputManager.Instance.ThrowAction.canceled -= PerformThrow;
    }

    private void GrabDrop(InputAction.CallbackContext ctx)
    {
        if (carriableObject == null)
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, 
                out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
            {
                UIManager.Instance.ShowThisCrosshairAndHideOthers(UIManager.Instance.handOpenCrosshair);
                if (raycastHit.transform.TryGetComponent(out carriableObject))
                {
                    //Manage the hand in UI
                    UIManager.Instance.ShowThisCrosshairAndHideOthers(UIManager.Instance.handClosedCrosshair);
                    carriableObject.Grab(objectGrabPointTransform, transform, playerCollider);
                }
            }
        }
        else
        {
            carriableObject.Drop();
            carriableObject = null;
            //Manage the hand in UI
            UIManager.Instance.mainCrosshair.gameObject.SetActive(false);
            UIManager.Instance.handOpenCrosshair.gameObject.SetActive(true);
        }
    }

    private void StartThrow(InputAction.CallbackContext ctx)
    {
        if (carriableObject != null)
        {
            throwStartTime = Time.time;
            isThrowing = true;
        }
    }

    private void PerformThrow(InputAction.CallbackContext ctx)
    {
        if (isThrowing && carriableObject != null)
        {
            float holdTime = Time.time - throwStartTime;
            float throwStrength = Mathf.Clamp(holdTime / maxThrowChargeTime, 0, 1);
            carriableObject.Throw(throwStrength);
            carriableObject = null;
            isThrowing = false;
        }
    }
}
