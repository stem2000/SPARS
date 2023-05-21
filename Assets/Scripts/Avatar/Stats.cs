using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Avatar
{
    [Serializable]
    public class Stats
    {
        [SerializeField] private StatsSet _stats;

        public void Initialize()
        {
            _stats.Initialize();
        }

        public float GetDashDuration()
        {
            return _stats.ImmutableStats.DashDuration;
        }

        public float GetDashForce()
        {
            return _stats.ImmutableStats.DashForce;
        }

        public float GetDashLockTime()
        {
            return _stats.ImmutableStats.DashLockTime;
        }

        public float GetFlySpeedLimit()
        {
            return _stats.ImmutableStats.FlySpeedLimit;
        }

        public float GetJumpCharges()
        {
            return _stats.MutableStats.JumpCharges;
        }

        public float GetJumpDuration()
        {
            return _stats.ImmutableStats.JumpDuration;
        }

        public float GetJumpForce()
        {
            return _stats.ImmutableStats.JumpForce;
        }

        public float GetRunSpeed()
        {
            return _stats.ImmutableStats.RunSpeed;
        }

        internal void ReduceJumpCharges(int removable)
        {
            if(_stats.MutableStats.JumpCharges - removable >= 0)
                _stats.MutableStats.JumpCharges -= removable;
        }

        internal void ResetJumpCharges()
        {
            _stats.MutableStats.JumpCharges = _stats.ImmutableStats.JumpCharges;
        }
    }

}