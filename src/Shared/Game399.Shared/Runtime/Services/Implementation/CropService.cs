using Game399.Shared.Diagnostics;
using Game399.Shared.Models;

namespace Game399.Shared.Services.Implementation
{
    public class CropService : ICropService
    {
        private readonly IGameLog _log;

        public CropService(IGameLog log)
        {
            _log = log;
        }

        public CropModel CreateCrop()
        {
            var model = new CropModel();
            _log.Info("Created new CropModel.");
            return model;
        }

        public void WaterCrop(CropModel crop)
        {
            if (crop == null) return;

            if (crop.IsWatered.Value)
            {
                _log.Info("Tried to water an already watered crop.");
                return;
            }

            crop.IsWatered.Value = true;
            crop.IsGrowing.Value = true;
            _log.Info("Crop watered – growth started.");
        }

        public void AdvanceStage(CropModel crop, int maxStageIndex)
        {
            if (crop == null) return;
            if (!crop.IsGrowing.Value) return;

            var nextStage = crop.Stage.Value + 1;
            crop.Stage.Value = nextStage;
            crop.TimeInStage.Value = 0;

            _log.Info($"Crop advanced to stage {nextStage}.");

            if (nextStage >= maxStageIndex)
            {
                crop.IsGrowing.Value = false;
                _log.Info("Crop finished growing.");
            }
        }
    }
}