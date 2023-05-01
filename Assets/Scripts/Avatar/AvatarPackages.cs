using UnityEngine;

namespace AvatarModel
{
    public class MovementDataPackage
    {
        public MovementData data;
        public MovementType type;
        public bool stateChanged;
    }

    public class StateFromInfoPackage
    {
        public MovementType CurrentMoveType;
        public AttackType CurrentAttackType;
        public bool Grounded;
        public bool MoveStateWasChanged;
        public bool AttackStateWasChanged;
        public Vector3 MoveDirection;
    }

    public class StateUpdatePackage
    {
        public Vector3 MoveDirection = Vector3.zero;
        public Vector3 Normal = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;

        public bool Grounded;
        public bool OnSlope;
        public bool ShouldDash;
        public bool ShouldJump;
        public bool ShouldShoot;
        public bool ShouldPunch;
    }

    public class StateStatsUpdatePackage
    {
        public JumpStats jumpStats;
        public DashStats dashStats;
        public float FlySpeedLimit = 9;
        public float RunSpeed = 10;

        public StateStatsUpdatePackage(in AvatarStats avatarStats)
        {
            jumpStats = avatarStats.jumpStats.Clone();
            dashStats = avatarStats.dashStats.Clone();
        }
    }
}
