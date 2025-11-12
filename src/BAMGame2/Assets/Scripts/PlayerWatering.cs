using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWatering : MonoBehaviour
{
    public void OnWater(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (FarmManager.Instance == null)
        {
            Debug.LogWarning("💧 No FarmManager found — cannot water crops.");
            return;
        }

        // Use FarmManager’s watering radius setting
        FarmManager.Instance.WaterNearbyCrops(transform.position, FarmManager.Instance.wateringRadius);
        Debug.Log($"💧 Watered crops near {transform.position}");
    }

    // Optional: visualization aid
    private void OnDrawGizmosSelected()
    {
        if (FarmManager.Instance != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, FarmManager.Instance.wateringRadius);
        }
    }
}