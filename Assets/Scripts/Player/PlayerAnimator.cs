using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private float SmoothAnimationSpeed = 50f;

    private bool _isRunning;
    private bool _shouldJump;
    private bool _isGrounded;

    private int _isRunningHash;
    private int _isJumpingHash;
    private int _isGroundedHash;
    private int _xBlendVelocityHash;
    private int _yBlendVelocityHash;

    private float _xBlendVelocity;
    private float _yBlendVelocity;

    private Animator _animator;

    #region PLAYERANIMATOR_METHODS
    protected void InterpolateBlendTreeVelocity(Vector3 moveDirection)
    {
        if (!_isRunning)
        {
            if (_xBlendVelocity > 0 || _yBlendVelocity > 0)
                _xBlendVelocity = _yBlendVelocity = 0;
            return;
        }

        if (_xBlendVelocity == 0 && _yBlendVelocity == 0)
        {
            _xBlendVelocity = moveDirection.x;
            _yBlendVelocity = moveDirection.y;
        }
        else
        {
            _xBlendVelocity = Mathf.Lerp(_xBlendVelocity, moveDirection.x, SmoothAnimationSpeed * Time.deltaTime);
            _yBlendVelocity = Mathf.Lerp(_yBlendVelocity, moveDirection.z, SmoothAnimationSpeed * Time.deltaTime);
        }
    }


    protected void GetAnimatorVariablesHashes()
    {
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isGroundedHash = Animator.StringToHash("isGrounded");
        _xBlendVelocityHash = Animator.StringToHash("xVelocity");
        _yBlendVelocityHash = Animator.StringToHash("yVelocity");
    }


    protected void ChangeAnimationState()
    {
        _animator.SetBool(_isRunningHash, _isRunning);
        _animator.SetBool(_isJumpingHash, _shouldJump);
        _animator.SetBool(_isGroundedHash, _isGrounded);
        _animator.SetFloat(_xBlendVelocityHash, _xBlendVelocity);
        _animator.SetFloat(_yBlendVelocityHash, _yBlendVelocity);
    }


    public void Animate(bool isRunning, bool shouldJump, bool isGrounded, Vector3 moveDirection)
    {
        _isRunning = isRunning;
        _shouldJump = shouldJump;
        _isGrounded = isGrounded;
        InterpolateBlendTreeVelocity(moveDirection);
        ChangeAnimationState();    
    }
    #endregion

    #region MONOBEHAVIOUR_METHODS
    protected void Start()
    {
        _animator = GetComponent<Animator>();
        GetAnimatorVariablesHashes();
    }
    #endregion
}
