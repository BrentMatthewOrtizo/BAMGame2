namespace Game399.Shared.Models
{
    /// <summary>
    /// Domain model for day/night & timer.
    /// </summary>
    public class WorldStateModel
    {
        public ObservableValue<bool> IsDay { get; } = new(true);
        public ObservableValue<float> TimeLeft { get; } = new();
    }
}