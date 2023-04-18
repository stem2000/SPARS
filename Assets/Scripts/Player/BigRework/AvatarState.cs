using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.VersionControl.Asset;

namespace AvatarModel
{
    public class AvatarState : MonoBehaviour
    {
        [SerializeField] protected AvatarStats ImmutableStats;

        protected AvatarStats MutableStats;

        [HideInInspector] public Vector2 Rotation;


        private Rigidbody Rigidbody;
        private StateMachine _stateMachine;
        private ConstantForce ConstantForce_;

        private Vector3 _lockedDirection = Vector3.zero;
        private Vector3 _flyDirection = Vector3.zero;
        private Vector3 _normal = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;

        protected bool Grounded;
        protected bool OnSlope;
        protected bool ShouldDash;
        protected bool ShouldJump;
        protected bool ShouldShoot;

        protected bool InJump;

        protected int UsedJumpCharges = 0;


        private Vector3 ConvertDirectionInput(Vector2 moveDirection)
        {
            return new Vector3(moveDirection.x, 0f, moveDirection.y);
        } 


        protected void Start()
        {
            MutableStats = ImmutableStats.Clone();
            Rigidbody = GetComponent<Rigidbody>();
            _stateMachine = new StateMachine(this);
            ConstantForce_ = GetComponent<ConstantForce>();
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


        public void ChangeState(in StateChangesFlagsPackage package)
        {
            _moveDirection = ConvertDirectionInput(package.MoveDirection);
            _normal = package.Normal;
            Grounded = package.Grounded;
            OnSlope = package.OnSlope;
            ShouldDash = package.ShouldDash;
            ShouldJump = package.ShouldJump;
            ShouldShoot = package.ShouldShoot;
            _stateMachine.ChangeState();
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
            return new RunOnSlopeData(transform.TransformVector(_moveDirection), MutableStats.RunSpeed, _normal);
        }


        private JumpData GetJumpData()
        {
            return new JumpData(transform.TransformVector(_moveDirection), MutableStats.JumpForce);
        }


        protected bool SemaphoreRun()
        {
            return Grounded;
        }


        protected void DonOnEnterRun()
        {
            EnableYForces();
        }


        protected void DoOnExitRun()
        {

        }


        protected bool SemaphoreRunOnSlope()
        {
            return Grounded && OnSlope;
        }


        protected void DoOnEnterRunOnSlope()
        {
            DisableYForces();
        }


        protected void DoOnExitRunSlope()
        {
 
        }

        protected bool SemaphoreFly()
        {
            return !Grounded;
        }


        protected void DonOnEnterFly()
        {
            EnableYForces();
        }

        protected void DoOnExitFly()
        {

        }


        protected bool SemaphoreJump()
        {
            return MutableStats.JumpCharges > 0 && (ShouldJump || InJump);
        }


        protected void DonOnEnterJump()
        {
            EnableYForces();
            _lockedDirection = _moveDirection;
            InJump = true;
            StartCoroutine(JumpDuration());
        }


        protected void DoOnExitJump()
        {
        }


        public IEnumerator JumpDuration()
        {
            UsedJumpCharges++;
            MutableStats.JumpCharges--;

            yield return new WaitForSeconds(MutableStats.JumpDuration);
            
            UsedJumpCharges--;

            StartCoroutine(RestoreJumpCharge());

            if (!(UsedJumpCharges == ImmutableStats.JumpCharges))
                InJump = false;
        }


        public IEnumerator RestoreJumpCharge()
        {
            yield return new WaitForSeconds(MutableStats.JumpChargeResetTime);
            MutableStats.JumpCharges++;
        }


        public void DisableYForces() { Rigidbody.useGravity = false; ConstantForce_.enabled = false;}

        public void EnableYForces() { Rigidbody.useGravity = true; ConstantForce_.enabled = true; }

        private class StateMachine
        {
            private AvatarState Avatar;

            private Dictionary<MovementType, State<MovementType>> _moveStatesSet;
            private State<MovementType> _previousMoveState = null;
            public State<MovementType> _currentMoveState = null;


            public MovementType CurrentMoveType { get { return _currentMoveState.CurrentStateType; }}


            public StateMachine(AvatarState avatar)
            {
                Avatar = avatar;
                _moveStatesSet = new Dictionary<MovementType, State<MovementType>>();
                InitializeStatesSets();
            }


            private void InitializeStatesSets()
            {
                InitializeMoveState(); 
            } 


            private void InitializeMoveState()
            {
                var state = new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer1,
                                            Avatar.SemaphoreRun,
                                            Avatar.DonOnEnterRun,
                                            Avatar.DoOnExitRun,
                                            MovementType.Run);
                _moveStatesSet.Add(MovementType.Run, state
                                        );
                _moveStatesSet.Add(MovementType.RunOnSlope,
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer2,
                                            Avatar.SemaphoreRunOnSlope,
                                            Avatar.DoOnEnterRunOnSlope,
                                            Avatar.DoOnExitRunSlope,
                                            MovementType.RunOnSlope));
                _moveStatesSet.Add(MovementType.Fly,
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer3,
                                            Avatar.SemaphoreFly,
                                            Avatar.DonOnEnterFly,
                                            Avatar.DoOnExitFly,
                                            MovementType.Fly));
                _moveStatesSet.Add(MovementType.Jump,
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer4,
                                            Avatar.SemaphoreJump,
                                            Avatar.DonOnEnterJump,
                                            Avatar.DoOnExitJump,
                                            MovementType.Jump));
                _moveStatesSet.Add(MovementType.Dash,
                                        new State<MovementType>(
                                            State<MovementType>.StateLayers.Layer4,
                                            Avatar.SemaphoreJump,
                                            Avatar.DonOnEnterJump,
                                            Avatar.DoOnExitJump,
                                            MovementType.Jump));

                _currentMoveState = GetLowestLayerState(_moveStatesSet);
            }


            public void CalculateCurrentState<EnumType>(Dictionary<EnumType, State<EnumType>> stateSet, ref State<EnumType> currentState, ref State<EnumType> previousState)
            {
                previousState = currentState;
                foreach (var pair in stateSet)
                {
                    if (pair.Value.Semaphore() && (pair.Value.Layer >= currentState.Layer || !currentState.Semaphore()))
                    {
                        currentState = pair.Value;
                    }
                }
                if(!EqualityComparer<EnumType>.Default.Equals(currentState.CurrentStateType, previousState.CurrentStateType))
                {
                    previousState.DoOnExit();
                    currentState.DoOnEnter();
                }
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


            public void ChangeState()
            {
                CalculateCurrentState(_moveStatesSet, ref _currentMoveState, ref _previousMoveState);
            }


            public void BlockStateMachine()
            {
                _currentMoveState.SetMaxLayer();
            }
        }


        protected class State<EnumType>
        {
            private StateLayers _layer;
            public StateLayers Layer { get { return _layer; } protected set { _layer = value; } }
            public EnumType CurrentStateType;

            public State(StateLayers layer, SemaphoreDelegate semaphore, DoOnEnterDelegate doOnStart, DoOnExitDelegate doOnExit, EnumType CurrentStateType)
            {
                _layer = layer;
                Semaphore = semaphore;
                DoOnEnter = doOnStart;
                DoOnExit = doOnExit;
                this.CurrentStateType = CurrentStateType;
            }

            public SemaphoreDelegate Semaphore;
            public DoOnEnterDelegate DoOnEnter;
            public DoOnExitDelegate DoOnExit;


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
            public delegate void DoOnEnterDelegate();
            public delegate void DoOnExitDelegate();
        }
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
