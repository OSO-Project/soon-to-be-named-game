using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{ 
    private GameObject _player;
    private Rigidbody _playerRB;
    private Rigidbody _ballRB;
    private Collider _ballHitBox;
    private CarriableObject _carriableObject;
    
    // private ContactPoint _contactPoint;
    [SerializeField] private GameObject _ballDecal;

    void Start()
    {
        // Find the CarriableObject script
        _carriableObject = GetComponent<CarriableObject>();
        if (_carriableObject == null)
        {
            Debug.LogError("Carriable Object script not found!");
            return;
        }

        // Find the player object by tag
        _player = GameObject.FindWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Get the Rigidbody component from the player (parent object)
        _playerRB = _player.GetComponent<Rigidbody>();
        if (_playerRB == null)
        {
            Debug.LogError("Rigidbody component not found on the player.");
            return;
        }

        // Assign the Rigidbody of the ball
        _ballRB = GetComponent<Rigidbody>();
        if (_ballRB == null)
        {
            Debug.LogError("Rigidbody component not found on the Ball object.");
            return;
        }

        // Find the BallHitBox child object and get its Collider
        _ballHitBox = transform.Find("BallHitBox").GetComponent<Collider>();
        if (_ballHitBox == null)
        {
            Debug.LogError("BallHitBox child object or its Collider not found.");
            return;
        }

        // Find the decal prefab
        _carriableObject = GetComponent<CarriableObject>();
        if (_carriableObject == null)
        {
            Debug.LogError("Carriable Object script not found!");
            return;
        }

        // Ensure BallHitBox is set as a trigger
        _ballHitBox.isTrigger = true;

        // Subscribe to the OnPickUpObject event
        CarriableObject.OnPickUpObject += HandlePickUpObject;
        DropCollider.OnDropObject += HandleDropOrThrowObject;
        InputManager.Instance.ThrowAction.performed += HandleDropOrThrowObject;

    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider entering the trigger is the player's collider
        if (other.transform.IsChildOf(_player.transform))
        {
            // Check the player's velocity magnitude
            if (_playerRB.velocity.magnitude >= 3)
            {
                // Calculate the direction from the player to the ball
                Vector3 direction = (transform.position - _player.transform.position).normalized;

                // Generate a random angle between 45 and 80 degrees
                float randomAngle = Random.Range(45.0f, 80.0f);

                // Rotate the direction vector by the random angle up
                Vector3 rotatedDirection = Quaternion.Euler(randomAngle, 0, 0) * direction;

                // Apply a force to the ball
                _ballRB.AddForce(rotatedDirection * 1500);
            }
        }
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     Quaternion rotation = Quaternion.LookRotation(_contactPoint.normal, Vector3.up);
    //     _contactPoint = collision.GetContact(0);
    //     Instantiate(_ballDecal,_contactPoint.point, rotation);
    // }

    private void HandlePickUpObject(GameObject pickedUpObject)
    {
        if (pickedUpObject == gameObject)
        {
            _ballRB.interpolation = RigidbodyInterpolation.None;
        }
    }

    private void HandleDropOrThrowObject(GameObject droppedOrThrownObject)
    {
        Debug.Log("Drop");
        if (droppedOrThrownObject == gameObject)
        {
            // Revert Rigidbody parameters when the object is dropped or thrown
            _ballRB.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void HandleDropOrThrowObject(InputAction.CallbackContext ctx)
    {
        Debug.Log("Throw");

        // Revert Rigidbody parameters when the object is thrown
        _ballRB.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnDestroy()
    {
        // Unsubscribe from the OnPickUpObject event
        CarriableObject.OnPickUpObject -= HandlePickUpObject;
        DropCollider.OnDropObject -= HandleDropOrThrowObject;
        InputManager.Instance.ThrowAction.performed -= HandleDropOrThrowObject;
    }

}