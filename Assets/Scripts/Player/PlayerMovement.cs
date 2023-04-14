using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour, ActReceiver
{
    [SerializeField] private CapsuleCollider _playerCollider;
    [SerializeField] private SphereCollider _groundCheckCollider;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float JumpForce = 10;
    [SerializeField] private float _jumpDuration = 0.15f;
    [SerializeField] private float CoyoteTime = 0.2f;
    [SerializeField] private float DashForce = 10;
    [SerializeField] private float DashLockTime = 0.5f;
    [SerializeField] private float DashDuration = 0.25f;
    [SerializeField] private float AirDashSpeedFactor = 0.5f;
    [SerializeField] private float FlySpeedLimit = 9;

    private Vector3 _moveDirection;
    private Vector3 _DashDirection = Vector3.zero;
    private Vector3 _jumpDirection = Vector3.zero;
    private Vector3 _flyForwardDirection;

    private bool _grounded;
    private bool _inFly = false;
    private bool _inDash = false;
    private bool _applyJumpSpeed = false;

    private float _coyoteTimeCounter;
    private float _dashPauseCounter = 0;

    private Rigidbody _rigidbody;

    private RaycastHit _slopeHit;

    #region PROPERTIES
    public Vector3 MoveDirection 
    { 
        get { return _moveDirection;} 
        set { _moveDirection = value.normalized;} 
    }


    public bool JumpState
    {
        get { return _applyJumpSpeed; }
    }


    public bool DashState
    {
        get { return _inDash; }
    }


    public bool Grounded
    {
        get { return _grounded; }
    }


    public float Speed
    {
        get { return _speed; }
    }
    #endregion

    #region PLAYERMOVEMENT_METHODS

    public bool ReceiveAct(ActType act, bool shouldAct) 
    {
        switch (act)
        {
            case ActType.Jump:
                return HandleJumpInput(shouldAct);
            case ActType.Dash:
                return HandleDashInput(shouldAct);
            default:
                return false;
        }
    }


    public bool HandleDashInput(bool value)
    {
            if (_inDash != true && _dashPauseCounter <= 0)
            {
                _inDash = value;
                return value;
            }

            return false;
    }

    public bool HandleJumpInput(bool value)
    {
        if (_applyJumpSpeed != true && !_inDash && _grounded)
        {
            _applyJumpSpeed = value;
            return value;
        }

        return false;
    }


    protected Vector3 CalculateRelativeVelocity()
    {
        return  transform.TransformVector(_moveDirection) * _speed;
    }

    protected void ApplyVelocity()
    {
        if (_applyJumpSpeed && _coyoteTimeCounter > 0)
        {
            Jump();
        }
        else if (!_grounded && !_inDash)
        {
            FreeFlight();
        }
        else if (_inDash)
        {
            Dash();
        }
        else if (IsOnSlope() && _grounded)
        {
            Slope();
        }
        else
        {
            _rigidbody.velocity = CalculateRelativeVelocity();
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
        return Vector3.ProjectOnPlane(transform.TransformVector(_moveDirection), _slopeHit.normal).normalized;
    }


    private void Jump()
    {
        Vector3 velocityDirection;
        velocityDirection = _moveDirection.normalized;
        velocityDirection.y = 1f;
        _rigidbody.velocity = transform.TransformVector(velocityDirection.normalized) * JumpForce;
        Invoke(nameof(ResetJump), _jumpDuration);
    }


    private void ResetJump()
    {
        _applyJumpSpeed = false;
        _jumpDirection = Vector3.zero;
    }

    private void FreeFlight()
    {
        if (!_inFly)
        {
            _inFly = true;
            _flyForwardDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        }
        Vector3 forwardXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        var angle = Vector3.SignedAngle(_flyForwardDirection, transform.forward, Vector3.up);

        var newVelocity = Quaternion.Euler(0f, angle, 0f) * _rigidbody.velocity;
        newVelocity = newVelocity.magnitude > FlySpeedLimit ? newVelocity.normalized * FlySpeedLimit : newVelocity;
        _rigidbody.velocity = newVelocity;
        _flyForwardDirection = forwardXZ;
    }


    private void Slope()
    {
        Vector3 finalVelocity = Vector3.zero;
        finalVelocity = GetSlopeMoveDirection();
        finalVelocity *= _speed;
        _rigidbody.velocity = finalVelocity;
    }


    private void Dash()
    {
        if(_DashDirection == Vector3.zero)
            _DashDirection = _moveDirection.normalized;

        if(_grounded)
            _rigidbody.velocity = transform.TransformVector(_DashDirection) * DashForce;
        else 
            _rigidbody.velocity = transform.TransformVector(_DashDirection) * DashForce * AirDashSpeedFactor;

        _dashPauseCounter = DashLockTime;
        Invoke(nameof(ResetDash), DashDuration);
    }
    

    private void ResetDash() 
    { 
        _inDash = false;
        _DashDirection = Vector3.zero;
    }
    #endregion

    #region MONOVEHAVIOUR_METHODS
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
        if(_dashPauseCounter > 0)
        {
            _dashPauseCounter -= Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        _grounded = true;
    }


    private void OnTriggerStay(Collider other)
    {
        _grounded = true;
        _inFly = false;
        _coyoteTimeCounter = CoyoteTime;
    }

    private void OnTriggerExit(Collider other)
    {
        _grounded = false;
    }
    #endregion
}
