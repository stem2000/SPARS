using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace AvatarModel
{
    public class AvatarState : MonoBehaviour
    {
        [SerializeField] protected AvatarStats StaticAvatarStats;
        public AvatarStats DynamicAvatarStats;

        public Vector2 Rotation;

        public ShouldMove ShouldMove;
        public ShouldAttack ShouldAttack;

        private Rigidbody Rigidbody;
        private GameObject _avatar;

        private Vector3 _lockedVelocity = Vector3.zero;
        private Vector3 _flyDirection = Vector3.zero;
        private Vector3 _touchSurfaceNormal = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;

        public Vector2 MoveDirection { get { return MoveDirection; } set { _moveDirection = ConvertDirectionInput(value); } }

        public bool Grounded;
        public bool OnSlope;
        protected bool DashLocked;
        protected bool InDash;
        protected bool InJump;

        public IEnumerator RecalculateCoyoteTime()
        {
            if (DynamicAvatarStats.CoyoteTime != 0)
                yield return new WaitForSeconds(DynamicAvatarStats.CoyoteTime);
            DynamicAvatarStats.CoyoteTime = 0f;
        }

        public void ResetCoyoteTime()
        {

        }


        private Vector3 ConvertDirectionInput(Vector2 moveDirection)
        {
            return new Vector3(moveDirection.x, 0f, moveDirection.y);
        } 


        public AvatarState(GameObject avatar, AvatarStats avatarStats)
        {
            _avatar = avatar;
            DynamicAvatarStats = avatarStats.Clone();

            Rigidbody = _avatar.GetComponent<Rigidbody>();
            ShouldAttack = new ShouldAttack();
            ShouldMove = new ShouldMove();
        }
        
        

        public MovementData TransferMovementData()
        {
            switch (StateMachine.CurrentStateType)
            {
                case MovementType.Run: return GetRunData();
                case MovementType.Fly: return GetFlyData();
                case MovementType.RunOnSlope: return GetRunOnSlopeData();
                case MovementType.Jump: return GetJumpData();
                default: return GetRunData();
            }
        }

        private RunData GetRunData()
        {
            var convertedVector = new Vector3(MoveDirection.x, 0f, MoveDirection.y);
            return new RunData(_avatar.transform.TransformVector(convertedVector) * DynamicAvatarStats.RunSpeed);
        }


        private FlyData GetFlyData()
        {
            _flyDirection = Vector3.ProjectOnPlane(_avatar.transform.forward, Vector3.up).normalized;
            return new FlyData(_flyDirection, DynamicAvatarStats.FlySpeedLimit);
        }


        private RunOnSlopeData GetRunOnSlopeData()
        {
            var convertedVector = _avatar.transform.TransformVector();
            convertedVector = Vector3.ProjectOnPlane(convertedVector, _touchSurfaceNormal).normalized * DynamicAvatarStats.RunSpeed;
            return new RunOnSlopeData(convertedVector);
        }


        protected bool SemaphoreRun()
        {
            return Grounded;
        }


        protected void DonOnStartRun()
        {

        }

        private class StateMachine
        {
            protected State _currentState;
            protected Dictionary<MovementType, State> _moveStatesSet;


            public MovementType CurrentMoveState { get { return CurrentMoveState; } protected set { CurrentMoveState = value; } }


            protected void InitializeStatesSets();


            public StateMachine()
            {
                _moveStatesSet = new Dictionary<MovementType, State>();
                InitializeStatesSets();
                _currentState = GetLowestLayerState();
            }


            public void CalculateCurrentState(Dictionary<MovementType, State> _stateSet)
            {
                foreach (var pair in _stateSet)
                {
                    if (pair.Value.Semaphore() && pair.Value.Layer > _currentState.Layer)
                    {
                        _currentState = pair.Value;
                        CurrentStateType = pair.Key;
                    }
                }
                _currentState.DoOnStart();
            }


            public State GetLowestLayerState()
            {
                var state = _stateSet.FirstOrDefault().Value;

                foreach (var stateType in Enum.GetValues(typeof(StateTypesEnum)))
                {
                    if(state.Layer > _stateSet[(StateTypesEnum)stateType].Layer)
                        state = _stateSet[(StateTypesEnum)stateType];
                } 
                return state;
            }


            public void BlockStateMachine()
            {
                _currentState.SetMaxLayer();
            }
        }


        private class State
        {
            private StateLayers _layer;
            public StateLayers Layer { get { return _layer; } protected set { _layer = value; } }

            public State(StateLayers layer, SemaphoreDelegate semaphore, DoOnStartDelegate doOnStart)
            {
                _layer = layer;
                Semaphore = semaphore;
                DoOnStart = doOnStart;
            }

            public SemaphoreDelegate Semaphore;
            public DoOnStartDelegate DoOnStart;

            public void SetMaxLayer()
            {
                Layer = StateLayers.AutomaticTransitionForbidden;
            }


            public enum StateLayers
            {
                Layer1, Layer2, Layer3, Layer4, Layer5, Layer6, Layer7, Layer8, Layer9, Layer10,

                AutomaticTransitionForbidden = Int32.MaxValue
            }

            public delegate bool SemaphoreDelegate();
            public delegate void DoOnStartDelegate();
        }
    }


    public class ShouldMove
    {
        public bool ShouldDash;
        public bool ShouldJump;
    }


    public class ShouldAttack
    {
        public bool ShouldShoot;
    }


    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope
    }


    public abstract class MovementData { }
    public class RunData: MovementData
    {
        public Vector3 velocity;

        public RunData(Vector3 velocity)
        {
            this.velocity = velocity;
        }
    }
    public class JumpData : MovementData { }
    public class DashData : MovementData { }
    public class FlyData : MovementData
    {
        public Vector3 Direction;
        public float SpeedLimit;

        public FlyData(Vector3 direction, float speedLimit)
        {
            Direction = direction;
            SpeedLimit = speedLimit;
        }
    }
    public class RunOnSlopeData : MovementData 
    {
        public Vector3 velocity;

        public RunOnSlopeData(Vector3 velocity)
        {
            this.velocity = velocity;
        }
    }
}
