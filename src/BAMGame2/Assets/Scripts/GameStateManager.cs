using System.Collections.Generic;
using UnityEngine;
using Game399.Shared.Models;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Player")]
    public Vector3 playerPosition;

    [Header("Crops")]
    public List<CropData> crops = new();

    [Header("Animals")]
    public List<AnimalWorldData> animals = new();

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

    // PLAYER
    public void SavePlayer(Vector3 pos) => playerPosition = pos;

    // CROPS
    public void SaveCrops(List<CropData> cropList) => crops = cropList;
    public void ClearCrops() => crops.Clear();

    // ANIMALS
    public void SaveAnimals(List<AnimalWorldData> animalList) => animals = animalList;
    public void ClearAnimals() => animals.Clear();
}