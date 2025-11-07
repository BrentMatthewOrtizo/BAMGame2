using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Player")]
    public Vector3 playerPosition;

    [Header("Crops")]
    public List<CropData> crops = new();  //Stores crop data for saving/loading

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

    // --- PLAYER ---
    public void SavePlayer(Vector3 pos)
    {
        playerPosition = pos;
    }

    // --- CROPS ---
    public void SaveCrops(List<CropData> cropList)
    {
        crops = cropList;
    }

    public void ClearCrops()
    {
        crops.Clear();
    }
}