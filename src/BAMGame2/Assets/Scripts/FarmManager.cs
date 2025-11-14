using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

    [Header("Crop Settings")]
    public GameObject cropPrefab;
    public Transform cropParent;

    [Header("Watering Settings")]
    public float wateringRadius = 2.5f; // How far the watering reaches
    public LayerMask cropLayer; // assign "Crop" layer in Inspector

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
        // Only load crops when returning to Game scene
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

        // ❗ Do NOT load if we still have active crops from DontDestroyOnLoad
        if (_activeCrops.Count > 0)
        {
            Debug.Log("Skipping crop load — crops already exist (preventing duplicates)");
            return;
        }

        foreach (var data in GameStateManager.Instance.crops)
        {
            var go = Instantiate(cropPrefab, data.worldPos, Quaternion.identity, cropParent);
            var crop = go.GetComponent<CropGrowth>();
            crop.LoadFromData(this, data.worldPos, data.currentStage, data.timeElapsed);
            _activeCrops.Add(crop);
        }

        Debug.Log($"Loaded {GameStateManager.Instance.crops.Count} crops");
    }

    // 🪣 Watering System
    public void WaterNearbyCrops(Vector3 playerPosition, float radius = 1.5f)
    {
        bool anyWatered = false;
        foreach (var crop in _activeCrops)
        {
            if (crop == null) continue;

            float distance = Vector3.Distance(crop.transform.position, playerPosition);
            Crop cropComp = crop.GetComponent<Crop>();

            if (distance <= radius && cropComp != null && !cropComp.IsWatered)
            {
                cropComp.WaterCrop();
                anyWatered = true;
            }
        }

        if (!anyWatered)
            Debug.Log("💧 No dry crops nearby!");
    }

    // Draw watering radius in Scene view for debugging
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(player.transform.position, wateringRadius);
            }
        }
    }
}