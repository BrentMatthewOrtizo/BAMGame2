using UnityEngine;

[CreateAssetMenu(menuName="Farm/Animal")]
public class AnimalDefinition : ScriptableObject
{
    public string animalName;
    public Sprite icon;
    public int baseHP;
    public int baseDamage;
    public int cost;
}