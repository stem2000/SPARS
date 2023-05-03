using AvatarModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarAnimationController
{
    private int _isRunningHash;
    private int _isJumpingHash;
    private int _isFallingHash;
    private int _isIdleHash;
    private int _xBlendVelocityHash;
    private int _yBlendVelocityHash;
    
    private float _xBlendVelocity;
    private float _yBlendVelocity;

    [SerializeField] private Animator _animator;

    private MovementType _lastMove;

    private void MoveAnimationSwitch(MovementType type, bool value)
    {
        switch (type) {
            case MovementType.Jump:
                _animator.SetBool(_isJumpingHash, value);
                _animator.SetBool(_isFallingHash, !value);
                break;
            case MovementType.Run:
            case MovementType.RunOnSlope:
            case MovementType.Dash:
                _animator.SetBool(_isRunningHash, value);
                _animator.SetFloat(_xBlendVelocityHash, _xBlendVelocity);
                _animator.SetFloat(_yBlendVelocityHash, _yBlendVelocity);
                break;
            case MovementType.Fly:
                _animator.SetBool(_isFallingHash, value);
                break;
            case MovementType.Idle:
                _animator.SetBool(_isIdleHash, value);
                break;
        }
    }

    public void SetAnimatorVariablesHashes()
    {
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isFallingHash = Animator.StringToHash("isFalling");
        _isIdleHash = Animator.StringToHash("isIdle");
        _xBlendVelocityHash = Animator.StringToHash("xVelocity");
        _yBlendVelocityHash = Animator.StringToHash("yVelocity");
    }

    public void ChangeAnimationState(StateChangingData stateInfo)
    {
        _xBlendVelocity = stateInfo.MoveDirection.x;
        _yBlendVelocity = stateInfo.MoveDirection.z;

        if (stateInfo.MoveStateWasChanged)
        {
            MoveAnimationSwitch(_lastMove, false);
            MoveAnimationSwitch(stateInfo.CurrentMoveType, true);
            _lastMove = stateInfo.CurrentMoveType;
        }
        else if (stateInfo.AttackStateWasChanged)
        {

        }
    }

}
