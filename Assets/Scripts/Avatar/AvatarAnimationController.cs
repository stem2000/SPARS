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

    private int _handsRunHash;
    private int _handsFlyHash;
    private int _handsJumpHash;
    private int _handsIdleHash;

    [SerializeField] private Animator _avatarAnimator;
    [SerializeField] private Animator _handsAnimator;

    private MovementType _lastMove;

    private void MoveAnimationSwitch(MovementType type, bool value)
    {
        switch (type) {
            case MovementType.Jump:
                SwitchJumpAnimations(value);
                break;
            case MovementType.Run:
            case MovementType.RunOnSlope:
            case MovementType.Dash:
                SwitchRunAnimations(value);
                break;
            case MovementType.Fly:
                SwitchFlyAnimations(value);
                break;
            case MovementType.Idle:
                SwitchIdleAnimations(value);
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

        _handsRunHash = Animator.StringToHash("HandsRun");
        _handsFlyHash = Animator.StringToHash("HandsFly");
        _handsJumpHash = Animator.StringToHash("HandsJump");
        _handsIdleHash = Animator.StringToHash("HandsIdle");
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

    private void SwitchJumpAnimations(bool value)
    {
        Debug.Log("Switch Jump");
        _avatarAnimator.SetBool(_isJumpingHash, value);
        _handsAnimator.SetBool(_handsJumpHash, value);
    }

    private void SwitchFlyAnimations(bool value)
    {
        _avatarAnimator.SetBool(_isFallingHash, value);
        _handsAnimator.SetBool(_handsFlyHash, value);
    }

    private void SwitchIdleAnimations(bool value)
    {
        _avatarAnimator.SetBool(_isIdleHash, value);
        _handsAnimator.SetBool(_handsIdleHash, value);
    }

    private void SwitchRunAnimations(bool value)
    {
        _avatarAnimator.SetBool(_isRunningHash, value);
        _avatarAnimator.SetFloat(_xBlendVelocityHash, _xBlendVelocity);
        _avatarAnimator.SetFloat(_yBlendVelocityHash, _yBlendVelocity);
        _handsAnimator.SetBool(_handsRunHash, value);
    }
}
