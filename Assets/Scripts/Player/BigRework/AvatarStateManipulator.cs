using UnityEngine;

namespace AvatarModel
{
    public class AvatarStateManipulator
    {
        private Rigidbody _rigidbody;
        private ConstantForce _constantForce;
        private AvatarStats _immutableStats;

        private StateInfoPackage _packageFromState;
        private StateStatsUpdatePackage _statsPackage;

        public AvatarStateManipulator(Rigidbody rigidbody, AvatarStats stats)
        {
            _rigidbody = rigidbody;
            _immutableStats = stats.Clone();
            _statsPackage = new StateStatsUpdatePackage(stats);
            _constantForce = _rigidbody.GetComponent<ConstantForce>();
        }

        public void UpdateAvatarState(StateInfoPackage package)
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

        private void UpdateStateStats()
        {
            UpdateJumpStats();
        }

        private void UpdateJumpStats()
        {
            if (_packageFromState.CurrentMoveState == MovementType.Jump && _packageFromState.StateWasChanged)
                _statsPackage.jumpStats.JumpCharges--;
            if (_packageFromState.Grounded && _packageFromState.CurrentMoveState != MovementType.Jump)
                _statsPackage.jumpStats.JumpCharges = _immutableStats.jumpStats.JumpCharges;
            //Debug.Log($"{_packageFromState.Grounded} - {_packageFromState.CurrentMoveState} - {_statsPackage.jumpStats.JumpCharges}");
        }      
    }

}
