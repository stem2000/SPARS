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
    private Collider _playerCollider;

    

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
            Debug.Log($"OnSlope - {_finalVelocity}");
        }
        else
            _rigidbody.velocity = CalculateRelativeVelocity();
    }


    public void ControlContactPoints(Collision collision)
    {
        
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float dot = Vector3.Dot(normal, Vector3.up);
            if (dot > MinDot)
            {
                _grounded = true;
            }
                
        }
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
        return transform.TransformVector(Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized);
    }


    public bool PlayerIsGrounded()
    {
        Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 0.5f);
        if(_slopeHit.distance > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerCollider = GetComponentInChildren<Collider>();
    }


    protected void FixedUpdate()
    {
        ApplyVelocity();
    }


    private void OnCollisionEnter(Collision collision)
    {
        ControlContactPoints(collision);
    }


    private void OnCollisionStay(Collision collision)
    {
        ControlContactPoints(collision);
    }


    private void OnCollisionExit(Collision collision)
    {
        _grounded = false;
    }
}
