using Game399.Shared.Models;

namespace Game399.Shared.Services
{
    public interface IDamageService
    {
        int CalculateDamage(Character attacker, Character defender);
        void ApplyDamage(Character defender, int damage);
    }
}