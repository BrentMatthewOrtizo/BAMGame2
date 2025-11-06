using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CropGrowth : MonoBehaviour
{
    public Sprite[] growthStages;
    public float timePerStage = 1f;

    public GameObject goldPrefab;
    public GameObject cropPrefab;
    public GameObject seedPrefab;

    private SpriteRenderer _sr;
    private FarmingGrid _grid;
    private Vector3Int _cell;

    private int _stage = 0;
    private float _timer = 0f;

    public void Initialize(FarmingGrid grid, Vector3Int cell)
    {
        _grid = grid;
        _cell = cell;
        _sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Grow());
    }

    public void LoadFromData(FarmingGrid grid, Vector3Int cell, int stage, float elapsed)
    {
        _grid = grid;
        _cell = cell;
        _sr = GetComponent<SpriteRenderer>();
        _stage = stage;
        _timer = elapsed;
        _sr.sprite = growthStages[Mathf.Min(stage, growthStages.Length - 1)];
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        while (_stage < growthStages.Length)
        {
            _sr.sprite = growthStages[_stage];
            yield return new WaitForSeconds(timePerStage - _timer);
            _timer = 0f;
            _stage++;
        }

        // spawn drops
        Vector3 p = transform.position;
        if (goldPrefab) Instantiate(goldPrefab, p + new Vector3(0.1f, 0.05f), Quaternion.identity);
        if (cropPrefab) Instantiate(cropPrefab, p, Quaternion.identity);
        if (seedPrefab) Instantiate(seedPrefab, p + new Vector3(-0.1f, -0.05f), Quaternion.identity);

        _grid.MarkHarvested(_cell);
        Destroy(gameObject);
    }

    public CropData GetData() => new CropData { cell = _cell, currentStage = _stage, timeElapsed = _timer };
}