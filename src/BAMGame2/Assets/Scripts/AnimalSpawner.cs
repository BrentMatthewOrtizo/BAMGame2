using UnityEngine;
using System.Collections.Generic;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using Game399.Shared.Models;

public class AnimalSpawner : MonoBehaviour
{
    public static AnimalSpawner Instance { get; private set; }
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    [Header("Animal Prefabs (must match AnimalDefinition names)")]
    public List<AnimalDefinition> animalDefinitions;   // ScriptableObjects
    public List<GameObject> animalPrefabs;             // Prefabs with movement + animator

    [Header("Spawn Settings")]
    public Transform animalParent;
    public BoxCollider2D spawnRegion;                  // NEW: where animals are allowed to spawn

    private void Awake()
    {
        Instance = this;

        if (animalParent == null)
            Log.Warn("[AnimalSpawner] animalParent is not assigned!");

        if (spawnRegion == null)
            Log.Warn("[AnimalSpawner] spawnRegion BoxCollider2D is not assigned!");

        if (animalDefinitions.Count != animalPrefabs.Count)
            Log.Warn("[AnimalSpawner] Definitions and Prefabs count mismatch.");
    }

    private void Start()
    {
        RestoreAnimals();
    }
    
    public void TrySpawnAnimal(AnimalDefinition definition)
    {
        if (!PlayerAnimalInventory.Instance.OwnsAnimal(definition))
        {
            Log.Info($"[AnimalSpawner] Player does NOT own {definition.animalName}");
            return;
        }

        GameObject prefab = GetPrefab(definition);
        if (prefab == null)
        {
            Log.Error($"[AnimalSpawner] No prefab found for {definition.animalName}");
            return;
        }

        // Pick a valid spawn point from region
        Vector3 pos = GetRandomPointInRegion();

        // Spawn
        GameObject animal = Instantiate(prefab, pos, Quaternion.identity);
        if (animalParent != null)
            animal.transform.SetParent(animalParent);

        // Assign ScriptableObject reference
        AnimalMovement move = animal.GetComponent<AnimalMovement>();
        move.definition = definition;

        // Save persistence state
        SaveAnimalState(animal, definition);

        Log.Info($"[AnimalSpawner] Spawned {definition.animalName} at {pos}");
    }

    // =========================================================================
    //  SAVE ANIMAL STATE
    // =========================================================================
    public void SaveAnimalState(GameObject obj, AnimalDefinition def)
    {
        AnimalWorldData data = new AnimalWorldData
        {
            animalName = def.animalName,
            x = obj.transform.position.x,
            y = obj.transform.position.y
        };

        // Remove old entry, insert new latest
        GameStateManager.Instance.animals.RemoveAll(a => a.animalName == def.animalName);
        GameStateManager.Instance.animals.Add(data);
    }
    
    private void RestoreAnimals()
    {
        foreach (var data in GameStateManager.Instance.animals)
        {
            GameObject prefab = GetPrefabByName(data.animalName);
            if (prefab == null)
                continue;

            Vector3 pos = new Vector3(data.x, data.y, 0);

            GameObject animal = Instantiate(prefab, pos, Quaternion.identity);
            if (animalParent != null)
                animal.transform.SetParent(animalParent);

            // Restore definition reference
            AnimalMovement move = animal.GetComponent<AnimalMovement>();
            move.definition = animalDefinitions.Find(d => d.animalName == data.animalName);

            Log.Info($"[AnimalSpawner] Restored {data.animalName} at {pos}");
        }
    }


    private Vector3 GetRandomPointInRegion()
    {
        if (spawnRegion == null)
        {
            Log.Warn("[AnimalSpawner] No spawnRegion provided. Spawning at Vector3.zero");
            return Vector3.zero;
        }

        Bounds b = spawnRegion.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float y = Random.Range(b.min.y, b.max.y);

        return new Vector3(x, y, 0);
    }

    private GameObject GetPrefab(AnimalDefinition def)
    {
        for (int i = 0; i < animalDefinitions.Count; i++)
        {
            if (animalDefinitions[i] == def)
                return animalPrefabs[i];
        }

        return null;
    }

    private GameObject GetPrefabByName(string name)
    {
        for (int i = 0; i < animalDefinitions.Count; i++)
        {
            if (animalDefinitions[i].animalName == name)
                return animalPrefabs[i];
        }

        return null;
    }
}