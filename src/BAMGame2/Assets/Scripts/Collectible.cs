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
        if (!other.CompareTag("Player"))
            return;

        switch (type)
        {
            case CollectibleType.Gold:
                if (PlayerWallet.Instance != null)
                {
                    PlayerWallet.Instance.AddGold(amount);
                    Debug.Log($"[Collectible] Player collected {amount} gold. " +
                              $"Total gold: {PlayerWallet.Instance.gold}");
                }
                else
                {
                    Debug.LogWarning("[Collectible] PlayerWallet.Instance is null – cannot track gold.");
                }
                break;

            case CollectibleType.Seed:
                Debug.Log("[Collectible] Player picked up a seed (inventory feature pending).");
                break;

            case CollectibleType.Crop:
                Debug.Log("[Collectible] Player picked up a crop (inventory feature pending).");
                break;
        }

        Destroy(gameObject);
    }
}