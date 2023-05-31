using System;
using UnityEngine;

namespace Avatar
{
    [Serializable]
    public class StatsCounter
    {
        [SerializeField] private StatsSet _statsSet;

        public void Initialize()
        {
            _statsSet.Initialize();
        }

        public float GetDashDuration()
        {
            return _statsSet.ImmutableStats.DashDuration;
        }

        public float GetDashForce()
        {
            return _statsSet.ImmutableStats.DashForce;
        }

        public float GetDashLockTime()
        {
            return _statsSet.ImmutableStats.DashLockTime;
        }

        public float GetFlySpeedLimit()
        {
            return _statsSet.ImmutableStats.FlySpeedLimit;
        }

        public float GetJumpCharges()
        {
            return _statsSet.MutableStats.JumpCharges;
        }

        public float GetJumpDuration()
        {
            return _statsSet.ImmutableStats.JumpDuration;
        }

        public float GetJumpForce()
        {
            return _statsSet.ImmutableStats.JumpForce;
        }

        public float GetRunSpeed()
        {
            return _statsSet.ImmutableStats.RunSpeed;
        }

        internal void ReduceJumpCharges(int removable)
        {
            if(_statsSet.MutableStats.JumpCharges - removable >= 0)
                _statsSet.MutableStats.JumpCharges -= removable;
        }

        internal void ResetJumpCharges()
        {
            _statsSet.MutableStats.JumpCharges = _statsSet.ImmutableStats.JumpCharges;
        }
    }

}