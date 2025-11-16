using System.Collections;
using Game399.Shared.Models;
using Game399.Shared.Services;
using Game.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class CropGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    public Sprite[] growthStages;
    public float timePerStage = 1f;

    [Header("Drops")]
    public GameObject goldPrefab;
    public GameObject cropPrefab;
    public GameObject seedPrefab;

    private SpriteRenderer _sr;
    private int _stage = 0;
    private float _timer = 0f;
    private bool _isGrowing = false;

    private Vector3 _worldPos;
    private FarmArea _farmArea;

    private Coroutine growthRoutine;

    private Crop crop; // reference to watering state
    private bool wasGrowingBeforeSceneChange = false;

    // MVVM: domain model + service
    private CropModel _model;
    private ICropService _cropService;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        crop = GetComponent<Crop>();

        // Resolve crop service once
        _cropService = ServiceResolver.Resolve<ICropService>();
        EnsureModel();
    }

    private void EnsureModel()
    {
        if (_model != null) return;

        _model = _cropService.CreateCrop();
        _model.Stage.ChangeEvent += OnStageChanged;
    }

    private void OnStageChanged(int newStage)
    {
        _stage = newStage;
        UpdateSprite();
    }

    // Called when first planted
    public void Initialize(FarmManager manager, FarmArea area, Vector3 position)
    {
        _worldPos = position;
        _farmArea = area;
        _stage = 0;
        _timer = 0f;
        _isGrowing = false; // must be watered first

        manager.Register(this);

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        EnsureModel();
        _model.Stage.Value = _stage;

        UpdateSprite();
    }

    // Called when loading from save
    public void LoadFromData(FarmManager manager, Vector3 position, int stage, float elapsed)
    {
        _worldPos = position;
        _stage = stage;
        _timer = elapsed;
        _isGrowing = false; // must be watered again unless you add saving of watered state

        manager.Register(this);

        EnsureModel();
        _model.Stage.Value = _stage;

        UpdateSprite();
    }

    /// <summary>
    /// Called by Crop when the player waters this crop.
    /// Uses ICropService to update the domain model, then starts growth.
    /// </summary>
    public void OnWateredFromService()
    {
        EnsureModel();

        if (_model.IsWatered.Value)
        {
            Debug.Log($"💧 {name} is already watered (model).");
            return;
        }

        _cropService.WaterCrop(_model); // sets IsWatered + IsGrowing

        if (_isGrowing) return;

        _isGrowing = true;
        growthRoutine = StartCoroutine(Grow());
        Debug.Log($"🌱 {name} started growing!");
    }

    private IEnumerator Grow()
    {
        int finalStage = growthStages.Length - 1;

        while (_isGrowing)
        {
            // Show the current stage
            UpdateSprite();

            // If we are at the final stage, wait 1 second so the player SEES it,
            // then finish growth
            if (_stage == finalStage)
            {
                yield return new WaitForSeconds(timePerStage);
                FinishGrowth();
                yield break;
            }

            // Wait for stage duration
            yield return new WaitForSeconds(timePerStage);

            // Now increment to next stage
            _stage++;
        }
    }

    private void FinishGrowth()
    {
        SpawnDrops();

        if (_farmArea != null)
            _farmArea.Unregister(_worldPos);

        if (FarmManager.Instance != null)
            FarmManager.Instance.Unregister(this);

        Destroy(gameObject);
    }

    private void UpdateSprite()
    {
        if (_sr == null || growthStages.Length == 0)
            return;

        int index = Mathf.Clamp(_stage, 0, growthStages.Length - 1);
        _sr.sprite = growthStages[index];
    }

    private void SpawnDrops()
    {
        if (goldPrefab) SpawnWithScatter(goldPrefab, 0.2f);
        if (cropPrefab) SpawnWithScatter(cropPrefab, 0.3f);
        if (seedPrefab) SpawnWithScatter(seedPrefab, 0.25f);
    }

    private void SpawnWithScatter(GameObject prefab, float spread)
    {
        Vector2 offset = Random.insideUnitCircle * spread;
        Vector3 spawnPos = _worldPos + new Vector3(offset.x, offset.y, 0f);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    public CropData GetData()
    {
        return new CropData
        {
            worldPos = _worldPos,
            currentStage = _stage,
            timeElapsed = _timer
        };
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.Stage.ChangeEvent -= OnStageChanged;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_sr == null) return;

        if (scene.name == "Battle")
        {
            _sr.enabled = false;

            // Remember if it was growing
            wasGrowingBeforeSceneChange = _isGrowing;

            // Stop growth coroutine
            _isGrowing = false;

            if (growthRoutine != null)
            {
                StopCoroutine(growthRoutine);
                growthRoutine = null;
            }

            return;
        }

        if (scene.name == "Game")
        {
            _sr.enabled = true;
            UpdateSprite();

            // Resume growth if it was previously growing and not finished
            if (wasGrowingBeforeSceneChange &&
                _stage < growthStages.Length &&
                _model != null &&
                _model.IsWatered.Value)
            {
                _isGrowing = true;
                growthRoutine = StartCoroutine(Grow());
            }
        }
    }
}