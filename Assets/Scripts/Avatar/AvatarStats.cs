using System;
using UnityEngine;

namespace AvatarModel
{
    [Serializable]
    public class AvatarStats
    {
        [Header("Movement Stats")]
        [Space]
        [Tooltip("Avatar running speed")]
        [SerializeField] public float RunSpeed = 10;
        [Space]
        [SerializeField] public float FlySpeedLimit = 9;
        [Space]
        public JumpStats jumpStats;
        [Space]
        public DashStats dashStats; [Space]        
        [Space]
        [Header("Basic Stats")]
        [Space]
        [SerializeField] public int MaxHp = 100;


        public AvatarStats Clone()
        {
            return (AvatarStats) MemberwiseClone();
        }
    }

    [Serializable]
    public class JumpStats
    {
        [Tooltip("The strength with which the avatar jumps")]
        [SerializeField] public float JumpForce = 10;
        [Tooltip("Duration of jump in seconds")]
        [SerializeField] public float JumpDuration = 0.15f;
        [SerializeField] public int JumpCharges = 1;
        [SerializeField] public float JumpChargeResetTime = 3f;
        [Tooltip("Time after breaking away from the ground when the avatar can still jump")]
        [SerializeField] public float CoyoteTime = 0.2f;

        public JumpStats Clone()
        {
            return (JumpStats)MemberwiseClone();
        }
    }

    [Serializable]
    public class DashStats
    {
        [Tooltip("The strength with which the avatar make dash")]
        [SerializeField] public float DashForce = 10;
        [SerializeField] public float AirDashSpeedFactor = 0.5f;
        [SerializeField] public float DashLockTime = 0.5f;
        [SerializeField] public float DashDuration = 0.25f;

        public DashStats Clone()
        {
            return (DashStats)MemberwiseClone();
        }
    }


}
