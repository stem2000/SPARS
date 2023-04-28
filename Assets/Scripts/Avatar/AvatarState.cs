using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AvatarModel
{
    public class AvatarState
    {
        public StateStatsUpdatePackage _statsPackage;

        private StateChanger<MovementType> _moveChanger;

        private StateFromInfoPackage _infoPackage;

        private Vector3 _normal = Vector3.zero;
        private Vector3 _moveDirection = Vector3.zero;

        protected bool Grounded;
        protected bool OnSlope;
        protected bool ShouldDash;
        protected bool ShouldJump;
        protected bool ShouldShoot;

        private MovementDataPackage _moveDataPack;

        #region METHODS
        private Vector3 ConvertDirectionInput(Vector2 moveDirection)
        {
            return new Vector3(moveDirection.x, 0f, moveDirection.y);
        } 

        public AvatarState(StateStatsUpdatePackage statsPackage)
        {
            _statsPackage = statsPackage;
            CreateMoveChanger();
            _infoPackage = new StateFromInfoPackage();
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
        }

        public StateFromInfoPackage GetStateInfo()
        {
            _infoPackage.Grounded = Grounded;
            _infoPackage.StateWasChanged = _moveChanger.StateWasChanged;
            _infoPackage.CurrentMoveState = _moveChanger.CurrentState;
            return _infoPackage;
        }

        public void ReceiveUpdatedStateInfo(StateStatsUpdatePackage statsPackage)
        {
            _statsPackage = statsPackage;
        }

        public MovementDataPackage GetMovementData()
        {
            _moveDataPack.stateChanged = _moveChanger.StateWasChanged;

            switch (_moveChanger.CurrentState)
            {
                case MovementType.Run:
                    _moveDataPack.data = new RunData(_moveDirection, _statsPackage.RunSpeed);
                    _moveDataPack.type = MovementType.Run;
                    break;
                case MovementType.Fly:
                    _moveDataPack.data = new FlyData(_statsPackage.FlySpeedLimit);
                    _moveDataPack.type = MovementType.Fly;
                    break;
                case MovementType.RunOnSlope:
                    _moveDataPack.data = new RunOnSlopeData(_moveDirection, _statsPackage.RunSpeed, _normal);
                    _moveDataPack.type = MovementType.RunOnSlope;
                    break;
                case MovementType.Jump:
                    _moveDataPack.data = new JumpData(_moveDirection, _statsPackage.jumpStats.JumpForce);
                    _moveDataPack.type = MovementType.Jump;
                    break;
                case MovementType.Dash:
                    _moveDataPack.data = new DashData(_moveDirection, _statsPackage.dashStats.DashForce);
                    _moveDataPack.type = MovementType.Dash;
                    break;
            }
            return _moveDataPack;
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
                if(enumType == MovementType.Fly)
                    return false;
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

            public override void DoOnEnter(){}

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
                inJumpTask = StartMainProcess(tokenSource.Token); 
            }

            public override void DoOnExit() 
            {
                if(inJumpTask != null && inJumpTask.IsCanceled)
                    _inProcess = false;                    
            }

            public override bool WantsToChange()
            {
                return avatar.ShouldJump && avatar._statsPackage.jumpStats.JumpCharges > 0;
            }

            protected async Task StartMainProcess(CancellationToken token) 
            { 
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * avatar._statsPackage.jumpStats.JumpDuration), token);
                
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
                return avatar.ShouldDash && !_dashInCD;
            }

            protected async Task StartMainProcess(CancellationToken token)
            {
                _inProcess = true;

                await Task.Delay(Mathf.FloorToInt(1000 * avatar._statsPackage.dashStats.DashDuration), token);

                _inProcess = false;
            }

            protected async void StartDashCooldown()
            {
                _dashInCD = true;

                await Task.Delay(Mathf.FloorToInt(1000 * avatar._statsPackage.dashStats.DashLockTime));

                _dashInCD = false;
            }
        }
        #endregion
    }

    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope
    }

    public enum AttackType
    {
        Shoot, Hit
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
    #endregion
}