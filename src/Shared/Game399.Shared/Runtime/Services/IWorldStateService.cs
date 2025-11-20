using Game399.Shared.Models;

namespace Game399.Shared.Services
{
    // Domain logic for ticking the world timers.
    public interface IWorldStateService
    {
        WorldStateModel World { get; }

        void StartDay(float durationSeconds);
        void StartNight(float durationSeconds);
        void Tick(float deltaTime);
    }
}