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
    [SerializeField] private BoxCollider _groundChecker;

    private float _currentSpeed;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float jumpStrength = 100f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float crouchGroundCheckerCenter = 0.52f;
    [SerializeField] private float crouchScale = 0.7f;
    [SerializeField] private float crouchRadius = 0.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float additionalFallGravity = 20f;

    public bool _grounded;
    private bool _wasGrounded;
    public bool _canStandUp;
    private bool _crouching;
    private float _startHeight;
    private Vector3 _startCenter;
    private Vector3 _uncrouchCenterVelocity = Vector3.zero;
    private float _uncrouchHeightVelocity = 0;

    public bool CanMove;

    public event System.Action OnLand;

    private void Start()
    {
        CanMove = true;
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponentInChildren<CapsuleCollider>();
        _ceilingChecker = GetComponentInChildren<BoxCollider>();
        _startHeight = _capsule.height;
        _startCenter = _capsule.center;
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;
        Move();
        CheckGround();
        DetectLanding();
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
            _currentSpeed = _crouching ? crouchSpeed : (InputManager.Instance.Run ? runSpeed : walkSpeed);

            if (input == Vector2.zero) _currentSpeed = 0;

            Vector3 targetVelocity = new Vector3(input.x, 0, input.y).normalized * _currentSpeed;
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
            // Preserve the horizontal velocity
            Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            // Calculate the jump velocity
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(2 * jumpStrength * Mathf.Abs(Physics.gravity.y));
            // Apply the jump while preserving horizontal speed
            _rb.velocity = horizontalVelocity + jumpVelocity;
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
        // Define the size of the box for overlap based on the ground checker's size
        Vector3 boxSize = _groundChecker.size;

        // Use the ground checker's position for the center of the box
        Vector3 boxCenter;
        if (_crouching)
        {
            Vector3 newCenter = _groundChecker.center;
            newCenter.y = crouchGroundCheckerCenter;
            boxCenter = newCenter + _groundChecker.transform.position;
        }
        else
        {
            boxCenter = _groundChecker.center + _groundChecker.transform.position;
        }
        // Perform the OverlapBox to check for ground
        Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize / 2, Quaternion.identity, groundMask);

        // Check if any colliders were found and set grounded accordingly
        _grounded = colliders.Length > 0;
        Vector3[] points = new Vector3[8];

        // Calculate the 8 corners of the box
        points[0] = boxCenter + new Vector3((boxSize / 2).x, (boxSize / 2).y, (boxSize / 2).z);
        points[1] = boxCenter + new Vector3((boxSize / 2).x, (boxSize / 2).y, -(boxSize / 2).z);
        points[2] = boxCenter + new Vector3((boxSize / 2).x, -(boxSize / 2).y, (boxSize / 2).z);
        points[3] = boxCenter + new Vector3((boxSize / 2).x, -(boxSize / 2).y, -(boxSize / 2).z);
        points[4] = boxCenter + new Vector3(-(boxSize / 2).x, (boxSize / 2).y, (boxSize / 2).z);
        points[5] = boxCenter + new Vector3(-(boxSize / 2).x, (boxSize / 2).y, -(boxSize / 2).z);
        points[6] = boxCenter + new Vector3(-(boxSize / 2).x, -(boxSize / 2).y, (boxSize / 2).z);
        points[7] = boxCenter + new Vector3(-(boxSize / 2).x, -(boxSize / 2).y, -(boxSize / 2).z);

        // Draw the edges of the box
        Debug.DrawLine(points[0], points[1], Color.blue);
        Debug.DrawLine(points[0], points[2], Color.blue);
        Debug.DrawLine(points[0], points[4], Color.blue);
        Debug.DrawLine(points[1], points[3], Color.blue);
        Debug.DrawLine(points[1], points[5], Color.blue);
        Debug.DrawLine(points[2], points[3], Color.blue);
        Debug.DrawLine(points[2], points[6], Color.blue);
        Debug.DrawLine(points[3], points[7], Color.blue);
        Debug.DrawLine(points[4], points[5], Color.blue);
        Debug.DrawLine(points[4], points[6], Color.blue);
        Debug.DrawLine(points[5], points[7], Color.blue);
        Debug.DrawLine(points[6], points[7], Color.blue);
    }

    private void DetectLanding()
    {

        if (!_wasGrounded && _grounded)
        {
            Debug.Log("Landed! DetectLanding Player");
            OnLand?.Invoke();
        }
        _wasGrounded = _grounded;
    }

    private void CheckCanStandUp()
    {
        // Get the position and size of the box collider
        Vector3 boxSize = _ceilingChecker.size;

        // Use the ground checker's position for the center of the box
        Vector3 boxCenter = _ceilingChecker.center + _ceilingChecker.transform.position;

        // Calculate the half extents of the box collider
        Vector3 halfExtents = boxSize / 2;

        // Perform the OverlapBox to check for collisions with other objects
        Collider[] colliders = Physics.OverlapBox(boxCenter, halfExtents, Quaternion.identity);

        // Check if any colliders were found and if their layer is not Ignore Raycast
        _canStandUp = !_crouching || !(colliders.Length == 0 || colliders.Any(collider => collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")));


        Vector3[] points = new Vector3[8];

        // Calculate the 8 corners of the box
        points[0] = boxCenter + new Vector3((boxSize / 2).x, (boxSize / 2).y, (boxSize / 2).z);
        points[1] = boxCenter + new Vector3((boxSize / 2).x, (boxSize / 2).y, -(boxSize / 2).z);
        points[2] = boxCenter + new Vector3((boxSize / 2).x, -(boxSize / 2).y, (boxSize / 2).z);
        points[3] = boxCenter + new Vector3((boxSize / 2).x, -(boxSize / 2).y, -(boxSize / 2).z);
        points[4] = boxCenter + new Vector3(-(boxSize / 2).x, (boxSize / 2).y, (boxSize / 2).z);
        points[5] = boxCenter + new Vector3(-(boxSize / 2).x, (boxSize / 2).y, -(boxSize / 2).z);
        points[6] = boxCenter + new Vector3(-(boxSize / 2).x, -(boxSize / 2).y, (boxSize / 2).z);
        points[7] = boxCenter + new Vector3(-(boxSize / 2).x, -(boxSize / 2).y, -(boxSize / 2).z);

        // Draw the edges of the box
        Debug.DrawLine(points[0], points[1], Color.red);
        Debug.DrawLine(points[0], points[2], Color.red);
        Debug.DrawLine(points[0], points[4], Color.red);
        Debug.DrawLine(points[1], points[3], Color.red);
        Debug.DrawLine(points[1], points[5], Color.red);
        Debug.DrawLine(points[2], points[3], Color.red);
        Debug.DrawLine(points[2], points[6], Color.red);
        Debug.DrawLine(points[3], points[7], Color.red);
        Debug.DrawLine(points[4], points[5], Color.red);
        Debug.DrawLine(points[4], points[6], Color.red);
        Debug.DrawLine(points[5], points[7], Color.red);
        Debug.DrawLine(points[6], points[7], Color.red);
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

    public bool IsGrounded()
    {
        return _grounded;
    }

    public bool IsWalking()
    {
        return _currentSpeed > 0 && _currentSpeed <= walkSpeed && !_crouching;
    }

    public bool IsRunning()
    {
        return _currentSpeed > walkSpeed && _currentSpeed <= runSpeed && !_crouching;
    }

    public bool IsCrouching()
    {
        return _crouching && _currentSpeed > 0;
    }
}
