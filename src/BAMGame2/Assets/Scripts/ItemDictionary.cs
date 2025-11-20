using System.Collections.Generic;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1; //auto increment ID for each item prefab
            }
        }

        // populate itemDictionary
        foreach (Item item in itemPrefabs)
        {
            itemDictionary[item.ID] =  item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab); //instead of crashing, will give null gameobject if itemID does not exist
        if (prefab == null)
        {
            Log.Warn($"Item with ID {itemID} does not exist.");
        }
        return prefab;
    }
}
