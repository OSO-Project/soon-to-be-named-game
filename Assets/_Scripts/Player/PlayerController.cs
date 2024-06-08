using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private CapsuleCollider _capsule;
    private BoxCollider _ceilingChecker;

    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float jumpStrength = 100f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float distanceToGround = 0.8f;
    [SerializeField] private float crouchDistanceToGround = 0.3f; 
    [SerializeField] private float distanceToCrouchCeiling = 0.7f;
    [SerializeField] private float crouchScale = 0.7f;
    [SerializeField] private float crouchRadius = 0.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float additionalFallGravity = 20f;

    private bool _grounded;
    public bool _canStandUp;
    public bool _crouching;
    private float _startHeight;
    private Vector3 _startCenter;
    private float _startRadius;
    private Vector3 _uncrouchCenterVelocity = Vector3.zero;
    private float _uncrouchHeightVelocity = 0;

    public bool CanMove;

    private void Start()
    {
        CanMove = true;
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponentInChildren<CapsuleCollider>();
        _ceilingChecker = GetComponentInChildren<BoxCollider>();
        _startHeight = _capsule.height;
        _startCenter = _capsule.center;
        _startRadius = _capsule.radius;
    }

    // For testing earthquake encounter
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameEventManager.Instance.HandleEncounterStart();
        }
    }

    private void FixedUpdate()
    {
        Move();
        if (!CanMove) return;
        CheckGround();
        CheckCanStandUp();
        HandleJump();
        ApplyAdditionalGravity();
        CrouchHandling();
    }

    private void Move()
    {
        if (!InputManager.Instance) return;
        if (!CanMove)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
        else
        {
            Vector2 input = InputManager.Instance.Move;
            float targetSpeed = _crouching ? crouchSpeed : (InputManager.Instance.Run ? runSpeed : walkSpeed);

            if (input == Vector2.zero) targetSpeed = 0;

            Vector3 targetVelocity = new Vector3(input.x, 0, input.y).normalized * targetSpeed;
            targetVelocity = transform.TransformDirection(targetVelocity);

            Vector3 velocity = _rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.y = 0;

            if (targetVelocity.magnitude > 0)
            {
                _rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(velocityChange * deceleration, ForceMode.Acceleration);
            }
        }
    }

    private void HandleJump()
    {
        if (InputManager.Instance.Jump && _grounded && _canStandUp)
        {
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(2 * jumpStrength * Mathf.Abs(Physics.gravity.y));
            _rb.velocity = jumpVelocity;
        }
    }

    private void ApplyAdditionalGravity()
    {
        if (!_grounded)
        {
            _rb.AddForce(Vector3.down * additionalFallGravity, ForceMode.Acceleration);
        }
    }

    private void CheckGround()
    {
        // Define the size of the box cast
        Vector3 boxSize = new Vector3(0.45f, 0.1f, 0.45f);

        // Calculate the center of the box based on crouching or standing
        Vector3 boxCenter;
        float castDistance;

        if (_crouching)
        {
            boxCenter = transform.position + Vector3.down * crouchDistanceToGround;
            castDistance = crouchDistanceToGround;
        }
        else
        {
            boxCenter = transform.position + Vector3.down * distanceToGround;
            castDistance = distanceToGround;
        }

        _grounded = Physics.BoxCast(transform.position, boxSize, Vector3.down, out RaycastHit hit, Quaternion.identity, castDistance, groundMask);
    }

    private void CheckCanStandUp()
    {
        // Get the position and size of the box collider
        Vector3 boxCenter = _ceilingChecker.transform.position;
        Vector3 boxSize = _ceilingChecker.transform.localScale;

        // Calculate the half extents of the box collider
        Vector3 halfExtents = boxSize / 2;

        // Perform the OverlapBox to check for collisions with other objects
        Collider[] colliders = Physics.OverlapBox(boxCenter, halfExtents, Quaternion.identity);

        // Check if any colliders were found and if their layer is not Ignore Raycast
        _canStandUp = ! (colliders.Length == 0 || colliders.Any(collider => collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")));
        Debug.Log(_canStandUp);
    }

    private void CrouchHandling()
    {
        if (InputManager.Instance.Crouch)
        {
            _crouching = true;
            _capsule.radius = crouchRadius;
            _capsule.height = crouchScale * _startHeight;
            _capsule.center = crouchScale * _startCenter;
        }
        else if (_crouching)
        {
            // If there is enough space, complete the uncrouching process
            if (_canStandUp)
            {
                _crouching = false;
                StartCoroutine(StandUp());
            }
        }
    }

    private IEnumerator StandUp()
    {
        // Get the target height and center
        float targetHeight = _startHeight;
        Vector3 targetCenter = _startCenter;

        // Smoothly adjust the height and center of the capsule collider
        while ((_capsule.height != targetHeight || _capsule.center != targetCenter) && !_crouching)
        {
            // Update the height and center gradually
            _capsule.height = Mathf.SmoothDamp(_capsule.height, targetHeight, ref _uncrouchHeightVelocity, 0.1f);
            _capsule.center = Vector3.SmoothDamp(_capsule.center, targetCenter, ref _uncrouchCenterVelocity, 0.1f);

            // Log the progress
            Debug.Log("Standing up...");

            yield return null;
        }

        _capsule.height = targetHeight;
        _capsule.center = targetCenter;

        Debug.Log("StandUp finished");
    }


    public float GetCurrentSpeed()
    {
        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude;
    }
}
