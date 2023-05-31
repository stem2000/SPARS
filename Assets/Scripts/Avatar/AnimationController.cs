using System;
using UnityEngine;

namespace Avatar
{
    [Serializable]
    public class AnimationController
    {
        private int _isRunningHash;
        private int _isJumpingHash;
        private int _isFallingHash;
        private int _isIdleHash;
        private int _xBlendVelocityHash;
        private int _yBlendVelocityHash;

        private int _handsRunHash;
        private int _handsFlyHash;
        private int _handsJumpHash;
        private int _handsIdleHash;

        [SerializeField] private Animator _avatarAnimator;
        [SerializeField] private Animator _handsAnimator;

        private StateInfoProvider _stateInfoProvider;

        private void MoveAnimationSwitch(MovementType type, bool value)
        {
            switch (type)
            {
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

        public void Initialize(StateInfoProvider state)
        {
            _stateInfoProvider = state;

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

        public void SwitchAnimation()
        {
            MoveAnimationSwitch(_stateInfoProvider.PreviousMoveState, false);
            MoveAnimationSwitch(_stateInfoProvider.CurrentMoveState, true);
        }

        public void UpdateAnimation()
        {
            _avatarAnimator.SetFloat(_xBlendVelocityHash, _stateInfoProvider.MoveDirection.x);
            _avatarAnimator.SetFloat(_yBlendVelocityHash, _stateInfoProvider.MoveDirection.z);
        }

        private void SwitchJumpAnimations(bool value)
        {
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
            _handsAnimator.SetBool(_handsRunHash, value);
        }

    }

}
