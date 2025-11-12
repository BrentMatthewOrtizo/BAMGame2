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
        if (!other.CompareTag("Player"))
            return;

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

            case CollectibleType.Seed:
                if (InventoryManager.Instance != null)
                {
                    Log.Info("[Collectible] Player picked up a seed (inventory feature pending).");
                    
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
                    
                    // InventoryManager.Instance.AddItem("Seed"); //add item to list
                } 
                else
                {
                    Log.Warn("[Collectible] InventoryManager.Instance is null – cannot track Seeds.");
                }
                break;

            case CollectibleType.Crop:
                if (InventoryManager.Instance != null)
                {
                    // implement inventory for crops after solidifying seeds
                    Log.Info("[Collectible] Player picked up a crop (inventory feature pending).");
                    Destroy(gameObject); //move later
                }
                else
                {
                    Log.Warn("[Collectible] InventoryManager.Instance is null – cannot track Crops.");
                }
                break;
        }

        // Destroy(gameObject);
    }
}