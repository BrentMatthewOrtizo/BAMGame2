using Game399.Shared.Models;

namespace Game399.Shared.Services
{
    // Domain logic for crop growth & watering.
    public interface ICropService
    {
        CropModel CreateCrop();
        void WaterCrop(CropModel crop);
        void AdvanceStage(CropModel crop, int maxStageIndex);
    }
}