using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel 
{
    public class AvatarMovement : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private MovementData _currentMoveData;
        private MovementType _currentMovetType;
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
            _moveSet = new Dictionary<MovementType, PerformMove>();
            _moveSet.Add(MovementType.Run, Run);
            _moveSet.Add(MovementType.Fly, Fly);
            _moveSet.Add(MovementType.RunOnSlope, RunOnSlope);
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
            _currentMovetType = type;
            _currentMoveData = data;
        }
        

        protected void DefineCurrentMove()
        {
            _currentMove = _moveSet[_currentMovetType];
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
            _rigidbody.velocity = ((RunOnSlopeData)data).velocity;
        }

        delegate void PerformMove(MovementData data);
    }
}


