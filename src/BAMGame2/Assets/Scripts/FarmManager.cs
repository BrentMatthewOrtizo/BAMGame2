using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.LogWarning("Duplicate FarmManager detected! Destroying duplicate");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"FarmManager initialized (scene: {gameObject.scene.name})");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Only load crops when returning to Game scene
        if (scene.name == "Game")
        {
            LoadCrops();
        }
    }

    private void OnDisable()
    {
        if (Instance != this) return; // don’t save from duplicate
        Debug.Log("FarmManager OnDisable called, saving crops...");
        SaveCrops();
    }

    public void Register(CropGrowth crop)
    {
        if (!_activeCrops.Contains(crop))
        {
            _activeCrops.Add(crop);
            Debug.Log($"Registered crop: {crop.name} at {crop.transform.position}");
        }
    }

    public void Unregister(CropGrowth crop)
    {
        _activeCrops.Remove(crop);
    }

    public void MarkHarvested(Vector3 pos)
    {
        GameStateManager.Instance?.crops.RemoveAll(c => Vector3.Distance(c.worldPos, pos) < 0.1f);
    }

    public void SaveCrops()
    {
        if (GameStateManager.Instance == null) return;

        var cropDataList = new List<CropData>();
        foreach (var crop in _activeCrops)
        {
            if (crop != null)
                cropDataList.Add(crop.GetData());
        }

        GameStateManager.Instance.SaveCrops(cropDataList);
        Debug.Log($"Saved {cropDataList.Count} crops");
    }

    private void LoadCrops()
    {
        if (GameStateManager.Instance == null || cropPrefab == null)
            return;

        foreach (var data in GameStateManager.Instance.crops)
        {
            var go = Instantiate(cropPrefab, data.worldPos, Quaternion.identity, cropParent);
            var crop = go.GetComponent<CropGrowth>();
            crop.LoadFromData(this, data.worldPos, data.currentStage, data.timeElapsed);
            _activeCrops.Add(crop);
        }

        Debug.Log($"Loaded {GameStateManager.Instance.crops.Count} crops");
    }
}
