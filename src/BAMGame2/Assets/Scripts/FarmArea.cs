using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FarmArea : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public float minPlantDistance = 0.5f; // Prevent overlapping crops
    public Transform cropParent;

    private readonly List<Vector2> _plantedPositions = new();
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if (_collider != null)
            _collider.isTrigger = true;
    }

    public bool CanInteract() => true;

    public void Interact()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (!_collider.bounds.Contains(playerPos))
        {
            Debug.Log("❌ Player not in farm area");
            return;
        }

        foreach (var pos in _plantedPositions)
        {
            if (Vector2.Distance(pos, playerPos) < minPlantDistance)
            {
                Debug.Log("❌ Too close to another crop");
                return;
            }
        }

        // 🌱 Plant new crop
        GameObject prefab = FarmManager.Instance.cropPrefab;
        if (prefab == null)
        {
            Debug.LogError("❌ No crop prefab set in FarmManager!");
            return;
        }

        var cropGO = Instantiate(prefab, playerPos, Quaternion.identity, cropParent);
        var crop = cropGO.GetComponent<CropGrowth>();
        crop.Initialize(FarmManager.Instance, this, playerPos);

        _plantedPositions.Add(playerPos);
        Debug.Log($"✅ Planted crop at {playerPos}");
    }

    // Called by CropGrowth when harvested
    public void Unregister(Vector2 pos)
    {
        _plantedPositions.RemoveAll(p => Vector2.Distance(p, pos) < 0.1f);
    }
}