using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Ball : MonoBehaviour
{ 
    private GameObject _player;
    private Rigidbody _playerRB;
    private Rigidbody _ballRB;
    private Collider _ballHitBox;
    private CarriableObject _carriableObject;
    private ContactPoint _contactPoint;
    [SerializeField] private GameObject _ballDecal;
    private float lastSpawnTime = -3f; // Initializing to allow immediate first spawn
    [SerializeField] private float spawnCooldown = 3f; // 3 seconds cooldown
    [SerializeField] private LayerMask printableLayer;

    [SerializeField] private AudioClip kickSoundFX;

    void Awake()
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

        //CarriableObject.OnPickUpObject += HandlePickUpObject;
        //DropCollider.OnDropObject += HandleDropOrThrowObject;
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

                //play the sound of kicking the ball
                SoundFxManager.instance .PlaySoundFXClip(kickSoundFX, transform, 1f);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((printableLayer | (1 << collision.gameObject.layer)) != printableLayer) return;
        // Check if enough time has passed since the last spawn
        if (Time.time - lastSpawnTime >= spawnCooldown)
        {
            if (_ballRB.velocity.magnitude >= 5)
            {
                // Get the contact point
                _contactPoint = collision.GetContact(0);

                // Calculate the rotation to make the decal face opposite to the normal
                Quaternion rotation = Quaternion.LookRotation(-_contactPoint.normal, Vector3.up);

                // Apply a 180-degree rotation around the Z-axis
                rotation *= Quaternion.Euler(0, 0, 180);

                // Offset the decal position slightly away from the wall
                Vector3 offsetPosition = _contactPoint.point + _contactPoint.normal * 0.01f; // Adjust the offset distance as needed

                // Instantiate the decal
                Instantiate(_ballDecal, offsetPosition, rotation);
                Debug.Log("Decal spawned!");

                // Update the last spawn time
                lastSpawnTime = Time.time;
            }
        }
    }

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

        //CarriableObject.OnPickUpObject -= HandlePickUpObject;
        //DropCollider.OnDropObject -= HandleDropOrThrowObject;
        InputManager.Instance.ThrowAction.performed -= HandleDropOrThrowObject;
    }

}