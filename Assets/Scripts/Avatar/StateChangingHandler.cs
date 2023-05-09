using UnityEngine;
using System;
using UnityEngine.Events;
using Microsoft.SqlServer.Server;
using UnityEngine.EventSystems;

namespace AvatarModel
{
    public class StateChangingHandler
    {

        private Rigidbody _rigidbody;
        private ConstantForce _constantForce;
        private AvatarStats _immutableStats;

        private StateData _packageFromState;
        private ActualStats _statsPackage;
        private MovementDataPackage _moveDataPack;

        #region METHODS
        public StateChangingHandler(Rigidbody rigidbody, AvatarStats stats)
        {
            _rigidbody = rigidbody;
            _immutableStats = stats;
            _statsPackage = new ActualStats(stats);
            _moveDataPack = new MovementDataPackage();
            _constantForce = _rigidbody.GetComponent<ConstantForce>();
        }

        public void GetStateData(in StateData package)
        {
            _packageFromState = package;

            HandleVerticalForces();
            UpdateStateStats();
        }

        public ActualStats GetStatsPackage()
        {
            return _statsPackage;
        }

        private void HandleVerticalForces()
        {
            if((_packageFromState.CurrentMoveType == MovementType.RunOnSlope || _packageFromState.CurrentMoveType == MovementType.Idle)
                && _packageFromState.MoveStateWasChanged)
            {
                _rigidbody.useGravity = false;
                _constantForce.enabled = false;
            }
            else if(_constantForce.enabled == false)
            {
                _rigidbody.useGravity = true;
                _constantForce.enabled = true;
            }
        }

        private void UpdateStateStats()
        {
            UpdateJumpStats();
        }

        private void UpdateJumpStats()
        {
            if (_packageFromState.CurrentMoveType == MovementType.Jump && _packageFromState.MoveStateWasChanged)
                _statsPackage.jumpStats.JumpCharges--;
            if (_packageFromState.Grounded && _packageFromState.CurrentMoveType != MovementType.Jump)
                _statsPackage.jumpStats.JumpCharges = _immutableStats.jumpStats.JumpCharges;
        }

        public MovementDataPackage GetMovementData()
        {
            _moveDataPack.stateChanged = _packageFromState.MoveStateWasChanged;

            switch (_packageFromState.CurrentMoveType)
            {
                case MovementType.Run:
                    _moveDataPack.data = new RunData(_packageFromState.MoveDirection, _statsPackage.RunSpeed);
                    _moveDataPack.type = MovementType.Run;
                    break;
                case MovementType.Fly:
                    _moveDataPack.data = new FlyData(_statsPackage.FlySpeedLimit);
                    _moveDataPack.type = MovementType.Fly;
                    break;
                case MovementType.RunOnSlope:
                    _moveDataPack.data = new RunOnSlopeData(_packageFromState.MoveDirection, _statsPackage.RunSpeed, _packageFromState.Normal);
                    _moveDataPack.type = MovementType.RunOnSlope;
                    break;
                case MovementType.Jump:
                    _moveDataPack.data = new JumpData(_packageFromState.MoveDirection, _statsPackage.jumpStats.JumpForce);
                    _moveDataPack.type = MovementType.Jump;
                    break;
                case MovementType.Dash:
                    _moveDataPack.data = new DashData(_packageFromState.MoveDirection, _statsPackage.dashStats.DashForce);
                    _moveDataPack.type = MovementType.Dash;
                    break;
                case MovementType.Idle:
                    _moveDataPack.data = new IdleData();
                    _moveDataPack.type = MovementType.Idle;
                    break;
            }
            return _moveDataPack;
        }
        #endregion
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
