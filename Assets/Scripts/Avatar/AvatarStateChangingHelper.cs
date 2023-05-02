using UnityEngine;

namespace AvatarModel
{
    public class AvatarStateChangingHelper
    {
        private Rigidbody _rigidbody;
        private ConstantForce _constantForce;
        private AvatarStats _immutableStats;

        private StateFromInfoPackage _packageFromState;
        private StateStatsUpdatePackage _statsPackage;

        public AvatarStateChangingHelper(Rigidbody rigidbody, AvatarStats stats)
        {
            _rigidbody = rigidbody;
            _immutableStats = stats.Clone();
            _statsPackage = new StateStatsUpdatePackage(stats);
            _constantForce = _rigidbody.GetComponent<ConstantForce>();
        }

        public void ReactToStateChanging(in StateFromInfoPackage package)
        {
            _packageFromState = package;

            HandleVerticalForces();
            UpdateStateStats();
        }

        public StateStatsUpdatePackage GetStatsPackage()
        {
            return _statsPackage;
        }

        private void HandleVerticalForces()
        {
            if((_packageFromState.CurrentMoveType == MovementType.RunOnSlope || _packageFromState.CurrentMoveType == MovementType.Idle)
                && _packageFromState.MoveStateWasChanged)
            {
                _rigidbody.useGravity = false;
                _constantForce.enabled = false;
            }
            else if(_constantForce.enabled == false)
            {
                _rigidbody.useGravity = true;
                _constantForce.enabled = true;
            }
        }

        private void UpdateStateStats()
        {
            UpdateJumpStats();
        }

        private void UpdateJumpStats()
        {
            if (_packageFromState.CurrentMoveType == MovementType.Jump && _packageFromState.MoveStateWasChanged)
                _statsPackage.jumpStats.JumpCharges--;
            if (_packageFromState.Grounded && _packageFromState.CurrentMoveType != MovementType.Jump)
                _statsPackage.jumpStats.JumpCharges = _immutableStats.jumpStats.JumpCharges;
        }      
    }

}
