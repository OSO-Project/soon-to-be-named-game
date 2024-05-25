using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public interface Interactable
{
   //this interface simply provides three methods that interactable objects need to implement
	void OnBeginLooking();
	void OnFinishLooking();
	void OnPressInteract(InputAction.CallbackContext ctx);

}
