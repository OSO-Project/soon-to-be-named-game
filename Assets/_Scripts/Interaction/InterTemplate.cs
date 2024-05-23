using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InterTemplate : MonoBehaviour, Interactable
{
    //This is a template for scripts for interactable objects
    //Remember to always set the tag of the object with this script to Interactable, either through engine
    //Or better yet, do it through code with f.e putting "gameObject.tag="Interactable"" in Start() or Awake()
    //Remember to also include the Interactable interface, as shown above.
    //The interaction range is global between interactable objects, and you can set it in the
    //InteractionManager.cs script or the object with that script. If needed, i can change this.
    

    public void OnBeginLooking()
    {
	//Code to execute when you aim at the object
    }

    public void OnFinishLooking()
    {
	//Code to execute when you aim away from the object
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
	//Code to execute upon pressing the interact key
    }

}
