using Game399.Shared.Models;
using Game399.Shared.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class CharacterView : ObserverMonoBehaviour
    {
        private static GameState GameState => ServiceResolver.Resolve<GameState>();
        private static IDamageService DamageService => ServiceResolver.Resolve<IDamageService>();

        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;

        protected override void OnTrySubscribe()
        {
            GameState.GoodGuy.Health.ChangeEvent += OnGoodGuyHealthChange;
        
            button.onClick.AddListener(OnButtonClick);
        }

        protected override void OnTryUnsubscribe()
        {
            Debug.Log(nameof(CharacterView) + "." + nameof(OnTryUnsubscribe));
        
            GameState.GoodGuy.Health.ChangeEvent -= OnGoodGuyHealthChange;
        
            button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnGoodGuyHealthChange(int health)
        {
            Debug.Log(nameof(CharacterView) + "." + nameof(OnGoodGuyHealthChange));
        
            label.text = $"Health: {health}";
        }

        private void OnButtonClick()
        {
            Debug.Log(nameof(CharacterView) + "." + nameof(OnButtonClick));
            
            var dmg = DamageService.CalculateDamage(GameState.BadGuy, GameState.GoodGuy);
            DamageService.ApplyDamage(GameState.GoodGuy, dmg);
        }
    }
}
