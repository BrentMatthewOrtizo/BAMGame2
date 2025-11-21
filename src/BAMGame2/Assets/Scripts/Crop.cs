using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [Header("Watering State")]
    public bool isWatered = false;
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    private CropGrowth cropGrowth;

    private void Awake()
    {
        cropGrowth = GetComponent<CropGrowth>();
    }

    // Called when the player waters crops nearby
    public void WaterCrop()
    {
        if (isWatered)
        {
            Log.Info($"{name} is already watered.");
            return;
        }

        isWatered = true;
        Log.Info($"Watered {name}, growth starting.");

        if (cropGrowth != null)
        {
            // MVVM: this will update the CropModel via ICropService
            cropGrowth.SyncModelWaterState();
            cropGrowth.OnWateredFromService();
        }
    }

    // Property used by FarmManager
    public bool IsWatered => isWatered;
}