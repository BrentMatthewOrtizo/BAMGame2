using System.Collections.Generic;
using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class InventoryManager : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Items")]
    [SerializeField] private List<string> items = new List<string>();

    // Optional: event system to notify UI or other systems
    public delegate void InventoryChanged(List<string> newInventory);
    public event InventoryChanged OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds an item to the inventory list.
    /// </summary>
    public void AddItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
            return;

        items.Add(itemName);
        OnInventoryChanged?.Invoke(items);
        Log.Info($"[Inventory] Added item: {itemName}. Total items: {items.Count}");
    }

    /// <summary>
    /// Removes an item from the inventory list.
    /// </summary>
    public void RemoveItem(string itemName)
    {
        if (items.Remove(itemName))
        {
            OnInventoryChanged?.Invoke(items);
            Log.Info($"[Inventory] Removed item: {itemName}. Total items: {items.Count}");
        }
    }

    /// <summary>
    /// Checks if the inventory contains a specific item.
    /// </summary>
    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    /// <summary>
    /// Returns a copy of the current inventory list.
    /// </summary>
    public List<string> GetItems()
    {
        return new List<string>(items);
    }
}