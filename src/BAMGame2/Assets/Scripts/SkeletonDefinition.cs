using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "BAM/Enemy Definition")]
public class SkeletonDefinition : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;

    [Header("Stats")]
    public int hp;
    public int damage;

    [Header("World Prefab / Battle Prefab")]
    public GameObject battlePrefab;  
}