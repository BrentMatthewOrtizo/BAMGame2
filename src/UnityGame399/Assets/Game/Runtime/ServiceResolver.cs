using System;
using Game399.Shared;
using Game399.Shared.DependencyInjection;
using Game399.Shared.DependencyInjection.Implementation;
using Game399.Shared.Diagnostics;
using Game399.Shared.Models;
using Game399.Shared.Services;
using Game399.Shared.Services.Implementation;

namespace Game.Runtime
{
    public static class ServiceResolver
    {
        public static T Resolve<T>() => Container.Value.Resolve<T>();

        private static readonly Lazy<IMiniContainer> Container = new (() =>
        {
            var container = new MiniContainer();

            var logger = new UnityGameLogger();
            container.RegisterSingletonInstance<IGameLog>(logger);

            var gameState = new GameState();
            gameState.GoodGuy.Name = "Good Sandy";
            gameState.GoodGuy.Health.Value = 10;
            gameState.GoodGuy.Damage.Value = 1;
            gameState.BadGuy.Name = "Bad Sandy";
            gameState.BadGuy.Health.Value = 10;
            gameState.BadGuy.Damage.Value = 1;
            container.RegisterSingletonInstance(gameState);

            var damageService = new DamageService(logger);
            container.RegisterSingletonInstance<IDamageService>(damageService);
            
            return container;
        });
    }
}