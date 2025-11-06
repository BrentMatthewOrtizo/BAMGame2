using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

    [Header("Crop Settings")]
    public GameObject cropPrefab;
    public Transform cropParent;

    private readonly List<CropGrowth> _activeCrops = new();

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

    private void Start()
    {
        LoadCrops();
    }

    private void OnDisable()
    {
        SaveCrops();
    }

    public void Register(CropGrowth crop)
    {
        if (!_activeCrops.Contains(crop))
            _activeCrops.Add(crop);
    }

    public void Unregister(CropGrowth crop)
    {
        _activeCrops.Remove(crop);
    }

    private void SaveCrops()
    {
        if (GameStateManager.Instance == null) return;

        var cropDataList = new List<CropData>();
        foreach (var crop in _activeCrops)
        {
            if (crop != null)
                cropDataList.Add(crop.GetData());
        }

        GameStateManager.Instance.SaveCrops(cropDataList);
        Debug.Log($"💾 Saved {cropDataList.Count} crops");
    }

    private void LoadCrops()
    {
        if (GameStateManager.Instance == null || cropPrefab == null)
            return;

        foreach (var data in GameStateManager.Instance.crops)
        {
            var go = Instantiate(cropPrefab, data.worldPos, Quaternion.identity, cropParent);
            var crop = go.GetComponent<CropGrowth>();
            crop.LoadFromData(this, data.currentStage, data.timeElapsed);
            _activeCrops.Add(crop);
        }

        Debug.Log($"🌿 Loaded {GameStateManager.Instance.crops.Count} crops");
    }
}