using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
	//This script manages when you're looking at an interactable object
	//And calls the interaction methods appropriately
	public static InteractionManager Instance; //Singleton

	[SerializeField] private float range;
	
	private Interactable _currentInteractableObject = null;
	private Camera _mainCamera;

	private void Start()
	{
		Instance = this;
		_mainCamera = Camera.main;
    }

	private void Update()
	{
		LookForInteractable();
	}

	private Collider _currentCollider;
	//This method's purpose is to detect when the player is looking at an object, detect if
	//the object is interactable, and call the appropriate methods. It also handles edge-cases
	//such as looking at another interactable object right after another.
	private void LookForInteractable()
	{
		RaycastHit cameraHit;
		Ray r = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);

		if(!Physics.Raycast(r, out cameraHit, range)) 
		{
			FinishLooking();
		}
		else if(cameraHit.transform.CompareTag("Interactable")) 
		{
			Collider _anotherCollider = cameraHit.collider;
			if(_currentCollider != _anotherCollider && _currentCollider != null && _currentInteractableObject != null)
			{
				_currentCollider = _anotherCollider;
				FinishLooking();
			}
			if(_currentCollider != _anotherCollider)
			{
				_currentCollider = _anotherCollider;
				_currentInteractableObject = null;
			}
			if(_currentInteractableObject == null)
			{
				_currentInteractableObject = _anotherCollider.GetComponent<Interactable>();
				_currentInteractableObject.OnBeginLooking();
			}
		}
		else
		{
			FinishLooking();
		}
	}

	private void FinishLooking()
	{
		if(_currentInteractableObject == null)
			return;
		_currentCollider=null;
		_currentInteractableObject.OnFinishLooking();
		_currentInteractableObject = null;
	}

	public void PressInteract(InputAction.CallbackContext ctx)
	{
		if(_currentInteractableObject != null)
			_currentInteractableObject.OnPressInteract(ctx);
	}

	public Interactable GetCurrentInteractable()
    {
        return _currentInteractableObject;
    }

}
