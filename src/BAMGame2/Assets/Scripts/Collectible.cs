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
                    PlayerWallet.Instance.AddGold(amount);
                break;

            case CollectibleType.Seed:
                // will hook into inventory later
                break;

            case CollectibleType.Crop:
                // will hook into inventory later
                break;
        }

        Destroy(gameObject);
    }
}