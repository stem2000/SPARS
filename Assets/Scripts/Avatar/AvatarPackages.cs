using UnityEngine;

namespace AvatarModel
{
    public class MovementDataPackage
    {
        public MovementData data;
        public MovementType type;
        public bool stateChanged;
    }

    public class StateChangingData
    {
        public MovementType CurrentMoveType;
        public AttackType CurrentAttackType;
        public bool Grounded;
        public bool MoveStateWasChanged;
        public bool AttackStateWasChanged;
        public Vector3 MoveDirection;
    }

    public class StatsPackage
    {
        public JumpStats jumpStats;
        public DashStats dashStats;
        public float FlySpeedLimit = 9;
        public float RunSpeed = 10;

        public StatsPackage(in AvatarStats avatarStats)
        {
            jumpStats = avatarStats.jumpStats.Clone();
            dashStats = avatarStats.dashStats.Clone();
        }
    }
}
