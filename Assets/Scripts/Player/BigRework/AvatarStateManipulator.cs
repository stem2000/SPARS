using UnityEngine;

namespace AvatarModel
{
    public class AvatarStateManipulator
    {
        private Rigidbody _rigidbody;
        private ConstantForce _constantForce;
        private AvatarStats _immutableStats;
        private AvatarStats _avatarStats;

        private StateInfoPackage _packageFromState;

        public AvatarStateManipulator(Rigidbody rigidbody, AvatarStats stats)
        {
            _rigidbody = rigidbody;
            _immutableStats = stats.Clone();
            _avatarStats = stats;
            _constantForce = _rigidbody.GetComponent<ConstantForce>();
        }

        public void UpdateAvatarState(StateInfoPackage package)
        {
            _packageFromState = package;

            HandleVerticalForces();
            UpdateStateStats();
        }

        private void HandleVerticalForces()
        {
            if(_packageFromState.CurrentMoveState == MovementType.RunOnSlope && _packageFromState.StateWasChanged)
            {
                _rigidbody.useGravity = false;
                _constantForce.enabled = false;
            }
            else if(_packageFromState.CurrentMoveState != MovementType.RunOnSlope && _constantForce.enabled == false)
            {
                _rigidbody.useGravity = true;
                _constantForce.enabled = true;
            }
        }

        public void UpdateStateStats()
        {
            if(!(_avatarStats.JumpCharges > 0) && _packageFromState.Grounded)
            {
                _avatarStats.JumpCharges = _immutableStats.JumpCharges;
            }
        }
    }

}
