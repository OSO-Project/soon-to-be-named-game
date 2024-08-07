using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class CarriableObject : MonoBehaviour
{
    private Rigidbody objRigidbody;
    private Transform objectGrabPointTransform;
    private Transform playerTransform; // Reference to the player's transform
    [SerializeField] float lerpSpeed;
    [SerializeField] float maxDistance = 5f; // Maximum allowed distance before dropping the object
    [SerializeField] float angularDampingFactor = 0.95f; // Factor to reduce angular velocity
    private int originalLayer; // Store the original layer
    public Collider playerCollider; // Reference to the player's collider
    public Collider objCollider; // Reference to the object's collider
    [SerializeField] private float maxThrowChargeTime = 3f;
    [SerializeField] private float maxThrowStrength = 1000f;

    private Camera cam;

    private void Awake()
    {
        objRigidbody = GetComponent<Rigidbody>();
        objCollider = GetComponent<Collider>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void Grab(Transform objectGrabPointTransform, Transform playerTransform, Collider playerCollider)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        this.playerTransform = playerTransform;
        this.playerCollider = playerCollider;
        objRigidbody.useGravity = false;

        // Store the original layer and change to IgnoreRaycast layer
        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Ignore collision between the player and the object
        Physics.IgnoreCollision(playerCollider, objCollider, true);
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objRigidbody.useGravity = true;

        // Restore the original layer
        gameObject.layer = originalLayer;

        if(playerCollider != null)
            // Re-enable collision between the player and the object
            Physics.IgnoreCollision(playerCollider, objCollider, false);

        // Clear the playerCollider reference
        playerCollider = null;
    }
    public void Throw(float throwStrength)
    {
        Drop();
        Vector3 direction = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction;
        objRigidbody.AddForce(direction * throwStrength * maxThrowStrength);
    }


    private void Update()
    {
        if(objectGrabPointTransform != null)
        {
            // Check the distance between the player and the object
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > maxDistance)
            {
                Drop();
                return; // Exit early if the object is dropped
            }

            Vector3 directionToTarget = (objectGrabPointTransform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, objectGrabPointTransform.position);

            // Calculate the force magnitude with damping
            float forceMagnitude = lerpSpeed * distanceToTarget;

            // Clamp the force to prevent too high acceleration
            float maxForce = 50f; // Adjust this value as needed
            forceMagnitude = Mathf.Clamp(forceMagnitude, 0, maxForce);

            // Apply the force towards the target position
            objRigidbody.AddForce(directionToTarget * forceMagnitude, ForceMode.Acceleration);

            // Apply damping to reduce the velocity over time
            float dampingFactor = 0.9f; // Adjust this value as needed (closer to 1 means less damping)
            objRigidbody.velocity *= dampingFactor;
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // Apply angular damping to slow down rotation over time
            objRigidbody.angularVelocity *= angularDampingFactor;
        }
    }
}
