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

    public GameObject inventoryTab;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    [Header("Inventory Items")]

    //private List<GameObject> itemPrefabList = new List<GameObject>(); 

    public GameObject[] itemPrefabs; //for testing
    
// Optional: event system to notify UI or other systems
    //public delegate void InventoryChanged(List<string> newInventory);
    //public event InventoryChanged OnInventoryChanged;

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
    }
    private void OnDestroy()
    {
        // Important: remove event subscription to avoid duplicates
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
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
}