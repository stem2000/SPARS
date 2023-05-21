using System;
using UnityEngine;

namespace Avatar
{
    [Serializable]
    public class StatsSet
    {
        [SerializeField] public ImmutableStats ImmutableStats;
        public MutableStats MutableStats;

        public void Initialize() 
        {
            MutableStats = new MutableStats(ImmutableStats);
        }
    }

    public class MutableStats
    {
        public int JumpCharges;

        public MutableStats(ImmutableStats stats)
        {
            JumpCharges = stats.JumpCharges;
        }
    }

    [Serializable]
    public class ImmutableStats
    {
        [SerializeField] private float _dashForce = 10;
        [SerializeField] private float _airDashSpeedFactor = 0.5f;
        [SerializeField] private float _dashLockTime = 0.5f;
        [SerializeField] private float _dashDuration = 0.25f;
        [SerializeField] private float _flySpeedLimit = 9;
        [SerializeField] private float _runSpeed = 10;
        [SerializeField] private float _jumpForce = 10;
        [SerializeField] private float _jumpDuration = 0.15f;
        [SerializeField] private int _jumpCharges = 1;
        [SerializeField] private float _coyoteTime = 0.2f;

        public float DashForce {get => _dashForce;}
        public float AirDashSpeedFactor { get => _airDashSpeedFactor; }
        public float DashLockTime { get => _dashLockTime; }
        public float DashDuration { get => _dashDuration; }
        public float FlySpeedLimit { get => _flySpeedLimit; }
        public float RunSpeed { get => _runSpeed; }
        public float JumpForce { get => _jumpForce; }
        public float JumpDuration { get => _jumpDuration; }
        public int JumpCharges { get => _jumpCharges; }
        public float CoyoteTime { get => _coyoteTime; }
    }

}
