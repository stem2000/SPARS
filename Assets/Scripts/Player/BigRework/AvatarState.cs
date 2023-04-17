using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace AvatarModel
{
    public class AvatarState
    {
        public AvatarStats AvatarStats;

        private RaycastHit _slopeHit;
        public Vector2 MoveDirection;
        public Vector2 Rotation;

        public ShouldMove ShouldMove;
        public ShouldAttack ShouldAttack;
        public MovementStateMachine MoveStateMachine;

        public Rigidbody Rigidbody;
        public GameObject Avatar;
        public MovementData MovementData;


        public AvatarState(GameObject avatar, AvatarStats avatarStats)
        {
            Avatar = avatar;
            Rigidbody = Avatar.GetComponent<Rigidbody>();
            AvatarStats = avatarStats.Clone();
            ShouldMove = new ShouldMove();
            ShouldAttack = new ShouldAttack();
            MoveStateMachine = new MovementStateMachine(this);
        }
        

        public void SetInteractionData(bool grounded)
        {
            if(grounded) MoveStateMachine.ExternalUpdate(grounded, IsOnSlope());
            else MoveStateMachine.ExternalUpdate(false, false);
        }


        public MovementData TransferMovementData()
        {
            switch (MoveStateMachine.CurrentMoveType)
            {
                case MovementType.Run: return GetRunData();
                case MovementType.Fly: return GetFlyData();
                case MovementType.RunOnSlope: return GetRunOnSlopeData();
                default: return GetRunData();
            }
        }

        private bool IsOnSlope()
        {
            if (Physics.Raycast(Rigidbody.transform.position, Vector3.down, out _slopeHit, Rigidbody.transform.lossyScale.y * 0.5f))
            {
                var dot = Vector3.Dot(Vector3.up, _slopeHit.normal);
                if (dot != 1)
                    return true;
            }
            return false;
        }


        private RunData GetRunData()
        {
            var convertedVector = new Vector3(MoveDirection.x, 0f, MoveDirection.y);
            return new RunData(Avatar.transform.TransformVector(convertedVector) * AvatarStats.RunSpeed);
        }


        private FlyData GetFlyData()
        {
            var convertedVector = Vector3.ProjectOnPlane(Avatar.transform.forward, Vector3.up).normalized;
            return new FlyData(convertedVector, AvatarStats.FlySpeedLimit);
        }


        private RunOnSlopeData GetRunOnSlopeData()
        {
            var convertedVector = Avatar.transform.TransformVector(new Vector3(MoveDirection.x, 0f, MoveDirection.y));
            convertedVector = Vector3.ProjectOnPlane(MoveDirection, _slopeHit.normal).normalized * AvatarStats.RunSpeed;
            return new RunOnSlopeData(convertedVector);
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


    public class MovementStateMachine
    {
        private MovementType _currentMoveType = MovementType.Run;
        private State _currentState;
        private Dictionary<MovementType, State> _stateSet;

        private AvatarState _avatarState;

        private StateMachineInternalInfo _workData;

        public MovementType CurrentMoveType { get { return _currentMoveType; } }

        public void ExternalUpdate(bool grounded, bool onSlope)
        {
            _workData._grounded = grounded;
            _workData._onSlope = onSlope;
        }
        

        public MovementStateMachine(AvatarState avatarState)
        {
            _avatarState = avatarState;
            _workData = new StateMachineInternalInfo();
            InitializeStateSet();
        }
        

        protected void InitializeStateSet()
        {
            _stateSet = new Dictionary<MovementType, State>();
            
            InitializeLayers1_5();
            InitializeLayers6_10();
        }


        private void InitializeLayers1_5()
        {
            _stateSet.Add(
               MovementType.Run,
               new State
               (
                   State.StateLayers.Layer1,
                   delegate () { return _workData._grounded; },
                   delegate () { _avatarState.Rigidbody.useGravity = true; return; }));

            _stateSet.Add(
               MovementType.RunOnSlope,
               new State
               (
                   State.StateLayers.Layer2,
                   delegate () { return _workData._grounded && _workData._onSlope; },
                   delegate () { _avatarState.Rigidbody.useGravity = false; return; }));

            _stateSet.Add(
               MovementType.Fly,
               new State
               (
                   State.StateLayers.Layer3,
                   delegate () { return !_workData._grounded; },
                   delegate () { _avatarState.Rigidbody.useGravity = true; return; }));

            _stateSet.Add(
               MovementType.Jump,
               new State
               (
                   State.StateLayers.Layer4,
                   delegate () {
                       var semaphore = (_workData._grounded || _avatarState.AvatarStats.CoyoteTime > 0) &&
                                        _avatarState.ShouldMove.ShouldJump && _avatarState.AvatarStats.JumpCharges > 0;
                       return semaphore;
                   },
                   delegate () { return; }));

            _stateSet.Add(
               MovementType.Dash,
               new State
               (
                   State.StateLayers.Layer5,
                   delegate () {
                       return (!_workData._dashLocked) && _avatarState.ShouldMove.ShouldDash;
                   },
                   delegate () { return; }));
        }


        private void InitializeLayers6_10()
        {
            _stateSet.Add(
               MovementType.LockedVelocity,
               new State
               (
                   State.StateLayers.Layer6,
                   delegate () {
                       return _workData._inDash || _workData._inJump;
                   },
                   delegate () { _workData._lockedVelocity = _avatarState.Rigidbody.velocity;}));
        }



        public void CalculateCurrentState()
        {
            State supposedNextState = _stateSet[MovementType.Run];
            MovementType supposedNextType = MovementType.Run;
            foreach(var movementType in Enum.GetValues(typeof(MovementType)))
            {
                var state = _stateSet[(MovementType)movementType];
           
                if(state.Semaphore() && state.Layer > supposedNextState.Layer)
                {
                    supposedNextState = state;
                    supposedNextType = (MovementType)movementType;
                }
            }
            _currentState = supposedNextState;
            _currentMoveType = supposedNextType;

            _currentState.DoOnStart();
        }


        class State
        {
            public StateLayers Layer;
            public StateSemaphore Semaphore;
            public DoOnStart DoOnStart;

            public State(StateLayers layer, StateSemaphore func, DoOnStart func1)
            {
                Layer = layer;
                Semaphore = func;
                DoOnStart = func1;
            }

            public enum StateLayers
            {
                Layer1, Layer2, Layer3, Layer4, Layer5, Layer6, Layer7
            }
        }


        class StateMachineInternalInfo
        {
            public bool _grounded;
            public bool _onSlope;
            public bool _dashLocked;
            public bool _inDash;
            public bool _inJump;

            public Vector3 _lockedVelocity = Vector3.zero;
        }

        delegate bool StateSemaphore();
        delegate void DoOnStart();
    }


    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope, LockedVelocity
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
