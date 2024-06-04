using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent (typeof(Rigidbody))]
public class CarriableObject : MonoBehaviour, Interactable
{
	[SerializeField] private float xOffset,yOffset,zOffset;
	[SerializeField] private float startAngleX, startAngleY, startAngleZ;
	[SerializeField] private float colliderScaleFactor;
	private Camera _mainCam;
	private Rigidbody _rb;
	private Collider _col;
	private bool _isBeingCarried = false;
	private Transform _prevParent;
	private int _prevLayer;

	public void Start()
	{
		_mainCam = Camera.main;
		_rb = GetComponent<Rigidbody>();
		_col = GetComponent<Collider>();
		_prevParent = gameObject.transform.parent;
		_prevLayer = gameObject.layer;
	}

    public void OnBeginLooking()
    {
		
    }

    public void OnFinishLooking()
    {

    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
		if(!_isBeingCarried)
		{
			gameObject.transform.position = _mainCam.transform.position; 
			gameObject.transform.parent=_mainCam.transform;
			gameObject.transform.localPosition += new Vector3(xOffset,yOffset,zOffset);
			_rb.useGravity = false;
			_rb.isKinematic = true;
			//_col.enabled = false;
			_col.isTrigger = true;
			_isBeingCarried = true;
			//_col.size *= new Vector3 (colliderScaleFactor, colliderScaleFactor, colliderScaleFactor);
			gameObject.layer = LayerMask.NameToLayer("Carriable");
			return;
		}

		gameObject.transform.parent=_prevParent;
		_rb.useGravity = true;
		_rb.isKinematic = false;
		//_col.enabled = true;
		_col.isTrigger=false;
		_isBeingCarried = false;
		//_col.size /= colliderScaleFactor;
		gameObject.layer = _prevLayer; 
    }

}
