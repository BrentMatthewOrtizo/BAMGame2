using System.Collections;
using UnityEngine;

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

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // Called when first planted
    public void Initialize(FarmManager manager, FarmArea area, Vector3 position)
    {
        _sr = GetComponent<SpriteRenderer>();
        _worldPos = position;
        _farmArea = area;
        _stage = 0;
        _timer = 0f;
        _isGrowing = true;

        manager.Register(this);
        StartCoroutine(Grow(manager));
    }

    // Called when loading from save
    public void LoadFromData(FarmManager manager, Vector3 position, int stage, float elapsed)
    {
        _sr = GetComponent<SpriteRenderer>();
        _worldPos = position;
        _stage = stage;
        _timer = elapsed;
        _isGrowing = true;

        _sr.sprite = growthStages[Mathf.Min(stage, growthStages.Length - 1)];
        manager.Register(this);
        StartCoroutine(Grow(manager));
    }

    private IEnumerator Grow(FarmManager manager)
    {
        while (_isGrowing && _stage < growthStages.Length)
        {
            _sr.sprite = growthStages[_stage];
            yield return new WaitForSeconds(timePerStage - _timer);
            _timer = 0f;
            _stage++;
        }

        // 🌾 Crop fully grown — spawn scattered drops
        SpawnDrops();

        // 🧹 Let the farm area know this spot is free again
        if (_farmArea != null)
            _farmArea.Unregister(_worldPos);

        manager.Unregister(this);
        Destroy(gameObject);
    }

    private void SpawnDrops()
    {
        if (goldPrefab) SpawnWithScatter(goldPrefab, 0.2f);
        if (cropPrefab) SpawnWithScatter(cropPrefab, 0.3f);
        if (seedPrefab) SpawnWithScatter(seedPrefab, 0.25f);
    }

    private void SpawnWithScatter(GameObject prefab, float spread)
    {
        Vector2 randomOffset = Random.insideUnitCircle * spread;
        Vector3 spawnPos = _worldPos + new Vector3(randomOffset.x, randomOffset.y, 0f);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    // Used by FarmManager for saving crop data
    public CropData GetData()
    {
        return new CropData
        {
            worldPos = _worldPos,
            currentStage = _stage,
            timeElapsed = _timer
        };
    }
}