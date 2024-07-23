using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class CarriableObject : MonoBehaviour, Interactable
{

    public static event Action<GameObject> OnPickUpObject;

    public bool _isBeingCarried = false;

    [SerializeField] private float xOffset, yOffset, zOffset;
    [SerializeField] private float throwStrength;

    private Camera _mainCam;
    private Rigidbody _rb;
    private Collider _col;
    private Transform _prevParent;
    private int _prevLayer;
    private bool _useGravity, _isKinematic, _isTrigger;

    public void Start()
    {
        _mainCam = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _prevParent = gameObject.transform.parent;
        _prevLayer = gameObject.layer;
        _useGravity = _rb.useGravity;
        _isKinematic = _rb.isKinematic;
        _isTrigger = _col.isTrigger;

        DropCollider.OnDropObject += Drop;
        InputManager.Instance.ThrowAction.performed += Throw;
    }

    public void OnBeginLooking()
    {

    }

    public void OnFinishLooking()
    {

    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        if (_isBeingCarried)
        {
            return;
        }
        gameObject.transform.position = _mainCam.transform.position;
        gameObject.transform.parent = _mainCam.transform;
        gameObject.transform.localPosition += new Vector3(xOffset, yOffset, zOffset);
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _col.isTrigger = true;
        _isBeingCarried = true;
        gameObject.layer = LayerMask.NameToLayer("Carriable");
        OnPickUpObject?.Invoke(gameObject);
    }

    private void Drop(GameObject go)
    {
        if (!_isBeingCarried)
            return;
        gameObject.transform.parent = _prevParent;
        _rb.useGravity = _useGravity;
        _rb.isKinematic = _isKinematic;
        _col.isTrigger = _isTrigger;
        _isBeingCarried = false;
        gameObject.layer = _prevLayer;
    }

    private void Throw(InputAction.CallbackContext ctx)
    {
        if (!_isBeingCarried)
            return;
        Drop(gameObject);
        Vector3 direction = _mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction;
        _rb.AddForce(direction * throwStrength);
    }

}
