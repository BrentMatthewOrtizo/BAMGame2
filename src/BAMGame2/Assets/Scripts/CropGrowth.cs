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

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // Called when first planted
    public void Initialize(FarmManager manager)
    {
        _sr = GetComponent<SpriteRenderer>();
        _stage = 0;
        _timer = 0f;
        _isGrowing = true;

        manager.Register(this);
        StartCoroutine(Grow(manager));
    }

    // Called when loading from save
    public void LoadFromData(FarmManager manager, int stage, float elapsed)
    {
        _sr = GetComponent<SpriteRenderer>();
        _stage = stage;
        _timer = elapsed;

        _sr.sprite = growthStages[Mathf.Min(stage, growthStages.Length - 1)];
        _isGrowing = true;

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

        // 🌾 Crop fully grown — spawn items
        Vector3 p = transform.position;

        if (goldPrefab)
            Instantiate(goldPrefab, p + new Vector3(0.1f, 0.05f), Quaternion.identity);

        if (cropPrefab)
            Instantiate(cropPrefab, p, Quaternion.identity);

        if (seedPrefab)
            Instantiate(seedPrefab, p + new Vector3(-0.1f, -0.05f), Quaternion.identity);

        manager.Unregister(this);
        Destroy(gameObject);
    }

    // Used by FarmManager when saving game state
    public CropData GetData()
    {
        return new CropData
        {
            worldPos = transform.position,
            currentStage = _stage,
            timeElapsed = _timer
        };
    }
}
