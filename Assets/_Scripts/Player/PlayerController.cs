using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private CapsuleCollider _capsule;

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
    private bool _crouching;
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
        
        if (_crouching)
        {
            _grounded = Physics.Raycast(transform.position, Vector3.down, crouchDistanceToGround, groundMask);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * crouchDistanceToGround, Color.red);
        }
        else
        {
            _grounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround, groundMask);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * distanceToGround, Color.red);
        }
        
    }

    private void CheckCanStandUp()
    {
        Debug.DrawLine(_rb.worldCenterOfMass, _rb.worldCenterOfMass + Vector3.up * distanceToCrouchCeiling, Color.blue);
        _canStandUp = !Physics.Raycast(_rb.worldCenterOfMass, Vector3.up, distanceToCrouchCeiling);
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
        else if (_canStandUp)
        {
            _crouching = false;
            _capsule.radius = _startRadius;
            _capsule.height = Mathf.SmoothDamp(_capsule.height, _startHeight, ref _uncrouchHeightVelocity, 0.1f);
            _capsule.center = Vector3.SmoothDamp(_capsule.center, _startCenter, ref _uncrouchCenterVelocity, 0.1f);
        }
    }

    public float GetCurrentSpeed()
    {
        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude;
    }
}
