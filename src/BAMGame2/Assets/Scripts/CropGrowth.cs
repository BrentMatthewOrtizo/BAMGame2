using System.Collections;
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

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        crop = GetComponent<Crop>();
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

        UpdateSprite();
    }

    // Called when loading from save
    public void LoadFromData(FarmManager manager, Vector3 position, int stage, float elapsed)
    {
        _worldPos = position;
        _stage = stage;
        _timer = elapsed;
        _isGrowing = false; // must be watered again unless you add saving

        UpdateSprite();

        manager.Register(this);
    }

    // Start growth when watered
    public void BeginGrowth()
    {
        if (_isGrowing)
            return;

        _isGrowing = true;
        growthRoutine = StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        while (_isGrowing && _stage < growthStages.Length)
        {
            UpdateSprite();

            float waitTime = Mathf.Max(0.1f, timePerStage - _timer);
            yield return new WaitForSeconds(waitTime);

            _timer = 0f;
            _stage++;
        }

        // If fully grown, spawn drops
        if (_stage >= growthStages.Length)
        {
            SpawnDrops();

            if (_farmArea != null)
                _farmArea.Unregister(_worldPos);

            FarmManager.Instance.Unregister(this);

            Destroy(gameObject);
        }
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_sr == null) return;

        if (scene.name == "Battle")
        {
            _sr.enabled = false;

            // Remember if it was growing
            wasGrowingBeforeSceneChange = _isGrowing;

            // Stop growth
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
            
            if (crop != null &&
                crop.isWatered &&
                wasGrowingBeforeSceneChange &&
                _stage < growthStages.Length)
            {
                _isGrowing = true;
                growthRoutine = StartCoroutine(Grow());
            }
        }
    }
}