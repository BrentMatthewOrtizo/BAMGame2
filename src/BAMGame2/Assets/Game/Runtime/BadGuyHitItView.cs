using Game399.Shared.Models;
using Game399.Shared.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class BadGuyHitItView : ObserverMonoBehaviour
    {
        private static GameState GameState => ServiceResolver.Resolve<GameState>();
        private static IDamageService DamageService => ServiceResolver.Resolve<IDamageService>();

        [SerializeField] private Button button;

        protected override void Subscribe()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        protected override void Unsubscribe()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var dmg = DamageService.CalculateDamage(GameState.BadGuy, GameState.GoodGuy);
            DamageService.ApplyDamage(GameState.GoodGuy, dmg);
        }
    }
}