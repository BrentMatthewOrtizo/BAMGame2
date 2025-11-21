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

    private int activeSlotIndex = -1;  // -1 means none selected
    private Key[] hotbarKeys;

    private void Awake()
    {
        CreateSlots();

        // Set keys 1–5
        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
            hotbarKeys[i] = (Key)((int)Key.Digit1 + i);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Update()
    {
        // Hotbar key selection
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                activeSlotIndex = i;
                Log.Info($"Active hotbar slot = {i + 1}");
            }
        }
    }

    private void CreateSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, hotbarPanel.transform)
                        .GetComponent<Slot>();
            slot.currentItem = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
            hotbarPanel.SetActive(true);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        hotbarPanel.SetActive(false);
    }

    // Called by FarmArea
    public Item GetActiveHotbarItem()
    {
        if (activeSlotIndex < 0)
            return null;

        Slot slot = hotbarPanel.transform.GetChild(activeSlotIndex).GetComponent<Slot>();
        if (slot == null || slot.currentItem == null)
            return null;

        return slot.currentItem.GetComponent<Item>();
    }

    // Consume 1 seed from active slot when planting
    public bool ConsumeSeedFromHotbar()
    {
        Item item = GetActiveHotbarItem();
        if (item == null) return false;
        if (item.Name != "Seed") return false;

        int removed = item.RemoveFromStack(1);
        if (removed == 0) return false;

        // If empty, remove item completely
        if (item.quantity <= 0)
        {
            Slot slot = hotbarPanel.transform.GetChild(activeSlotIndex).GetComponent<Slot>();
            slot.currentItem = null;
            Destroy(item.gameObject);
        }

        return true;
    }
}