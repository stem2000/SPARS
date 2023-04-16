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
            _rigidbody = GetComponent<Rigidbody>();   
            InitializeComponents();
        }


        private void InitializeComponents()
        {
            FillMoveSet();

        }


        private void FillMoveSet()
        {
            _moveSet = new Dictionary<MovementType, PerformMove>();
            _moveSet.Add(MovementType.Run, Run);
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
    }

    delegate void PerformMove(MovementData data);
}


