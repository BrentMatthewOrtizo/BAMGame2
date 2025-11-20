namespace Game399.Shared.Models
{
    // Domain model for day/night & timer.
    public class WorldStateModel
    {
        public ObservableValue<bool> IsDay { get; } = new(true);
        public ObservableValue<float> TimeLeft { get; } = new();
    }
}