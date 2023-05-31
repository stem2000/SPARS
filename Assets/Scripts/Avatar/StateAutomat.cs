using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Avatar
{
    public class StateAutomat
    {
        private StatsProvider _statsProvider;
        private StateChanger<MovementType> _moveChanger;
        private StateChanger<AttackType> _attackChanger;

        public Vector3 Normal = Vector3.zero;
        public Vector3 MoveDirection = Vector3.zero;

        public bool Grounded;
        public bool OnSlope;
        public bool ShouldDash;
        public bool ShouldJump;
        public bool ShouldShoot;
        public bool ShouldPunch;
        public bool CanMove;
        public bool CanAttack;

        #region METHODS
        public StateAutomat(StatsProvider stats)
        {
            _statsProvider = stats;

            CreateMoveChanger();
            CreateAttackChanger();
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
        public void ChangeState()
        {
            _moveChanger.ChangeState();
            _attackChanger.ChangeState();
        }

        public MovementType GetMoveState()
        {
            return _moveChanger.CurrentState;
        }

        public AttackType GetAttackState()
        {
            return _attackChanger.CurrentState;
        }

        public MovementType GetPrevMoveState()
        {
            return _moveChanger.PreviousState;
        }

        public bool HasMoveStateChanged()
        {
            return _moveChanger.StateWasChanged;
        }

        public bool HasAttackStateChanged()
        {
            return _attackChanger.StateWasChanged;
        }

        public bool WasAttemptToChangeState()
        {
            return ShouldDash || ShouldJump || ShouldPunch || ShouldShoot;
        }
        #endregion

        #region STATE CHANGER
        private class StateChanger<EnumType>
        {
            protected static StateAutomat avatar;
            private Dictionary<EnumType, State<EnumType>> _statesSet;
            private State<EnumType> _currentState;

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

            public bool StateWasChanged;
            public EnumType PreviousState;

            public StateChanger(StateAutomat Avatar)
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
                        PreviousState = _currentState.StateType;                        

                        _currentState = state;
                        _currentState.DoOnEnter();
                        StateWasChanged = true;
                    }
                }
            } 
            
        }
        #endregion

        #region STATES CLASSES
        protected abstract class State<EnumType>
        {
            protected StateAutomat _avatar = null;

            public EnumType StateType;
            public State(StateAutomat avatar)
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
            public StateWithProcess(StateAutomat avatar) : base(avatar) {}
        }

        protected class RunState : State<MovementType>
        {
            public RunState(StateAutomat avatar) : base(avatar)
            {
                StateType = MovementType.Run;
            }

            public override bool CanBeChangedBy(MovementType enumType)
            {
                if(enumType == MovementType.Run)
                    return false;
                return true;
            }

            public override void DoOnEnter(){}

            public override void DoOnExit(){}

            public override bool WantsToChange()
            {
                return _avatar.Grounded && _avatar.MoveDirection.magnitude != 0 && !_avatar.OnSlope;
            }
        }

        protected class FlyState : State<MovementType>
        {
            public FlyState(StateAutomat avatar) : base(avatar)
            {
                StateType = MovementType.Fly;
            }

            public override bool CanBeChangedBy(MovementType enumType) 
            { 
                if(enumType == MovementType.Fly)
                    return false;
                if(enumType == MovementType.Idle && !_avatar.Grounded)
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
            public OnSlopeState(StateAutomat avatar) : base(avatar)
            {
                StateType = MovementType.RunOnSlope;
            }

            public override bool CanBeChangedBy(MovementType enumType)
            {
                if(enumType == MovementType.RunOnSlope)
                    return false;
                return true;
            }

            public override void DoOnEnter(){}

            public override void DoOnExit() { }

            public override bool WantsToChange()
            {
                return _avatar.Grounded && _avatar.OnSlope && _avatar.MoveDirection.magnitude != 0;
            }
        }

        protected class JumpState : StateWithProcess<MovementType>
        {   
            Task inJumpTask;

            public JumpState(StateAutomat avatar) : base(avatar) 
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
                return _avatar.ShouldJump && _avatar._statsProvider.JumpCharges > 0 && _avatar.CanMove;
            }

            protected async Task StartMainProcess(CancellationToken token) 
            { 
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * _avatar._statsProvider.JumpDuration), token);
                
                _inProcess = false;
            }
        }

        protected class DashState : StateWithProcess<MovementType>
        {
            Task inDashTask;
            private bool _dashInCD;
            public DashState(StateAutomat avatar) : base(avatar)
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
                return _avatar.ShouldDash && !_dashInCD && _avatar.CanMove;
            }

            protected async Task StartMainProcess(CancellationToken token)
            {
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * _avatar._statsProvider.DashDuration), token);

                _inProcess = false;
            }

            protected async void StartDashCooldown()
            {
                _dashInCD = true;

                await Task.Delay(Mathf.FloorToInt(1000 * _avatar._statsProvider.DashLockTime));

                _dashInCD = false;
            }
        }

        protected class IdleState : State<MovementType>
        {
            public IdleState(StateAutomat avatar) : base(avatar) 
            {
                StateType = MovementType.Idle;
            }

            public override bool CanBeChangedBy(MovementType enumType)
            {
                if(enumType == MovementType.Idle)
                    return false;
                return true;
            }

            public override void DoOnEnter(){}

            public override void DoOnExit(){}

            public override bool WantsToChange()
            {
                if (_avatar.MoveDirection.magnitude == 0)
                    return true;
                return false;
            }
        }

        protected class ShootState : State<AttackType>
        {
            public ShootState(StateAutomat avatar) : base(avatar) 
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
                return _avatar.ShouldShoot && _avatar.CanAttack;
            }
        }

        protected class CalmState : State<AttackType>
        {
            public CalmState(StateAutomat avatar) : base(avatar) 
            {
                StateType = AttackType.Calm;
            }

            public override bool CanBeChangedBy(AttackType enumType)
            {
                if(enumType == AttackType.Calm) 
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
            public PunchState(StateAutomat avatar) : base(avatar)
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
                return _avatar.ShouldPunch && _avatar.CanAttack;
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
        Calm, Shoot, Punch
    }

    public class StateInfoProvider
    {
        private StateAutomat _avatarState;

        public StateInfoProvider(StateAutomat avatarState)
        {
            _avatarState = avatarState;
        }

        public MovementType CurrentMoveState { get => _avatarState.GetMoveState(); }
        public AttackType CurrentAttackState { get => _avatarState.GetAttackState(); }
        public bool Grounded { get => _avatarState.Grounded; }
        public bool OnSlope { get => _avatarState.OnSlope; }
        public bool ShouldDash { get => _avatarState.ShouldDash; }
        public bool ShouldShoot { get => _avatarState.ShouldShoot; }
        public bool ShouldPunch { get => _avatarState.ShouldPunch; }
        public bool ShouldJump { get => _avatarState.ShouldJump; }
        public bool CanMove { get => _avatarState.CanMove; }
        public bool CanAttack { get => _avatarState.CanAttack; }
        public Vector3 MoveDirection { get => _avatarState.MoveDirection; }
        public Vector3 Normal { get => _avatarState.Normal; }
        public bool WasMoveStateChanged { get => _avatarState.HasMoveStateChanged(); }
        public bool WasAttackStateChanged { get => _avatarState.HasAttackStateChanged(); }
        public bool WasAttemptToChangeState { get => _avatarState.WasAttemptToChangeState(); }
        public MovementType PreviousMoveState { get => _avatarState.GetPrevMoveState(); }
    }

}
