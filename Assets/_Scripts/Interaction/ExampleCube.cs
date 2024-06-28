using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExampleCube : MonoBehaviour, Interactable
{

	
    [SerializeField] private Material mat;
    private Material _startMat;
    private MeshRenderer _mr;
    private Rigidbody _rb;

    void Start()
    {
	_mr = gameObject.GetComponent<MeshRenderer>();
	_rb = gameObject.GetComponent<Rigidbody>();
	_startMat = _mr.material;
    }

    public void OnBeginLooking()
    {
	_mr.material = mat;
    }

    public void OnFinishLooking()
    {
	_mr.material = _startMat;
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
	_rb.AddForce(transform.up * 1000);
    }

}
