using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _moveDirection;
    private bool _grounded;
    private bool _shouldJump;
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _speed = 10;
    [SerializeField]
    private float JumpForce = 10;
    private RaycastHit _slopeHit;
    [SerializeField]
    private CapsuleCollider _playerCollider;
    [SerializeField]
    private SphereCollider _groundCheckCollider;
    [SerializeField] private float CoyoteTime = 0.2f;
    private float _coyoteTimeCounter;


    public Vector3 MoveDirection 
    { 
        get { return _moveDirection;} 
        set { _moveDirection = value.normalized;} 
    }


    public bool ShouldJump
    {
        get { return _shouldJump; }
        set { _shouldJump = value; }
    }


    public bool Grounded
    {
        get { return _grounded; }
    }

    public float Speed
    {
        get { return _speed; }
    }


    protected Vector3 CalculateRelativeVelocity()
    {
        return  transform.TransformVector(_moveDirection) * _speed;
    }

    protected void ApplyVelocity()
    {
        if (_shouldJump && _coyoteTimeCounter > 0)
        {
            Vector3 velocityDirection;
            velocityDirection = _moveDirection.normalized;
            velocityDirection.y = 1f;
            _rigidbody.velocity = transform.TransformVector(velocityDirection.normalized) * JumpForce;
            _coyoteTimeCounter = 0f;
        }
        else if (!_grounded)
        {
            var newVelocityDirection = Vector3.Project(_rigidbody.velocity, transform.forward);
            newVelocityDirection.y = _rigidbody.velocity.y;
            _rigidbody.velocity = newVelocityDirection;
        }
        else if (IsOnSlope() && _grounded)
        {
            Vector3 finalVelocity = Vector3.zero;
            finalVelocity = GetSlopeMoveDirection();
            finalVelocity *= _speed;
            _rigidbody.velocity = finalVelocity;
        }
        else
            _rigidbody.velocity = CalculateRelativeVelocity();
    }


    private bool IsOnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, transform.lossyScale.y * 0.5f))
        {
            var dot = Vector3.Dot(Vector3.up, _slopeHit.normal);
            if(dot != 1)
                return true;
        }
        return false;
    }


    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(transform.TransformVector(_moveDirection), _slopeHit.normal).normalized;
    }


    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _coyoteTimeCounter = CoyoteTime;
    }


    protected void FixedUpdate()
    {
        ApplyVelocity();
    }


    protected void Update()
    {
        if(!_grounded)
            _coyoteTimeCounter -= Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        _grounded = true;
    }


    private void OnTriggerStay(Collider other)
    {
        _grounded = true;
        _coyoteTimeCounter = CoyoteTime;
    }

    private void OnTriggerExit(Collider other)
    {
        _grounded = false;
    }
}
