using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropData
{
    public Vector3Int cell;
    public int currentStage;
    public float timeElapsed;
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Player")]
    public Vector3 playerPosition;

    [Header("Crops")]
    public List<CropData> crops = new();

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

    public void SavePlayer(Vector3 pos) => playerPosition = pos;
    public void SaveCrops(List<CropData> cropList) => crops = cropList;
    public void ClearCrops() => crops.Clear();
}