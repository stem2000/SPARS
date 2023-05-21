
namespace Avatar
{
    public class StatsProvider
    {
        private Stats _stats;

        public StatsProvider(Stats stats)
        {
            _stats = stats;
        }

        public float DashDuration { get => _stats.GetDashDuration();  }
        public float DashLockTime { get => _stats.GetDashLockTime(); }
        public float DashForce { get => _stats.GetDashForce(); }
        public float JumpForce { get => _stats.GetJumpForce(); }
        public float JumpCharges { get => _stats.GetJumpCharges(); }
        public float JumpDuration { get => _stats.GetJumpDuration(); }
        public float RunSpeed { get => _stats.GetRunSpeed(); }
        public float FlySpeedLimit { get => _stats.GetFlySpeedLimit(); }
    }
}
