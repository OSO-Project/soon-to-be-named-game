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
	
	private Interactable _interactionInterface = null;
	private Camera _mainCamera;

	private void Start()
	{
		Instance = this;
		_mainCamera = Camera.main;
		InputManager.current.InteractAction.performed += PressInteract;
		
	}

	private void Update()
	{
		LookForInteractable();
	}

	private Collider _current;
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
			Collider selectedObject = cameraHit.collider;
			if(_current != selectedObject && _current != null && _interactionInterface != null)
			{
				_current = selectedObject;
				FinishLooking();
			}
			if(_current != selectedObject)
			{
				_current = selectedObject;
				_interactionInterface = null;
			}
			if(_interactionInterface == null)
			{
				_interactionInterface = selectedObject.GetComponent<Interactable>();
				_interactionInterface.OnBeginLooking();
			}
		}
		else
		{
			FinishLooking();
		}

	}

	private void FinishLooking()
	{
		if(_interactionInterface == null)
			return;
		_current=null;
		_interactionInterface.OnFinishLooking();
		_interactionInterface = null;
	}

	private void PressInteract(InputAction.CallbackContext ctx)
	{
		if(_interactionInterface != null)
			_interactionInterface.OnPressInteract(ctx);
	}

}
