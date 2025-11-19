using UnityEngine;

public enum CollectibleType
{
    Gold,
    Seed,
    Crop
}

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public CollectibleType type = CollectibleType.Gold;
    public int amount = 1;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Keep the original player-pickup behavior
        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    /// <summary>
    /// Shared pickup logic so both Player and Animals can collect items.
    /// </summary>
    public void Collect()
    {
        switch (type)
        {
            case CollectibleType.Gold:
                if (PlayerWallet.Instance != null)
                {
                    PlayerWallet.Instance.AddGold(amount);
                    Debug.Log($"[Collectible] Collected {amount} gold. " +
                              $"Total gold: {PlayerWallet.Instance.gold}");
                }
                else
                {
                    Debug.LogWarning("[Collectible] PlayerWallet.Instance is null – cannot track gold.");
                }
                break;

            case CollectibleType.Seed:
                Debug.Log("[Collectible] Seed collected (inventory feature pending).");
                break;

            case CollectibleType.Crop:
                Debug.Log("[Collectible] Crop collected (inventory feature pending).");
                break;
        }

        Destroy(gameObject);
    }
    
    public void CollectByAnimal()
    {
        // Simulate what happens when player picks it up
        PlayerWallet.Instance.AddGold(amount);

        // You may also add seeds/items to inventory here

        Destroy(gameObject);
    }
}