using Game399.Shared.Models;

namespace Game399.Shared.Services
{
    /// <summary>
    /// Domain logic for ticking the world timer.
    /// </summary>
    public interface IWorldStateService
    {
        WorldStateModel World { get; }

        void StartDay(float durationSeconds);
        void StartNight(float durationSeconds);
        void Tick(float deltaTime);
    }
}