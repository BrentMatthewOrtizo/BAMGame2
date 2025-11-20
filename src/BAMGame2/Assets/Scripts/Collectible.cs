using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;

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

    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
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
                    Log.Info($"[Collectible] Player collected {amount} gold. " +
                              $"Total gold: {PlayerWallet.Instance.gold}");
                    Destroy(gameObject); //maybe move later
                }
                else
                {
                    Log.Warn("[Collectible] PlayerWallet.Instance is null – cannot track gold.");
                }
                break;

            case CollectibleType.Crop:
            case CollectibleType.Seed:
                if (InventoryManager.Instance != null)
                {
                    Log.Info("[Collectible] Player collected an item.");
                    
                    Item item = gameObject.GetComponent<Item>(); //maybe?
                    if (item != null)
                    {
                        //add item to list
                        bool itemAdded = InventoryManager.Instance.AddItem(gameObject); //maybe?

                        if (itemAdded)
                        {
                            //add to itemPrefabs array in inventorymanager?
                            gameObject.GetComponent<Item>().PickUp();
                            Destroy(gameObject); 
                            AudioManager.Instance.PlayItemPickupSFX();
                        }
                    }
                    
                } 
                else
                {
                    Log.Warn("[Collectible] InventoryManager.Instance is null – cannot track Seeds.");
                }
                break;
        }
    }
    
    public void CollectByAnimal()
    {
        // Simulate what happens when player picks it up
        PlayerWallet.Instance.AddGold(amount);

        // You may also add seeds/items to inventory here

        Destroy(gameObject);
    }
}