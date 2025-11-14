using System.Collections.Generic;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HotbarController : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 5;
    
    private ItemDictionary itemDictionary;
    public List<InventorySaveData> inventorySaveData;

    private Key[] hotbarKeys; //1-5 on keyboard

    private void CreateSlots()
    {
        //create slots
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, hotbarPanel.transform).GetComponent<Slot>();
            slot.currentItem = null;
        }
    }
    
    private void Awake()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        CreateSlots();
        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = (Key)((int)Key.Digit1 + i);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    

    void Update()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                //Use item in slot i
                UseItemInSlot(i);
            }
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            hotbarPanel.SetActive(true);

            if (inventorySaveData != null && inventorySaveData.Count > 0)
            {
                SetHotbarItems(inventorySaveData);
            }
            else
            {
                Log.Warn("No hotbar data found to be loaded.");
            }
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        hotbarPanel.SetActive(false);
        
        // Only save if we’re unloading the scene that actually has the UI
        if (scene.name == "Game" && hotbarPanel != null)
        {
            inventorySaveData = GetHotbarItems();
            Log.Info($"Saved {inventorySaveData.Count} hotbar slots.");
        }
    }
    
    private void OnDestroy()
    {
        // Important: remove event subscription to avoid duplicates
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void UseItemInSlot(int i)
    {
        Slot slot = hotbarPanel.transform.GetChild(i).GetComponent<Slot>();
        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            item.UseItem(); 
        }
    }
    
    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null) //has item
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData
                {
                    itemID = item.ID, 
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity  = item.quantity
                });
            }
        }
        return hotbarData;
    }
    
    // for loading
    public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
    {
        //create missing slots
        int existing = hotbarPanel.transform.childCount;
        for (int i = existing; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }
        
        //clear all items
        for (int i = 0; i < hotbarPanel.transform.childCount; i++)
        {
            Slot slot = hotbarPanel.transform.GetChild(i).GetComponent<Slot>();
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
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
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
