using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _moveDirection;
    private bool _grounded;
    private bool _shouldJump;
    private Rigidbody _rigidbody;
    private Vector3 _finalVelocity;
    [SerializeField]
    private float _speed = 10;
    [SerializeField]
    private float _jumpForce = 10;

    

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


    protected Vector3 CalculateRelativeVelocity()
    {
        var _relativeSpeedVector = transform.TransformVector(_moveDirection) * _speed;
        return _relativeSpeedVector;
    }


    public void ControlContactPoints(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float dot = Vector3.Dot(normal, Vector3.up);
            if (dot > 0.5f)
                _grounded = true;
        }
    }


    public bool PlayerIsGrounded()
    {
        return _grounded;
    }


    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    protected void Update()
    {
        if (_grounded && _shouldJump)
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpForce, _rigidbody.velocity.z);
        //Debug.Log($"Current player position: {transform.position} Frame number: {Time.frameCount}");
    }


    protected void FixedUpdate()
    {
        _finalVelocity = CalculateRelativeVelocity();
        _finalVelocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = _finalVelocity;
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
