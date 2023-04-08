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
    private float JumpForwardLenght = 0.8f;
    [SerializeField]
    private float _speed = 10;
    [SerializeField]
    private float JumpForce = 10;
    [SerializeField]
    private float MinDot = 0.6f;
    private RaycastHit _slopeHit;
    [SerializeField]
    private CapsuleCollider _playerCollider;
    [SerializeField]
    private SphereCollider _groundCheckCollider;
    private Vector3 xzProjection;



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
        Vector3 _finalVelocity = Vector3.zero;

        if (_shouldJump && _grounded)
        {
            var velocityDirection = transform.forward;
            velocityDirection.y = 1f;
            _rigidbody.velocity = velocityDirection * JumpForce;
        }
        else if (!_grounded)
        {
            return;
        }
        else if (IsOnSlope() && _grounded)
        {
            _finalVelocity = GetSlopeMoveDirection();
            _finalVelocity *= _speed;
            _rigidbody.velocity = _finalVelocity;
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


    public void PlayerIsGrounded(Collision collision)
    {
        if(collision.collider == _groundCheckCollider)
        {
            _grounded = true;
        }
    }


    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    protected void FixedUpdate()
    {
        ApplyVelocity();
    }


    private void OnTriggerEnter(Collider other)
    {
        _grounded = true;
    }


    private void OnTriggerStay(Collider other)
    {
        _grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _grounded = false;
    }
}
