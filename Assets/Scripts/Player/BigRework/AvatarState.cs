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
        [SerializeField] protected AvatarStats ImmutableStats;
        public AvatarStats MutableStats;

        [HideInInspector] public Vector2 Rotation;
        [HideInInspector] public Vector3 Normal = Vector3.zero;


        public ShouldMove ShouldMove;
        public ShouldAttack ShouldAttack;

        private Rigidbody Rigidbody;
        private readonly StateMachine _stateMachine = new StateMachine();

        private Vector3 _lockedDirection = Vector3.zero;
        private Vector3 _flyDirection = Vector3.zero;
        private Vector3 _touchSurfaceNormal = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;

        [HideInInspector] public bool Grounded;
        [HideInInspector] public bool OnSlope;
        protected bool DashLocked;
        protected bool InDash;
        protected bool InJump;

        public Vector2 MoveDirection { set { _moveDirection = ConvertDirectionInput(value); } }


        private Vector3 ConvertDirectionInput(Vector2 moveDirection)
        {
            return new Vector3(moveDirection.x, 0f, moveDirection.y);
        } 


        protected void Start()
        {
            MutableStats = ImmutableStats.Clone();
            Rigidbody = GetComponent<Rigidbody>();
            ShouldAttack = new ShouldAttack();
            ShouldMove = new ShouldMove();
        }


        public MovementData GetMovementData()
        {
            switch (_stateMachine.CurrentMoveType)
            {
                case MovementType.Run: return GetRunData();
                case MovementType.Fly: return GetFlyData();
                case MovementType.RunOnSlope: return GetRunOnSlopeData();
                case MovementType.Jump: return GetJumpData();
                default: return GetRunData();
            }
        }


        public MovementType GetMoveStateType()
        {
            return _stateMachine.CurrentMoveType;
        }


        private RunData GetRunData()
        {
            return new RunData(transform.TransformVector(_moveDirection) * MutableStats.RunSpeed);
        }


        private FlyData GetFlyData()
        {
            _flyDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            return new FlyData(_flyDirection, MutableStats.FlySpeedLimit);
        }


        private RunOnSlopeData GetRunOnSlopeData()
        {
            return new RunOnSlopeData(transform.TransformVector(_moveDirection), MutableStats.RunSpeed, Normal);
        }


        private JumpData GetJumpData()
        {
            return new JumpData(transform.TransformVector(_moveDirection), MutableStats.JumpForce);
        }


        protected bool SemaphoreRun()
        {
            return Grounded;
        }


        protected void DonOnStartRun()
        {
            Rigidbody.useGravity = true;
        }


        protected bool SemaphoreRunOnSlope()
        {
            return Grounded && OnSlope;
        }


        protected void DoOnStartRunOnSlope()
        {
            Rigidbody.useGravity = false;
        }


        protected bool SemaphoreFly()
        {
            return !Grounded;
        }


        protected void DonOnStartFly()
        {
            Rigidbody.useGravity = false;
        }


        protected bool SemaphoreJump()
        {
            return MutableStats.JumpCharges > 0 && ShouldMove.ShouldJump;
        }


        protected void DonOnStartJump()
        {
            Rigidbody.useGravity = true;
            _lockedDirection = _moveDirection;
            StartCoroutine(JumpDuration());
        }


        public IEnumerator JumpDuration()
        {
            _stateMachine._currentMoveState.SetMaxLayer();
            yield return new WaitForSeconds(MutableStats.JumpDuration);
            _stateMachine._currentMoveState.SetMinLayer();
        }


        private class StateMachine
        {
            private AvatarState Avatar;

            private Dictionary<MovementType, State<MovementType>> _moveStatesSet;
            private State<MovementType> _previousMoveState = null;
            public State<MovementType> _currentMoveState = null;


            public MovementType CurrentMoveType { get { return CurrentMoveType; } protected set { CurrentMoveType = value; } }


            private void InitializeStatesSets()
            {
                InitializeMoveState();
            } 


            private void InitializeMoveState()
            {
                _moveStatesSet.Add(MovementType.Run, 
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer1, 
                                            Avatar.SemaphoreRun, 
                                            Avatar.DonOnStartRun, 
                                            MovementType.Run));
                _moveStatesSet.Add(MovementType.RunOnSlope, 
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer2,
                                            Avatar.SemaphoreRunOnSlope, 
                                            Avatar.DoOnStartRunOnSlope,
                                            MovementType.RunOnSlope));
                _moveStatesSet.Add(MovementType.Fly, 
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer3,
                                            Avatar.SemaphoreFly,
                                            Avatar.DonOnStartFly,
                                            MovementType.Fly));
                _moveStatesSet.Add(MovementType.Jump,
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer4,
                                            Avatar.SemaphoreJump,
                                            Avatar.DonOnStartJump,
                                            MovementType.Jump));
            }


            public StateMachine()
            {
                _moveStatesSet = new Dictionary<MovementType, State<MovementType>>();
                InitializeStatesSets();
                _currentMoveState = GetLowestLayerState(_moveStatesSet);
            }


            public void CalculateCurrentState<EnumType>(Dictionary<EnumType, State<EnumType>> stateSet, State<EnumType> currentState, State<EnumType> nextState)
            {
                _previousMoveState = _currentMoveState;
                foreach (var pair in stateSet)
                {
                    if (pair.Value.Semaphore() && pair.Value.Layer > currentState.Layer)
                    {
                        currentState = pair.Value;
                    }
                }
                currentState.DoOnStart();
            }


            public State<EnumType> GetLowestLayerState<EnumType>(Dictionary<EnumType, State<EnumType>> stateSet)
            {
                var state = stateSet.FirstOrDefault().Value;

                foreach (var stateType in Enum.GetValues(typeof(EnumType)))
                {
                    if(state.Layer > stateSet[(EnumType)stateType].Layer)
                        state = stateSet[(EnumType)stateType];
                } 
                return state;
            }


            public void BlockStateMachine()
            {
                _currentMoveState.SetMaxLayer();
            }
        }


        private class State<EnumType>
        {
            private StateLayers _layer;
            public StateLayers Layer { get { return _layer; } protected set { _layer = value; } }
            public EnumType CurrentStateType;

            public State(StateLayers layer, SemaphoreDelegate semaphore, DoOnStartDelegate doOnStart, EnumType CurrentStateType)
            {
                _layer = layer;
                Semaphore = semaphore;
                DoOnStart = doOnStart;
                this.CurrentStateType = CurrentStateType;
            }

            public SemaphoreDelegate Semaphore;
            public DoOnStartDelegate DoOnStart;


            public void SetMaxLayer()
            {
                Layer = StateLayers.AutomaticTransitionForbidden;
            }


            public void SetMinLayer()
            {
                Layer = StateLayers.Layer1;
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
    public class JumpData : MovementData 
    { 
        public Vector3 direction;
        public float force;

        public JumpData(Vector3 direction, float force)
        {
            this.direction = direction;
            this.force = force;
        }
    }
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
        public Vector3 direction;
        public Vector3 normal;
        public float speed;
        public RunOnSlopeData(Vector3 direction, float speed, Vector3 normal)
        {
            this.direction = direction;
            this.speed = speed;
            this.normal = normal;
        }
    }
}
