using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;


namespace AvatarModel
{
    public class AvatarState
    {
        protected AvatarStats ImmutableStats;

        public AvatarStats MutableStats;

        private StateChanger<MovementType> _moveChanger;

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


        private Vector3 ConvertDirectionInput(Vector2 moveDirection)
        {
            return new Vector3(moveDirection.x, 0f, moveDirection.y);
        } 

        public AvatarState(AvatarStats avatarStats)
        {
            ImmutableStats = MutableStats = avatarStats;
            CreateMoveChanger();
        }

        private void CreateMoveChanger()
        {
            _moveChanger = new StateChanger<MovementType>(this);
            _moveChanger.AddState(new RunState(this));
            _moveChanger.AddState(new FlyState(this));
        }

        public MovementData GetMovementData()
        {
            switch (GetMoveState())
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
        #endregion

        protected delegate void HandleLandInteraction(); 

        private class StateChanger<EnumType>
        {
            protected static AvatarState avatar;
            private Dictionary<EnumType, State<EnumType>> _statesSet;
            public EnumType CurrentState {get {return _currentState.StateType;} }
            private State<EnumType>  _currentState;

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

                foreach (var state in _statesSet.Values)
                {
                    if(state.WantsToChange() && _currentState.CanBeChangedBy(state.StateType))
                    {
                        _currentState.DoOnExit();
                        _currentState = state;
                        _currentState.DoOnEnter();
                    }
                }
            }       
        }

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
            private HashSet<EnumType> _interruptingStates;
            private bool _inProcess;
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
    }


    public delegate bool SemaphoreDelegate();
    public delegate void DoOnEnterDelegate();
    public delegate void DoOnExitDelegate();
    public delegate void ProcessDelegate();

    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope
    }
    public enum PriorityLevels
    {
        Level1, Level2, Level3
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

    public class DashData : MovementData { }

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
