using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimalInventory : MonoBehaviour
{
    public static PlayerAnimalInventory Instance { get; private set; }

    [Tooltip("List of animals the player currently owns..")]
    public List<AnimalDefinition> ownedAnimals = new List<AnimalDefinition>();

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

    public bool OwnsAnimal(AnimalDefinition def)
    {
        return ownedAnimals.Contains(def);
    }

    public void AddAnimal(AnimalDefinition def)
    {
        if (!ownedAnimals.Contains(def))
        {
            ownedAnimals.Add(def);
            Debug.Log($"[AnimalInventory] Added: {def.animalName}");
        }
    }
}