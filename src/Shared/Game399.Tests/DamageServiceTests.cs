using Game399.Shared;
using Game399.Shared.Models;
using Game399.Shared.Services.Implementation;

namespace Game399.Tests;

public class DamageServiceTests
{
    private static Character CreateCharacter(int health, int damage, int armor)
    {
        var c = new Character();
        c.Health.Value = health;
        c.Damage.Value = damage;
        c.Armor.Value = armor;
        return c;
    }

    [Test]
    public void CalculateDamage_Returns_AttackerDamage_Minus_DefenderArmor()
    {
        var svc = new DamageService(EmptyGameLog.Instance);
        var attacker = CreateCharacter(health: 100, damage: 12, armor: 1);
        var defender = CreateCharacter(health: 100, damage: 5, armor: 3);

        var dmg = svc.CalculateDamage(attacker, defender);

        Assert.That(dmg, Is.EqualTo(12 - 3));
    }

    [Test]
    public void CalculateDamage_Can_Be_Negative_When_Armor_Exceeds_Damage()
    {
        var svc = new DamageService(EmptyGameLog.Instance);
        var attacker = CreateCharacter(health: 100, damage: 5, armor: 0);
        var defender = CreateCharacter(health: 100, damage: 0, armor: 8);

        var dmg = svc.CalculateDamage(attacker, defender);

        Assert.That(dmg, Is.EqualTo(-3));
    }

    [Test]
    public void ApplyDamage_Reduces_Health_By_Specified_Amount()
    {
        var svc = new DamageService(EmptyGameLog.Instance);
        var defender = CreateCharacter(health: 20, damage: 0, armor: 0);

        svc.ApplyDamage(defender, 7);

        Assert.That(defender.Health.Value, Is.EqualTo(13));
    }

    [Test]
    public void ApplyDamage_Allows_Health_To_Go_Negative_When_Damage_Exceeds_Health()
    {
        var svc = new DamageService(EmptyGameLog.Instance);
        var defender = CreateCharacter(health: 5, damage: 0, armor: 0);

        svc.ApplyDamage(defender, 10);

        Assert.That(defender.Health.Value, Is.EqualTo(-5));
    }
}
