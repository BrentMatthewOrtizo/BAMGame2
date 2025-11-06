using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FarmTileInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private FarmingGrid farmingGrid;

    public bool CanInteract() => true;

    public void Interact()
    {
        Debug.Log("✅ FarmTileInteractable.Interact() called!");
        if (farmingGrid == null)
        {
            Debug.LogWarning("⚠️ farmingGrid is not assigned in inspector!");
            return;
        }

        farmingGrid.PlantAt(transform.position);
    }
}