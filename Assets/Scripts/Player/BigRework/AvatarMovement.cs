using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel 
{
    public class AvatarMovement
    {
        private Rigidbody _rigidbody;
        private MovementData _currentMoveData;
        private MovementType _currentMoveType;
        private PerformMove _currentMove;
        private Dictionary<MovementType, PerformMove> _moveSet;



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
                { MovementType.Dash, Dash}
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


        public void ReceiveMovementData(MovementType type, MovementData data)
        {
            _currentMoveType = type;
            _currentMoveData = data;
            Move();
        }
        

        protected void DefineCurrentMove()
        {
            _currentMove = _moveSet[_currentMoveType];
            Debug.Log(_currentMoveType);
        }


        private void Run(MovementData data)
        {
            var runData = (RunData)data;
            _rigidbody.velocity = _rigidbody.transform.TransformVector(runData.direction) * runData.speed;
        }


        private void Fly(MovementData data)
        {
            var flyData = (FlyData)data;

            var direction = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            var speedLimit = flyData.speedLimit;

            var angle = Vector3.SignedAngle(direction, _rigidbody.transform.forward, Vector3.up);
            var newVelocity = Quaternion.Euler(0f, angle, 0f) * _rigidbody.velocity;

            newVelocity = newVelocity.magnitude > speedLimit ? newVelocity.normalized * speedLimit : newVelocity;
            _rigidbody.velocity = newVelocity;
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

            var direction = _rigidbody.transform.TransformVector(jumpData.direction);
            var force = jumpData.force;

            direction.y = 1f;
            direction = direction.normalized;

            _rigidbody.velocity = direction * force;
        }

        private void Dash(MovementData data)
        {
            var dashData = (DashData)data;

            var direction = _rigidbody.transform.TransformVector(dashData.direction);
            var force = dashData.force;

            _rigidbody.velocity = direction * force;
        }


        delegate void PerformMove(MovementData data);
    }
}


