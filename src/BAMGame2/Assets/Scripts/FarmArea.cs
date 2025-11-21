using System.Collections.Generic;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;
using Game399.Shared.Logic;


[RequireComponent(typeof(Collider2D))]
public class FarmArea : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public float minPlantDistance = 0.5f; // Prevent overlapping crops
    public Transform cropParent;

    private readonly List<Vector2> _plantedPositions = new();
    private Collider2D _collider;
    
    private FarmLogic _logic;

    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if (_collider != null)
            _collider.isTrigger = true;
        _logic = new FarmLogic(minPlantDistance);
    }

    public bool CanInteract() => true;

    public void Interact()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (!_collider.bounds.Contains(playerPos))
        {
            Log.Info("Player not in farm area");
            return;
        }

        // Try planting through FarmLogic instead of manual check
        if (!_logic.TryPlant(playerPos.x, playerPos.y))
        {
            Log.Info("Too close to another crop");
            return;
        }


        // 🌱 Plant new crop
        GameObject prefab = FarmManager.Instance.cropPrefab;
        if (prefab == null)
        {
            Log.Error("No crop prefab set in FarmManager");
            return;
        }

        var cropGO = Instantiate(prefab, playerPos, Quaternion.identity, cropParent);
        var crop = cropGO.GetComponent<CropGrowth>();
        crop.Initialize(FarmManager.Instance, this, playerPos);
        
        Log.Info($"Planted crop at {playerPos}");
    }

    // Called by CropGrowth when harvested
    public void Unregister(Vector2 pos)
    {
        _logic.Remove(pos.x, pos.y);
    }
}