using Game399.Shared.Diagnostics;
using Game399.Shared.Models;

namespace Game399.Shared.Services.Implementation
{
    public class WorldStateService : IWorldStateService
    {
        private readonly IGameLog _log;

        public WorldStateModel World { get; } = new();

        private float _currentDurationSeconds;

        public WorldStateService(IGameLog log)
        {
            _log = log;
        }

        public void StartDay(float durationSeconds)
        {
            _currentDurationSeconds = durationSeconds;
            World.IsDay.Value = true;
            World.TimeLeft.Value = durationSeconds;
            _log.Info($"WorldState: Day started for {durationSeconds} seconds.");
        }

        public void StartNight(float durationSeconds)
        {
            _currentDurationSeconds = durationSeconds;
            World.IsDay.Value = false;
            World.TimeLeft.Value = durationSeconds;
            _log.Info($"WorldState: Night started for {durationSeconds} seconds.");
        }

        public void Tick(float deltaTime)
        {
            var t = World.TimeLeft.Value - deltaTime;
            if (t < 0f) t = 0f;
            World.TimeLeft.Value = t;
        }
    }
}