using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel 
{
    public class AvatarMovement : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private MovementData _currentMoveData;
        private MovementType _currentMoveType;
        private PerformMove _currentMove;
        private Dictionary<MovementType, PerformMove> _moveSet;


        protected void Start()
        {  
            InitializeComponents();
        }


        private void InitializeComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();
            FillMoveSet();
        }


        private void FillMoveSet()
        {
            _moveSet = new Dictionary<MovementType, PerformMove>
            {
                { MovementType.Run, Run },
                { MovementType.Fly, Fly },
                { MovementType.RunOnSlope, RunOnSlope },
                { MovementType.Jump, Jump}
            };
        }


        protected void FixedUpdate()
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
        }
        

        protected void DefineCurrentMove()
        {
            _currentMove = _moveSet[_currentMoveType];
            Debug.Log(_currentMoveType);
        }


        private void Run(MovementData data)
        {
            _rigidbody.velocity = ((RunData)data).velocity;
        }


        private void Fly(MovementData data)
        {
            var direction = ((FlyData)data).Direction;
            var speedLimit = ((FlyData)data).SpeedLimit;

            var angle = Vector3.SignedAngle(direction, transform.forward, Vector3.up);
            var newVelocity = Quaternion.Euler(0f, angle, 0f) * _rigidbody.velocity;

            newVelocity = newVelocity.magnitude > speedLimit ? newVelocity.normalized * speedLimit : newVelocity;
            _rigidbody.velocity = newVelocity;
        }


        private void RunOnSlope(MovementData data)
        {
            var speed = ((RunOnSlopeData)data).speed;
            var direction = ((RunOnSlopeData)data).direction;
            var normal = ((RunOnSlopeData)data).normal;

            var velocity = Vector3.ProjectOnPlane(direction, normal).normalized * speed;
            _rigidbody.velocity = velocity;
        }


        private void Jump(MovementData data)
        {
            var direction = ((JumpData)data).direction;
            var force = ((JumpData)data).force;

            direction.y = 1f;
            direction = direction.normalized;

            _rigidbody.velocity = direction * force;
        }


        delegate void PerformMove(MovementData data);
    }
}


