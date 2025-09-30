using Game399.Shared.Diagnostics;
using Game399.Shared.Models;

namespace Game399.Shared.Services.Implementation
{
    public class DamageService : IDamageService
    {
        private readonly IGameLog _gameLog;

        public DamageService(IGameLog gameLog)
        {
            _gameLog = gameLog;
        }
        
        public int CalculateDamage(Character attacker, Character defender)
        {
            var damage = attacker.Damage.Value - defender.Armor.Value;
            _gameLog.Info($"{attacker.Name} attacked {defender.Name} for {damage} damage");
            return damage;
        }

        public void ApplyDamage(Character defender, int damage)
        {
            _gameLog.Info($"{defender.Name} takes {damage} damage");
            defender.Health.Value -= damage;
        }
    }
}