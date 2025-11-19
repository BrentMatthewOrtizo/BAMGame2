namespace Game399.Shared.Models
{
    /// <summary>
    /// Pure domain representation of a crop.
    /// Unity views (CropGrowth) observe this model.
    /// </summary>
    public class CropModel
    {
        public ObservableValue<int> Stage { get; } = new();
        public ObservableValue<float> TimeInStage { get; } = new();
        public ObservableValue<bool> IsWatered { get; } = new();
        public ObservableValue<bool> IsGrowing { get; } = new();

        public CropModel()
        {
            Stage.Value = 0;
            TimeInStage.Value = 0;
            IsWatered.Value = false;
            IsGrowing.Value = false;
        }
    }
}