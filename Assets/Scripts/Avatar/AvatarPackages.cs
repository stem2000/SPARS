using UnityEngine;

namespace AvatarModel
{
    public class MovementDataPackage
    {
        public MovementData data;
        public MovementType type;
        public bool stateChanged;
    }

    public class StateData
    {
        public MovementType CurrentMoveType;
        public AttackType CurrentAttackType;
        public bool Grounded;
        public bool MoveStateWasChanged;
        public bool AttackStateWasChanged;
        public Vector3 MoveDirection;
        public Vector3 Normal;
        public bool WasAttemptToChangeState;
    }

    public class ActualStats
    {
        public JumpStats jumpStats;
        public DashStats dashStats;
        public float FlySpeedLimit;
        public float RunSpeed;

        public ActualStats(in AvatarStats avatarStats)
        {
            jumpStats = avatarStats.jumpStats.Clone();
            dashStats = avatarStats.dashStats.Clone();
            FlySpeedLimit = avatarStats.FlySpeedLimit;
            RunSpeed = avatarStats.RunSpeed;
        }
    }
}
