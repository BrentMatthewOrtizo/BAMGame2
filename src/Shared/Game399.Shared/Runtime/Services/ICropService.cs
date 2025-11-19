using Game399.Shared.Models;

namespace Game399.Shared.Services
{
    /// <summary>
    /// Domain logic for crop growth & watering.
    /// </summary>
    public interface ICropService
    {
        CropModel CreateCrop();
        void WaterCrop(CropModel crop);
        void AdvanceStage(CropModel crop, int maxStageIndex);
    }
}