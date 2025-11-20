using UnityEngine;

public class Crop : MonoBehaviour
{
    [Header("Watering State")]
    public bool isWatered = false;

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
            Debug.Log($"💧 {name} is already watered.");
            return;
        }

        isWatered = true;
        Debug.Log($"💧 Watered {name}, growth starting...");

        if (cropGrowth != null)
        {
            // MVVM: this will update the CropModel via ICropService
            cropGrowth.OnWateredFromService();
        }
    }

    // Property used by FarmManager
    public bool IsWatered => isWatered;
}