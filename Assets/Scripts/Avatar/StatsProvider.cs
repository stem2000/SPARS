
namespace Avatar
{
    public class StatsProvider
    {
        private StatsAnalyst _analyst;

        public StatsProvider(StatsAnalyst analyst)
        {
            _analyst = analyst;
        }

        public float DashDuration { get => _analyst.GetDashDuration();  }
        public float DashLockTime { get => _analyst.GetDashLockTime(); }
        public float DashForce { get => _analyst.GetDashForce(); }
        public float JumpForce { get => _analyst.GetJumpForce(); }
        public float JumpCharges { get => _analyst.GetJumpCharges(); }
        public float JumpDuration { get => _analyst.GetJumpDuration(); }
        public float RunSpeed { get => _analyst.GetRunSpeed(); }
        public float FlySpeedLimit { get => _analyst.GetFlySpeedLimit(); }
    }
}
