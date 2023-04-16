using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel
{
    [Serializable]
    public class AvatarStats
    {
        [Header("Movement Stats")]
        [Space]
        [SerializeField] public float RunSpeed = 10;
        [Space]
        [SerializeField] public float JumpForce = 10;
        [SerializeField] public float JumpDuration = 0.15f;
        [SerializeField] public float CoyoteTime = 0.2f;
        [Space]
        [SerializeField] public float DashForce = 10;
        [SerializeField] public float DashLockTime = 0.5f;
        [SerializeField] public float DashDuration = 0.25f;
        [SerializeField] public float AirDashSpeedFactor = 0.5f;
        [Space]
        [SerializeField] public float FlySpeedLimit = 9;
        [Space]
        [Header("Basic Stats")]
        [Space]
        [SerializeField] public int MaxHp = 100;
        [Space]
        [Header("HitIntervals")]
        [Space]
        [Tooltip("Part of sample time given to the player to make a shoot")]
        [SerializeField] public float ShootHitInterval = 0.2f;
        [Tooltip("Part of sample time given to the player to make a dash")]
        [SerializeField] public float DashHitInterval = 0.2f;
        [Tooltip("Part of sample time given to the player to make a jump")]
        [SerializeField] public float JumpHitInterval = 0.2f;


        public AvatarStats Clone()
        {
            return (AvatarStats) MemberwiseClone();
        }
    }
}
