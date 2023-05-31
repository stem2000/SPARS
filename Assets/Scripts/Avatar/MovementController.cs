using System.Collections.Generic;
using UnityEngine;

namespace Avatar 
{
    public class MovementController
    {
        private StateInfoProvider _stateInfoProvider;
        private StatsProvider _statsProvider;

        private Rigidbody _rigidbody;

        private Action _currentMove;

        private Dictionary<MovementType, Action> _moveSet;

        private Vector3 _forwardDirection;
        private Vector3 _lockedDirection;


        public MovementController(Rigidbody rigidbody, StateInfoProvider stateInfoProvider, StatsProvider statsProvider)
        {
            _rigidbody = rigidbody;
            _stateInfoProvider = stateInfoProvider;
            _statsProvider = statsProvider;
            FillMoveSet();
        }

        public void Move()
        {
            _currentMove = _moveSet[_stateInfoProvider.CurrentMoveState];
            _currentMove.Invoke();
        }

        public void UpdateDirections()
        {
            if (_stateInfoProvider.CurrentMoveState == MovementType.Fly && _stateInfoProvider.WasMoveStateChanged)
                _forwardDirection = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            if (_stateInfoProvider.CurrentMoveState == MovementType.Dash && _stateInfoProvider.WasMoveStateChanged)
                _lockedDirection = _rigidbody.transform.TransformVector(_stateInfoProvider.MoveDirection);
            if (_stateInfoProvider.CurrentMoveState == MovementType.Jump && _stateInfoProvider.WasMoveStateChanged)
            {
                _lockedDirection = _rigidbody.transform.TransformVector(_stateInfoProvider.MoveDirection);
                _lockedDirection.y = 1f;
                _lockedDirection = _lockedDirection.normalized;
            }
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

        private void Run()
        {
            _rigidbody.velocity = _rigidbody.transform.TransformVector(_stateInfoProvider.MoveDirection) * _statsProvider.RunSpeed;
        }

        private void Fly()
        {
            var newDirection = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            var angle = Vector3.SignedAngle(_forwardDirection, newDirection, Vector3.up);
            var newVelocity = Quaternion.Euler(0f, angle, 0f) * _rigidbody.velocity;

            newVelocity = newVelocity.magnitude > _statsProvider.FlySpeedLimit ? newVelocity.normalized * _statsProvider.FlySpeedLimit : newVelocity;

            _rigidbody.velocity = newVelocity;
            _forwardDirection = newDirection;
        }


        private void RunOnSlope()
        {
            var direction = _rigidbody.transform.TransformVector(_stateInfoProvider.MoveDirection);
            _rigidbody.velocity = Vector3.ProjectOnPlane(direction, _stateInfoProvider.Normal).normalized * _statsProvider.RunSpeed;
        }

        private void Jump()
        {
            _rigidbody.velocity = _lockedDirection * _statsProvider.JumpForce;
        }

        private void Dash()
        {            
            _rigidbody.velocity = _lockedDirection * _statsProvider.DashForce;
        }

        private void Idle() 
        { 
            _rigidbody.velocity = Vector3.zero;
        }


        private delegate void Action();
    }
}


