
namespace Avatar
{
    public class StatsProvider
    {
        private StatsCounter _statsCounter;

        public StatsProvider(StatsCounter statsCounter)
        {
            _statsCounter = statsCounter;
        }

        public float DashDuration { get => _statsCounter.GetDashDuration();  }
        public float DashLockTime { get => _statsCounter.GetDashLockTime(); }
        public float DashForce { get => _statsCounter.GetDashForce(); }
        public float JumpForce { get => _statsCounter.GetJumpForce(); }
        public float JumpCharges { get => _statsCounter.GetJumpCharges(); }
        public float JumpDuration { get => _statsCounter.GetJumpDuration(); }
        public float RunSpeed { get => _statsCounter.GetRunSpeed(); }
        public float FlySpeedLimit { get => _statsCounter.GetFlySpeedLimit(); }
    }
}
