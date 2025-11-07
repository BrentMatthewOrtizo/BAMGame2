using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("NPC Settings")]
    public string npcName = "Farmer Bob";

    public bool CanInteract() => true;

    public void Interact()
    {
        Debug.Log($"{npcName}: Toggling shop UI...");

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ToggleShop();
        }
        else
        {
            Debug.LogWarning("[NPCInteractable] No ShopManager instance found in scene.");
        }
    }
}