using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

namespace AvatarModel 
{
    public class AvatarMovement
    {
        private Rigidbody _rigidbody;
        private PerformMove _currentMove;
        private Dictionary<MovementType, PerformMove> _moveSet;

        private MovementData _currentMoveData;
        private MovementType _currentMoveType;

        private Vector3 _forwardDirection;
        private Vector3 _lockedDirection;


        public AvatarMovement(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            FillMoveSet();
        }


        private void FillMoveSet()
        {
            _moveSet = new Dictionary<MovementType, PerformMove>
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
            if(_currentMoveData != null)
            {
                DefineCurrentMove();
                _currentMove.Invoke(_currentMoveData);
            }
        }


        public void UpdateMovementData(MovementDataPackage package)
        {
            _currentMoveType = package.type;
            _currentMoveData = package.data;
            HandleMoveType(package.stateChanged);
        }
        
        protected void HandleMoveType(bool stateChanged) 
        {
            if (_currentMoveType == MovementType.Fly && stateChanged)
                _forwardDirection = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            if (_currentMoveType == MovementType.Dash && stateChanged)
                _lockedDirection = _rigidbody.transform.TransformVector(((DashData)_currentMoveData).direction);
            if (_currentMoveType == MovementType.Jump && stateChanged)
            {
                _lockedDirection = _rigidbody.transform.TransformVector(((JumpData)_currentMoveData).direction);
                _lockedDirection.y = 1f;
                _lockedDirection = _lockedDirection.normalized;
            }
        }

        protected void DefineCurrentMove()
        {
            _currentMove = _moveSet[_currentMoveType];
        }


        private void Run(MovementData data)
        {
            var runData = (RunData)data;
            _rigidbody.velocity = _rigidbody.transform.TransformVector(runData.direction) * runData.speed;
        }


        private void Fly(MovementData data)
        {
            var flyData = (FlyData)data;

            var newDirection = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            var speedLimit = flyData.speedLimit;

            var angle = Vector3.SignedAngle(_forwardDirection, newDirection, Vector3.up);
            var newVelocity = Quaternion.Euler(0f, angle, 0f) * _rigidbody.velocity;

            newVelocity = newVelocity.magnitude > speedLimit ? newVelocity.normalized * speedLimit : newVelocity;
            _rigidbody.velocity = newVelocity;

            _forwardDirection = newDirection;
        }


        private void RunOnSlope(MovementData data)
        {
            var runSData = (RunOnSlopeData)data;

            var speed = runSData.speed;
            var direction = _rigidbody.transform.TransformVector(runSData.direction);
            var normal = runSData.normal;
            var velocity = Vector3.ProjectOnPlane(direction, normal).normalized * speed;

            _rigidbody.velocity = velocity;
        }

        private void Jump(MovementData data)
        {
            var jumpData = (JumpData)data;

            var force = jumpData.force;

            _rigidbody.velocity = _lockedDirection * force;
        }

        private void Dash(MovementData data)
        {
            var dashData = (DashData)data;

            var force = dashData.force;

            _rigidbody.velocity = _lockedDirection * force;
        }

        private void Idle(MovementData data) 
        { 
            _rigidbody.velocity = Vector3.zero;
        }


        delegate void PerformMove(MovementData data);
    }
}


