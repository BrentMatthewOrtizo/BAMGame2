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
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        hotbarPanel.SetActive(false);
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
}
