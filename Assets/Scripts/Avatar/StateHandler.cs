using UnityEngine;
using System;
using UnityEngine.Events;
using Microsoft.SqlServer.Server;
using UnityEngine.EventSystems;

namespace Avatar
{
    public class StateHandler
    {
        private Rigidbody _rigidbody;
        private ConstantForce _constantForce;
        private StateAutomatRestricted _state;
        private Stats _analyst;

        #region METHODS
        public StateHandler(Rigidbody rigidbody, StateAutomatRestricted state, Stats analyst)
        {
            _rigidbody = rigidbody;
            _state = state;
            _analyst = analyst;
            _constantForce = _rigidbody.GetComponent<ConstantForce>();
        }

        public void HandleState()
        {
            HandleVerticalForces();
            UpdateStateStats();
        }

        private void HandleVerticalForces()
        {
            if((_state.CurrentMoveState == MovementType.RunOnSlope || _state.CurrentMoveState == MovementType.Idle))
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
            if (_state.CurrentMoveState == MovementType.Jump && _state.WasMoveStateChanged)
                _analyst.ReduceJumpCharges(1);
            if (_state.Grounded && _state.CurrentMoveState != MovementType.Jump)
                _analyst.ResetJumpCharges();
        }
        #endregion
    }
}
