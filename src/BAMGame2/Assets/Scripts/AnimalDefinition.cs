using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimal", menuName = "BAM/Animal Definition")]
public class AnimalDefinition : ScriptableObject
{
    [Header("Identity")]
    public string animalName;

    [Header("Stats")]
    public int hp;
    public int damage;

    [Header("Shop")]
    public int cost;
    public Sprite portraitSprite;

    [Header("World Prefab")]
    public GameObject worldPrefab;

    [Header("Battle Sprite")]
    public Sprite battleSprite;
}