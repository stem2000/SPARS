using UnityEngine;

namespace AvatarModel
{
    public class StateChangingReactor
    {
        private Rigidbody _rigidbody;
        private ConstantForce _constantForce;
        private AvatarStats _immutableStats;

        private StateChangingData _packageFromState;
        private StatsPackage _statsPackage;

        public StateChangingReactor(Rigidbody rigidbody, AvatarStats stats)
        {
            _rigidbody = rigidbody;
            _immutableStats = stats.Clone();
            _statsPackage = new StatsPackage(stats);
            _constantForce = _rigidbody.GetComponent<ConstantForce>();
        }

        public void ReactToStateChanging(in StateChangingData package)
        {
            _packageFromState = package;

            HandleVerticalForces();
            UpdateStateStats();
        }

        public StatsPackage GetStatsPackage()
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
