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

        private float _xBlendVelocity;
        private float _yBlendVelocity;

        private int _handsRunHash;
        private int _handsFlyHash;
        private int _handsJumpHash;
        private int _handsIdleHash;

        [SerializeField] private Animator _avatarAnimator;
        [SerializeField] private Animator _handsAnimator;

        private StateAutomatRestricted _state;

        private MovementType _lastMove;

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

        public void Initialize(StateAutomatRestricted state)
        {
            _state = state;

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

        public void ChangeAnimationState()
        {
            _xBlendVelocity = _state.MoveDirection.x;
            _yBlendVelocity = _state.MoveDirection.z;

            if (_state.WasMoveStateChanged)
            {
                MoveAnimationSwitch(_lastMove, false);
                MoveAnimationSwitch(_state.CurrentMoveState, true);
                _lastMove = _state.CurrentMoveState;
            }
            else if (_state.WasAttackStateChanged)
            {

            }
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
            _avatarAnimator.SetFloat(_xBlendVelocityHash, _xBlendVelocity);
            _avatarAnimator.SetFloat(_yBlendVelocityHash, _yBlendVelocity);
        }
    }

}
