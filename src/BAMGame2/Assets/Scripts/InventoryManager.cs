using System.Collections.Generic;
using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static InventoryManager Instance { get; private set; }
    private ItemDictionary itemDictionary;

    public GameObject inventoryTab;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public List<InventorySaveData> inventorySaveData;

    [Header("Inventory Items")]
    public GameObject[] itemPrefabs; //for testing
    
// Optional: event system to notify UI or other systems
    //public delegate void InventoryChanged(List<string> newInventory);
    //public event InventoryChanged OnInventoryChanged;

    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        //create slots
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
        inventoryPanel.SetActive(false);
        inventoryTab.SetActive(false);
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
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDestroy()
    {
        // Important: remove event subscription to avoid duplicates
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void Update()
    {
        if (Keyboard.current?.tabKey.wasPressedThisFrame == true)
        {
            if (inventoryPanel == null)
            {
                Log.Info("Inventory panel reference is missing!");
                return;
            }
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            inventoryTab.SetActive(!inventoryTab.activeSelf);
            AudioManager.Instance.PlayMousePressSFX();
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            inventoryPanel.SetActive(false);
            inventoryTab.SetActive(false);

            if (inventorySaveData != null && inventorySaveData.Count > 0)
            {
                SetInventoryItems(inventorySaveData);
            }
            else
            {
                Log.Warn("No inventory data found to be loaded.");
            }
        }
    }
    
    private void OnSceneUnloaded(Scene scene)
    {
        inventoryPanel.SetActive(false);
        inventoryTab.SetActive(false);
        
        // Only save if we’re unloading the scene that actually has the UI
        if (scene.name == "Game" && inventoryPanel != null)
        {
            inventorySaveData = GetInventoryItems();
            Log.Info($"Saved {inventorySaveData.Count} inventory entries.");
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if (itemToAdd == null)
        {
            return false;
        }
        // check if item type is already in inventory
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null) //has slot and is occupied
            {
                if (slot.currentItem.GetComponent<Collectible>().type == itemToAdd.GetComponent<Collectible>().type)
                {
                    //same item, stack item
                    Item slotItem = slot.currentItem.GetComponent<Item>();
                    slotItem.AddToStack();
                    return true;
                }
            }
        }
        
        //look for empty slot
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null) //has slot and is an empty slot
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
        }
        Log.Warn("Inventory is full!");
        return false;
    }
    
    // for saving
    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null) //has item
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID, 
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity  = item.quantity
                });
            }
        }
        return invData;
    }
    
    // for loading
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        //create missing slots
        int existing = inventoryPanel.transform.childCount;
        for (int i = existing; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
        
        //clear all items
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
            Slot slot = inventoryPanel.transform.GetChild(i).GetComponent<Slot>();
            if (!slot) continue;

            if (slot.currentItem != null)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }

            // safety: also destroy any remaining children under slot (icons, etc.)
            for (int c = slot.transform.childCount - 1; c >= 0; c--)
                Destroy(slot.transform.GetChild(c).gameObject);
        }
        
        //populate slots with saved data
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                    if (item != null)
                    {
                        item.GetComponent<Item>().quantity = Mathf.Max(1, data.quantity); // restore quantity
                        item.GetComponent<Item>().UpdateQuantityDisplay();
                    }
                }
            }
        }
    }
}