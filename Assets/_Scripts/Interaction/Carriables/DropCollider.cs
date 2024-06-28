using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropCollider : MonoBehaviour, Interactable
{

	public static event Action<GameObject> OnDropObject;

	private GameObject _carriedItemRef;

	public void Start()
	{
		CarriableObject.OnPickUpObject += ItemPickedUp;
	}

	public void OnBeginLooking(){
	}
	public void OnFinishLooking(){
	}

	public void OnPressInteract(InputAction.CallbackContext ctx)
	{
		OnDropObject?.Invoke(_carriedItemRef);
		gameObject.SetActive(false);
	}
	private void ItemPickedUp(GameObject go)
	{
        gameObject.SetActive(true);
		_carriedItemRef = go;
	}


}
