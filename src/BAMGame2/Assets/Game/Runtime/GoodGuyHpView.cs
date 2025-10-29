using Game399.Shared.Models;
using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class GoodGuyHpView : ObserverMonoBehaviour
    {
        private static GameState GameState => ServiceResolver.Resolve<GameState>();

        [SerializeField] private TextMeshProUGUI label;

        protected override void Subscribe()
        {
            GameState.GoodGuy.Health.ChangeEvent += OnGoodGuyHealthChange;
        }

        protected override void Unsubscribe()
        {
            GameState.GoodGuy.Health.ChangeEvent -= OnGoodGuyHealthChange;
        }

        private void OnGoodGuyHealthChange(int health)
        {
            label.text = $"Health: {health}";
        }
    }
}