using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("NPC Settings")]
    public string npcName = "Farmer Bob";
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    public bool CanInteract() => true;

    public void Interact()
    {
        Log.Info($"{npcName}: Toggling shop UI...");

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ToggleShop();
        }
        else
        {
            Log.Warn("[NPCInteractable] No ShopManager instance found in scene.");
        }
    }
}