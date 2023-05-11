using System.Collections.Generic;
using UnityEngine;

namespace Avatar 
{
    public class MovementController
    {
        private StateAutomatRestricted _state;
        private StatsProvider _stats;

        private Rigidbody _rigidbody;

        private Action _currentMove;

        private Dictionary<MovementType, Action> _moveSet;

        private Vector3 _forwardDirection;
        private Vector3 _lockedDirection;


        public MovementController(Rigidbody rigidbody, StateAutomatRestricted state, StatsProvider stats)
        {
            _rigidbody = rigidbody;
            _state = state;
            _stats = stats;
            FillMoveSet();
        }


        private void FillMoveSet()
        {
            _moveSet = new Dictionary<MovementType, Action>
            {
                { MovementType.Run, Run },
                { MovementType.Fly, Fly },
                { MovementType.RunOnSlope, RunOnSlope },
                { MovementType.Jump, Jump},
                { MovementType.Dash, Dash},
                { MovementType.Idle, Idle}
            };
        }

        public void Move()
        {
            HandleMoveType();
            _currentMove = _moveSet[_state.CurrentMoveState];
            if (_state.WasMoveStateChanged)
                Debug.Log(_state.CurrentMoveState);
            _currentMove.Invoke();
        }
        
        protected void HandleMoveType() 
        {
            if (_state.CurrentMoveState == MovementType.Fly && _state.WasMoveStateChanged)
                _forwardDirection = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            if (_state.CurrentMoveState == MovementType.Dash && _state.WasMoveStateChanged)
                _lockedDirection = _rigidbody.transform.TransformVector(_state.MoveDirection);
            if (_state.CurrentMoveState == MovementType.Jump && _state.WasMoveStateChanged)
            {
                _lockedDirection = _rigidbody.transform.TransformVector(_state.MoveDirection);
                _lockedDirection.y = 1f;
                _lockedDirection = _lockedDirection.normalized;
            }
        }

        private void Run()
        {
            _rigidbody.velocity = _rigidbody.transform.TransformVector(_state.MoveDirection) * _stats.RunSpeed;
        }

        private void Fly()
        {
            var newDirection = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            var angle = Vector3.SignedAngle(_forwardDirection, newDirection, Vector3.up);
            var newVelocity = Quaternion.Euler(0f, angle, 0f) * _rigidbody.velocity;

            newVelocity = newVelocity.magnitude > _stats.FlySpeedLimit ? newVelocity.normalized * _stats.FlySpeedLimit : newVelocity;
            _rigidbody.velocity = newVelocity;
            _forwardDirection = newDirection;
        }


        private void RunOnSlope()
        {
            var direction = _rigidbody.transform.TransformVector(_state.MoveDirection);
            _rigidbody.velocity = Vector3.ProjectOnPlane(direction, _state.Normal).normalized * _stats.RunSpeed;
        }

        private void Jump()
        {
            _rigidbody.velocity = _lockedDirection * _stats.JumpForce;
            Debug.Log($"LockedDirection - {_lockedDirection} DashForce - {_stats.JumpForce} Velocity - {_rigidbody.velocity}");
        }

        private void Dash()
        {            
            _rigidbody.velocity = _lockedDirection * _stats.DashForce;
            Debug.Log($"LockedDirection - {_lockedDirection} DashForce - {_stats.DashForce} Velocity - {_rigidbody.velocity}");
        }

        private void Idle() 
        { 
            _rigidbody.velocity = Vector3.zero;
        }


        private delegate void Action();
    }
}


