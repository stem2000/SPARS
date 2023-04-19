using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.VersionControl.Asset;

namespace AvatarModel
{
    public class AvatarState : MonoBehaviour
    {
        [SerializeField] protected AvatarStats ImmutableStats;

        public AvatarStats MutableStats;

        [HideInInspector] public Vector2 Rotation;


        private Rigidbody Rigidbody;
        private StateMachine _stateMachine;
        private ConstantForce ConstantForce_;

        protected HandleLandInteraction TouchTheLand;
        protected HandleLandInteraction LeaveTheLand;

        private Vector3 _lockedDirection = Vector3.zero;
        private Vector3 _normal = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;

        protected bool Grounded;
        protected bool OnSlope;
        protected bool ShouldDash;
        protected bool ShouldJump;
        protected bool ShouldShoot;
        protected bool JumpLocked;
        protected bool DashLocked;

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
            InitializeLandInteractions();
        }

        private void InitializeLandInteractions()
        {
            LeaveTheLand += delegate { StartCoroutine(CoyoteTimer()); };
            TouchTheLand += delegate { MutableStats.CoyoteTime = ImmutableStats.CoyoteTime; };
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
            return new FlyData(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized, MutableStats.FlySpeedLimit);
        }


        private RunOnSlopeData GetRunOnSlopeData()
        {
            return new RunOnSlopeData(transform.TransformVector(_moveDirection), MutableStats.RunSpeed, _normal);
        }


        private JumpData GetJumpData()
        {
            return new JumpData(transform.TransformVector(_lockedDirection), MutableStats.JumpForce);
        }

        protected void DisableYForces() { Rigidbody.useGravity = false; ConstantForce_.enabled = false;}

        protected void EnableYForces() { Rigidbody.useGravity = true; ConstantForce_.enabled = true; }

        protected void RestoreJumpCharges()
        {
            MutableStats.JumpCharges = ImmutableStats.JumpCharges;
        }

        protected IEnumerator CoyoteTimer()
        {
            yield return new WaitForSeconds(MutableStats.CoyoteTime);
            MutableStats.CoyoteTime = 0;
        }


        protected delegate void HandleLandInteraction(); 

        private class StateMachine
        {
            private AvatarState avatar;
            private Dictionary<MovementType, State<MovementType>> _moveStatesSet;
            private State<MovementType> _previousMoveState = null;
            public State<MovementType> _currentMoveState = null;


            public MovementType CurrentMoveType { get { return _currentMoveState.StateType; }}


            public StateMachine(AvatarState avatar)
            {
                this.avatar = avatar; 
                _moveStatesSet = new Dictionary<MovementType, State<MovementType>>();
                InitializeStatesSets();
            }


            private void InitializeStatesSets()
            {
                InitializeMoveStateSet(); 
            } 


            private void InitializeMoveStateSet()
            {
                _moveStatesSet.Add(MovementType.Run, new RunState<MovementType>(StateLayers.Layer1, MovementType.Run, avatar));
                _moveStatesSet.Add(MovementType.RunOnSlope, new RunOnSlopeState<MovementType>(StateLayers.Layer2, MovementType.RunOnSlope, avatar));
                _moveStatesSet.Add(MovementType.Fly, new FlyState<MovementType>(StateLayers.Layer3, MovementType.Fly, avatar));
                _moveStatesSet.Add(MovementType.Jump, new JumpState<MovementType>(StateLayers.Layer4, MovementType.Jump, avatar));
                _moveStatesSet.Add(MovementType.Dash, new JumpState<MovementType>(StateLayers.Layer4, MovementType.Jump, avatar));
                _currentMoveState = GetLowestLayerState(_moveStatesSet);
            }


            public void CalculateCurrentState<EnumType>(Dictionary<EnumType, State<EnumType>> stateSet, ref State<EnumType> currentState, ref State<EnumType> previousState)
            {
                foreach (var pair in stateSet)
                {
                    if (pair.Value.Semaphore() && ((pair.Value.Layer > currentState.Layer) || !pair.Value.InProcess))
                    {
                        currentState.DoOnExit();
                        currentState = pair.Value;
                        currentState.DoOnEnter();
                    }
                    else if(pair.Value.Semaphore() && pair.Value.CanInterrupt)
                    {
                        currentState.StopProcess();
                        currentState = pair.Value;
                        currentState.DoOnEnter();
                    }
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
        }


        protected abstract class State<EnumType>
        {
            protected static AvatarState avatar;
            private StateLayers _layer;
            public StateLayers Layer { get { return _layer; } protected set { _layer = value; } }
            public bool CanInterrupt { get { return _canInterrupt; } }
            public bool InProcess { get { return _inProcess; } }
            public EnumType StateType;

            protected bool _canInterrupt;
            protected bool _inProcess;

            protected Coroutine MainProcess = null;

            public State(StateLayers layer, EnumType stateType, AvatarState _avatar)
            {
                _layer = layer;
                StateType = stateType;
                _canInterrupt = _inProcess = false;
                avatar = _avatar;
            }           

            public abstract bool Semaphore();
            public virtual void DoOnEnter() { HandleProcess();}
            public virtual void DoOnExit() { }
            protected virtual void HandleProcess() { MainProcess = avatar.StartCoroutine(Process()); }
            protected virtual IEnumerator Process() { yield return null; }
            public virtual void StopProcess(){ avatar.StopCoroutine(MainProcess); _inProcess = false;}
        }


        protected class RunState<MovementType> : State<MovementType>
        {
            public RunState(StateLayers layer, MovementType stateType, AvatarState _avatar) : base(layer, stateType, _avatar) {}

            public override void DoOnEnter()
            {
                avatar.EnableYForces();
                avatar.TouchTheLand();
            }        

            new public void DoOnExit()
            {
                avatar.LeaveTheLand();
            }

            public override bool Semaphore()
            {
                return avatar.Grounded;
            }
        }

        protected class RunOnSlopeState<MovementType> : State<MovementType>
        {
            public RunOnSlopeState(StateLayers layer, MovementType stateType, AvatarState _avatar) : base(layer, stateType, _avatar) { }

            public override void DoOnEnter()
            {
                avatar.DisableYForces();
                avatar.TouchTheLand();
            }

            new public void DoOnExit()
            {
                avatar.LeaveTheLand();
            }

            public override bool Semaphore()
            {
                return avatar.Grounded && avatar.OnSlope;
            }
        }

        protected class FlyState<MovementType> : State<MovementType>
        {
            public FlyState(StateLayers layer, MovementType stateType, AvatarState _avatar) : base(layer, stateType, _avatar) { }

            public override void DoOnEnter()
            {
                avatar.EnableYForces();
            }

            public override bool Semaphore()
            {
                return !avatar.Grounded;
            }
        }
        
        protected class JumpState<MovementType> : State<MovementType>
        {
            Coroutine restoreCharges = null;
            bool _chargesRestored = false;
            public JumpState(StateLayers layer, MovementType stateType, AvatarState _avatar) : base(layer, stateType, _avatar) 
            {
                _canInterrupt = true;
            }

            public override void DoOnEnter()
            {
                avatar.EnableYForces();
                avatar.MutableStats.CoyoteTime = 0f;
                if(avatar._lockedDirection == Vector3.zero)
                    avatar._lockedDirection = avatar._moveDirection;
                if(avatar.MutableStats.JumpCharges != 0f)
                    avatar.MutableStats.JumpCharges--;
            }

            public override void DoOnExit()
            {
                avatar._lockedDirection = Vector3.zero;
                
                if (avatar.MutableStats.JumpCharges == 0)
                {
                    if(restoreCharges == null)
                        restoreCharges = avatar.StartCoroutine(ResetCharges());
                    else if (_chargesRestored)
                    {
                        restoreCharges = null;
                        _chargesRestored = false;
                    }
                }
            }

            public override bool Semaphore()
            {
                 return (avatar.ShouldJump && (avatar.MutableStats.JumpCharges > 0 || avatar.Grounded || avatar.MutableStats.CoyoteTime > 0));
            }

            protected override IEnumerator Process()
            {
                _inProcess = true;
                avatar.ShouldJump = false;
                yield return new WaitForSeconds(avatar.MutableStats.JumpDuration);
                _inProcess = false;
                DoOnExit();
            }

            private IEnumerator ResetCharges()
            {
                yield return new WaitForSeconds(avatar.MutableStats.JumpChargeResetTime);

                avatar.RestoreJumpCharges();
            }
        }


        protected class DashState<MovementType> : State<MovementType>
        {
            public DashState(StateLayers layer, MovementType stateType, AvatarState _avatar) : base(layer, stateType, _avatar) { }

            public override void DoOnEnter()
            {
                avatar.EnableYForces();
                if (avatar._lockedDirection == Vector3.zero)
                    avatar._lockedDirection = avatar._moveDirection;
            }

            public override bool Semaphore()
            {
                return false;
            }


            new public void DoOnExit()
            {
                avatar._lockedDirection = Vector3.zero;
            }
        }
    }


    public delegate void LandingOnTheGround();
    public delegate bool SemaphoreDelegate();
    public delegate void DoOnEnterDelegate();
    public delegate void DoOnExitDelegate();
    public delegate void ProcessDelegate();


    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope
    }

    public enum StateLayers
    {
        Layer1, Layer2, Layer3, Layer4, Layer5, Layer6, Layer7, Layer8, Layer9, Layer10,

        AutomaticTransitionForbidden = Int32.MaxValue
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
