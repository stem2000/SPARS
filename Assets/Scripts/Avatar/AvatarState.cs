using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace AvatarModel
{
    public class AvatarState
    {
        public StatsPackage _statsPackage;

        private StateChanger<MovementType> _moveChanger;
        private StateChanger<AttackType> _attackChanger;

        private StateChangingData _infoPackage;

        public Vector3 Normal = Vector3.zero;
        public Vector3 MoveDirection = Vector3.zero;

        public bool Grounded;
        public bool OnSlope;
        public bool ShouldDash;
        public bool ShouldJump;
        public bool ShouldShoot;
        public bool ShouldPunch;

        private MovementDataPackage _moveDataPack;

        #region METHODS
        public AvatarState(StatsPackage statsPackage)
        {
            _statsPackage = statsPackage;
            CreateMoveChanger();
            CreateAttackChanger();
            _infoPackage = new StateChangingData();
            _moveDataPack = new MovementDataPackage();
        }

        private void CreateMoveChanger()
        {
            _moveChanger = new StateChanger<MovementType>(this);
            _moveChanger.AddState(new RunState(this));
            _moveChanger.AddState(new FlyState(this));
            _moveChanger.AddState(new OnSlopeState(this));
            _moveChanger.AddState(new JumpState(this));
            _moveChanger.AddState(new DashState(this));
            _moveChanger.AddState(new IdleState(this));
        }

        private void CreateAttackChanger()
        {
            _attackChanger = new StateChanger<AttackType>(this);
            _attackChanger.AddState(new ShootState(this));
            _attackChanger.AddState(new CalmState(this));
            _attackChanger.AddState(new PunchState(this));
        }

        public StateChangingData GetStateInfo()
        {
            _infoPackage.Grounded = Grounded;
            _infoPackage.MoveStateWasChanged = _moveChanger.StateWasChanged;
            _infoPackage.AttackStateWasChanged = _attackChanger.StateWasChanged;
            _infoPackage.CurrentMoveType = _moveChanger.CurrentState;
            _infoPackage.CurrentAttackType = _attackChanger.CurrentState;
            _infoPackage.MoveDirection = MoveDirection;
            return _infoPackage;
        }

        public void UpdateStats(StatsPackage statsPackage)
        {
            _statsPackage = statsPackage;
        }

        public MovementDataPackage GetMovementData()
        {
            _moveDataPack.stateChanged = _moveChanger.StateWasChanged;

            switch (_moveChanger.CurrentState)
            {
                case MovementType.Run:
                    _moveDataPack.data = new RunData(MoveDirection, _statsPackage.RunSpeed);
                    _moveDataPack.type = MovementType.Run;
                    break;
                case MovementType.Fly:
                    _moveDataPack.data = new FlyData(_statsPackage.FlySpeedLimit);
                    _moveDataPack.type = MovementType.Fly;
                    break;
                case MovementType.RunOnSlope:
                    _moveDataPack.data = new RunOnSlopeData(MoveDirection, _statsPackage.RunSpeed, Normal);
                    _moveDataPack.type = MovementType.RunOnSlope;
                    break;
                case MovementType.Jump:
                    _moveDataPack.data = new JumpData(MoveDirection, _statsPackage.jumpStats.JumpForce);
                    _moveDataPack.type = MovementType.Jump;
                    break;
                case MovementType.Dash:
                    _moveDataPack.data = new DashData(MoveDirection, _statsPackage.dashStats.DashForce);
                    _moveDataPack.type = MovementType.Dash;
                    break;
                case MovementType.Idle:
                    _moveDataPack.data = new IdleData();
                    _moveDataPack.type = MovementType.Idle;
                    break;
            }
            return _moveDataPack;
        }

        public void ChangeState()
        {
            _moveChanger.ChangeState();
            _attackChanger.ChangeState();
        }
        #endregion
        private class StateChanger<EnumType>
        {
            protected static AvatarState avatar;
            private Dictionary<EnumType, State<EnumType>> _statesSet;
            public EnumType CurrentState 
            {
                get 
                {                 
                    if(_currentState == null)
                    {
                        Type type = typeof(EnumType);
                        return (EnumType)type.GetEnumValues().GetValue(0);
                    }
                    return _currentState.StateType;
                } 
            }
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
            protected AvatarState _avatar = null;

            public EnumType StateType;
            public State(AvatarState avatar)
            {
                this._avatar = avatar;
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
                return _avatar.Grounded;
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
                if(enumType == MovementType.Fly || enumType == MovementType.Idle)
                    return false;
                return true; 
            }

            public override void DoOnEnter() { }

            public override void DoOnExit() { }

            public override bool WantsToChange()
            {
                return !_avatar.Grounded;
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
                if(enumType == MovementType.Run && _avatar.OnSlope)
                    return false;
                return true;
            }

            public override void DoOnEnter(){}

            public override void DoOnExit() { }

            public override bool WantsToChange()
            {
                return _avatar.Grounded && _avatar.OnSlope;
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
                inJumpTask = StartMainProcess(tokenSource.Token); 
            }

            public override void DoOnExit() 
            {
                if(inJumpTask != null && inJumpTask.IsCanceled)
                    _inProcess = false;                    
            }

            public override bool WantsToChange()
            {
                return _avatar.ShouldJump && _avatar._statsPackage.jumpStats.JumpCharges > 0;
            }

            protected async Task StartMainProcess(CancellationToken token) 
            { 
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * _avatar._statsPackage.jumpStats.JumpDuration), token);
                
                _inProcess = false;
            }
        }

        protected class DashState : StateWithProcess<MovementType>
        {
            Task inDashTask;
            private bool _dashInCD;
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
                inDashTask = StartMainProcess(tokenSource.Token);
            }

            public override void DoOnExit()
            {
                if (inDashTask != null && inDashTask.IsCanceled)
                    _inProcess = false;

                StartDashCooldown();
            }

            public override bool WantsToChange()
            {
                return _avatar.ShouldDash && !_dashInCD;
            }

            protected async Task StartMainProcess(CancellationToken token)
            {
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * _avatar._statsPackage.dashStats.DashDuration), token);

                _inProcess = false;
            }

            protected async void StartDashCooldown()
            {
                _dashInCD = true;

                await Task.Delay(Mathf.FloorToInt(1000 * _avatar._statsPackage.dashStats.DashLockTime));

                _dashInCD = false;
            }
        }

        protected class IdleState : State<MovementType>
        {
            public IdleState(AvatarState avatar) : base(avatar) 
            {
                StateType = MovementType.Idle;
            }

            public override bool CanBeChangedBy(MovementType enumType)
            {
                return true;
            }

            public override void DoOnEnter(){}

            public override void DoOnExit(){}

            public override bool WantsToChange()
            {
                if (_avatar.MoveDirection == Vector3.zero)
                    return true;
                return false;
            }
        }

        protected class ShootState : State<AttackType>
        {
            public ShootState(AvatarState avatar) : base(avatar) 
            { 
                StateType = AttackType.Shoot;
            }

            public override bool CanBeChangedBy(AttackType enumType)
            {
                return true;
            }

            public override void DoOnEnter(){}

            public override void DoOnExit(){}

            public override bool WantsToChange()
            {
                return _avatar.ShouldShoot;
            }
        }

        protected class CalmState : State<AttackType>
        {
            public CalmState(AvatarState avatar) : base(avatar) 
            {
                StateType = AttackType.Idle;
            }

            public override bool CanBeChangedBy(AttackType enumType)
            {
                if(enumType == AttackType.Idle) 
                    return false;
                return true;
            }

            public override void DoOnEnter(){}

            public override void DoOnExit(){}

            public override bool WantsToChange()
            {
                return !_avatar.ShouldShoot;
            }
        }

        protected class PunchState : State<AttackType>
        {
            public PunchState(AvatarState avatar) : base(avatar)
            {
                StateType = AttackType.Punch;
            }

            public override bool CanBeChangedBy(AttackType enumType)
            {
                return true;
            }

            public override void DoOnEnter() { }

            public override void DoOnExit() { }

            public override bool WantsToChange()
            {
                return _avatar.ShouldPunch;
            }
        }
        #endregion
    }

    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope, Idle
    }

    public enum AttackType
    {
        Idle, Shoot, Punch
    }

    #region MOVEMENTDATA CLASSES
    public abstract class MovementData { }

    public class RunData : MovementData
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

    public class IdleData : MovementData
    {

    }

    #endregion
}
