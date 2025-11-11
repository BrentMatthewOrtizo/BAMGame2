using System.Collections.Generic;
using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static InventoryManager Instance { get; private set; }

    public GameObject inventoryCanvas;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    [Header("Inventory Items")]
    [SerializeField] private List<string> items = new List<string>(); 
    //change to List<GameObject> itemPrefabs = new List<GameObject>() ??

    public GameObject[] itemPrefabs; //for testing

    // Optional: event system to notify UI or other systems
    public delegate void InventoryChanged(List<string> newInventory);
    public event InventoryChanged OnInventoryChanged;

    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //makes sure item is in the center of the slot
                slot.currentItem = item;
            }
        }
        inventoryCanvas.SetActive(false);
    }
    
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

    void Update()
    {
        if (Keyboard.current?.tabKey.wasPressedThisFrame == true)
        {
            //inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
            if (inventoryCanvas == null)
            {
                Log.Info("Inventory panel reference is missing!");
                return;
            }
            inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
        }
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