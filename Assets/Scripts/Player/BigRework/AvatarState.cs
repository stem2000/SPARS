using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AvatarModel
{
    public class AvatarState
    {
        public AvatarStats MutableStats;

        private StateChanger<MovementType> _moveChanger;

        private StateInfoPackage _infoPackage;

        private Vector3 _lockedDirection = Vector3.zero;
        private Vector3 _normal = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;

        protected bool Grounded;
        protected bool OnSlope;
        protected bool ShouldDash;
        protected bool ShouldJump;
        protected bool ShouldShoot;


        private Vector3 ConvertDirectionInput(Vector2 moveDirection)
        {
            return new Vector3(moveDirection.x, 0f, moveDirection.y);
        } 

        public AvatarState(AvatarStats avatarStats)
        {
            MutableStats = avatarStats;
            _infoPackage = new StateInfoPackage();
            CreateMoveChanger();
        }

        private void CreateMoveChanger()
        {
            _moveChanger = new StateChanger<MovementType>(this);
            _moveChanger.AddState(new RunState(this));
            _moveChanger.AddState(new FlyState(this));
            _moveChanger.AddState(new OnSlopeState(this));
            _moveChanger.AddState(new JumpState(this));
            _moveChanger.AddState(new DashState(this));
        }

        public StateInfoPackage GetStateInfo()
        {
            _infoPackage.Grounded = Grounded;
            _infoPackage.StateWasChanged = _moveChanger.StateWasChanged;
            _infoPackage.CurrentMoveState = _moveChanger.CurrentState;
            return _infoPackage;
        }

        public MovementData GetMovementData()
        {
            switch (_moveChanger.CurrentState)
            {
                case MovementType.Run: return GetRunData();
                case MovementType.Fly: return GetFlyData();
                case MovementType.RunOnSlope: return GetRunOnSlopeData();
                case MovementType.Jump: return GetJumpData();
                case MovementType.Dash: return GetDashData();
                default: return GetRunData();
            }
        }

        public void HandleInput(in StateUpdatePackage package)
        {
            _moveDirection = ConvertDirectionInput(package.MoveDirection);
            _normal = package.Normal;
            Grounded = package.Grounded;
            OnSlope = package.OnSlope;
            ShouldDash = package.ShouldDash;
            ShouldJump = package.ShouldJump;
            ShouldShoot = package.ShouldShoot;
            _moveChanger.ChangeState();
        }

        public MovementType GetMoveState()
        {
            return _moveChanger.CurrentState;
        }

        #region GET MOVEMENTDATA METHODS
        private RunData GetRunData()
        {
            return new RunData(_moveDirection, MutableStats.RunSpeed);
        }

        private FlyData GetFlyData()
        {
            return new FlyData(MutableStats.FlySpeedLimit);
        }

        private RunOnSlopeData GetRunOnSlopeData()
        {
            return new RunOnSlopeData(_moveDirection, MutableStats.RunSpeed, _normal);
        }

        private JumpData GetJumpData()
        {
            return new JumpData(_lockedDirection, MutableStats.JumpForce);
        }

        private DashData GetDashData()
        {        
            return new DashData(_lockedDirection, MutableStats.DashForce);
        }
        #endregion

        private class StateChanger<EnumType>
        {
            protected static AvatarState avatar;
            private Dictionary<EnumType, State<EnumType>> _statesSet;
            public EnumType CurrentState {get {return _currentState.StateType;} }
            private State<EnumType>  _currentState;
            public bool StateWasChanged;

            public StateChanger(AvatarState Avatar)
            {
                avatar = Avatar;
                _statesSet = new Dictionary<EnumType, State<EnumType>>();
            }

            public void AddState(State<EnumType> state)
            {
                _statesSet.Add(state.StateType, state);
            }

            public void ChangeState()
            {
                if (_currentState == null)
                        _currentState = _statesSet.FirstOrDefault().Value;

                StateWasChanged = false;

                foreach (var state in _statesSet.Values)
                {
                    if(state.WantsToChange() && _currentState.CanBeChangedBy(state.StateType))
                    {
                        _currentState.DoOnExit();
                        _currentState = state;
                        StateWasChanged = true;
                        _currentState.DoOnEnter();
                    }
                }
            }       
        }

        #region STATES CLASSES
        protected abstract class State<EnumType>
        {
            protected AvatarState avatar = null;

            public EnumType StateType;
            public State(AvatarState avatar)
            {
                this.avatar = avatar;
            }           

            public abstract bool CanBeChangedBy(EnumType enumType);
            public abstract bool WantsToChange();
            public abstract void DoOnEnter();
            public abstract void DoOnExit();
        }

        protected abstract class StateWithProcess<EnumType> : State<EnumType>
        {
            protected HashSet<EnumType> _interruptingStates;
            protected bool _inProcess;
            protected CancellationTokenSource tokenSource;
            public StateWithProcess(AvatarState avatar) : base(avatar) {}
        }

        protected class RunState : State<MovementType>
        {
            public RunState(AvatarState avatar) : base(avatar)
            {
                StateType = MovementType.Run;
            }

            public override bool CanBeChangedBy(MovementType enumType){return true;}

            public override void DoOnEnter(){}

            public override void DoOnExit(){}

            public override bool WantsToChange()
            {
                return avatar.Grounded;
            }
        }

        protected class FlyState : State<MovementType>
        {
            public FlyState(AvatarState avatar) : base(avatar)
            {
                StateType = MovementType.Fly;
            }

            public override bool CanBeChangedBy(MovementType enumType) 
            { 
                return true; 
            }

            public override void DoOnEnter() { }

            public override void DoOnExit() { }

            public override bool WantsToChange()
            {
                return !avatar.Grounded;
            }
        }

        protected class OnSlopeState : State<MovementType>
        {
            public OnSlopeState(AvatarState avatar) : base(avatar)
            {
                StateType = MovementType.RunOnSlope;
            }

            public override bool CanBeChangedBy(MovementType enumType)
            {
                if(enumType == MovementType.Run && avatar.OnSlope)
                    return false;
                return true;
            }

            public override void DoOnEnter() { }

            public override void DoOnExit() { }

            public override bool WantsToChange()
            {
                return avatar.Grounded && avatar.OnSlope;
            }
        }

        protected class JumpState : StateWithProcess<MovementType>
        {   
            Task inJumpTask;

            public JumpState(AvatarState avatar) : base(avatar) 
            {
                _interruptingStates = new HashSet<MovementType>()
                {
                    MovementType.Jump,
                    MovementType.Dash
                };
                StateType = MovementType.Jump;
            }

            public override bool CanBeChangedBy(MovementType enumType)
            {
                if (!_inProcess || _interruptingStates.Contains(enumType))
                    return true;
                else return false;
            }

            public override void DoOnEnter() 
            {
                tokenSource = new CancellationTokenSource();
                avatar.MutableStats.JumpCharges--;
                avatar._lockedDirection = avatar._moveDirection;
                inJumpTask = StartMainProcess(tokenSource.Token); 
                DoOnExit();
            }

            public override void DoOnExit() 
            {
                if(inJumpTask != null && inJumpTask.IsCanceled)
                    _inProcess = false;                    
            }

            public override bool WantsToChange()
            {
                return avatar.ShouldJump && avatar.MutableStats.JumpCharges > 0;
            }

            public async Task StartMainProcess(CancellationToken token) 
            { 
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * avatar.MutableStats.JumpDuration), token);
                
                _inProcess = false;
            }
        }

        protected class DashState : StateWithProcess<MovementType>
        {
            Task inDashTask;

            public DashState(AvatarState avatar) : base(avatar)
            {
                _interruptingStates = new HashSet<MovementType>
                {
                    MovementType.Jump
                };
                StateType = MovementType.Dash;
            }

            public override bool CanBeChangedBy(MovementType enumType)
            {
                if (!_inProcess || _interruptingStates.Contains(enumType))
                    return true;
                else return false;
            }

            public override void DoOnEnter()
            {
                tokenSource = new CancellationTokenSource();
                avatar._lockedDirection = avatar._moveDirection;
                inDashTask = StartMainProcess(tokenSource.Token);
                DoOnExit();
            }

            public override void DoOnExit()
            {
                if (inDashTask != null && inDashTask.IsCanceled)
                    _inProcess = false;
            }

            public override bool WantsToChange()
            {
                return avatar.ShouldDash;
            }

            public async Task StartMainProcess(CancellationToken token)
            {
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * avatar.MutableStats.DashDuration), token);

                _inProcess = false;
            }
        }
        #endregion
    }


    public delegate bool SemaphoreDelegate();
    public delegate void DoOnEnterDelegate();
    public delegate void DoOnExitDelegate();
    public delegate void ProcessDelegate();

    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope
    }

    #region MOVEMENTDATA CLASSES
    public abstract class MovementData { }

    public class RunData: MovementData
    {
        public Vector3 direction;
        public float speed;

        public RunData(Vector3 direction, float speed)
        {
            this.direction = direction;
            this.speed = speed;
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

    public class DashData : MovementData 
    {
        public Vector3 direction;
        public float force;

        public DashData(Vector3 direction, float force)
        {
            this.direction = direction;
            this.force = force;
        }
    }

    public class FlyData : MovementData
    {
        public float speedLimit;

        public FlyData(float speedLimit)
        {
            this.speedLimit = speedLimit;
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

    #endregion
}
