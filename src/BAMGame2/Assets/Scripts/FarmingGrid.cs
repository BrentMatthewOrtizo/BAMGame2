using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingGrid : MonoBehaviour
{
    [Header("Tilemap & Prefabs")]
    public Tilemap farmlandTilemap; // Make sure to assign this in Inspector!

    [Header("Farmable Tiles")]
    public List<TileBase> farmlandTiles = new List<TileBase>(); // All valid farmland tiles

    [Header("Crop Settings")]
    public GameObject cropPrefab;
    public Transform cropParent;
    public DayNightCycleUI cycle;

    private readonly Dictionary<Vector3Int, CropGrowth> _crops = new();

    private void Start() => LoadCrops();
    private void OnDisable() => SaveCrops();

    public void PlantAt(Vector3 worldPos)
    {
        
        Vector3 adjustedPos = worldPos + new Vector3(0f, -0.5f, 0f); // Shift down half a unit
        Vector3Int cell = farmlandTilemap.WorldToCell(adjustedPos);
        Vector3 cellCenter = farmlandTilemap.GetCellCenterWorld(cell);
        TileBase tile = farmlandTilemap.GetTile(cell);

        Debug.Log($"📦 Player worldPos (adjusted): {adjustedPos:F2} | Cell: {cell} | Cell Center: {cellCenter:F2}");

        if (!cycle || !cycle.IsDay)
        {
            Debug.Log("❌ Not day time — cannot plant!");
            return;
        }

        Debug.DrawRay(cellCenter, Vector3.up * 0.5f, Color.green, 1f);

        if (tile == null)
        {
            Debug.Log("❌ No tile found under player — likely between cells.");
            return;
        }

        Debug.Log($"✅ Player standing on tile: {tile.name}");

        if (!farmlandTiles.Contains(tile))
        {
            Debug.Log($"❌ Tile '{tile.name}' is NOT farmable!");
            return;
        }

        if (_crops.ContainsKey(cell))
        {
            Debug.Log("❌ There’s already a crop here!");
            return;
        }

        Vector3 spawnPos = farmlandTilemap.GetCellCenterWorld(cell);
        Debug.Log($"🌿 Planting crop at {spawnPos}");
        var go = Instantiate(cropPrefab, spawnPos, Quaternion.identity, cropParent);
        var g = go.GetComponent<CropGrowth>();
        g.Initialize(this, cell);
        _crops[cell] = g;
    }


    public void MarkHarvested(Vector3Int cell) => _crops.Remove(cell);

    // Save all crops to GameStateManager
    private void SaveCrops()
    {
        if (GameStateManager.Instance == null) return;
        var list = new List<CropData>();
        foreach (var kv in _crops)
            if (kv.Value != null) list.Add(kv.Value.GetData());
        GameStateManager.Instance.SaveCrops(list);
    }

    // Load crops back into the scene
    private void LoadCrops()
    {
        if (GameStateManager.Instance == null) return;

        foreach (var data in GameStateManager.Instance.crops)
        {
            Vector3 pos = farmlandTilemap.GetCellCenterWorld(data.cell);
            var go = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);
            var g = go.GetComponent<CropGrowth>();
            g.LoadFromData(this, data.cell, data.currentStage, data.timeElapsed);
            _crops[data.cell] = g;
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("🔍 List all tiles in Tilemap")]
    private void ListAllTilesInMap()
    {
        if (farmlandTilemap == null)
        {
            Debug.Log("❌ No farmlandTilemap assigned!");
            return;
        }

        var bounds = farmlandTilemap.cellBounds;
        HashSet<string> tileNames = new();

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase t = farmlandTilemap.GetTile(pos);
            if (t != null) tileNames.Add(t.name);
        }

        Debug.Log($"🟢 Found {tileNames.Count} unique tiles:");
        foreach (string name in tileNames)
            Debug.Log($"  • {name}");
    }
#endif
    
    private void OnDrawGizmosSelected()
    {
        if (farmlandTilemap == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.25f);
        var bounds = farmlandTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (farmlandTilemap.HasTile(pos))
            {
                Vector3 world = farmlandTilemap.GetCellCenterWorld(pos);
                Gizmos.DrawCube(world, farmlandTilemap.cellSize);
            }
        }
    }

}