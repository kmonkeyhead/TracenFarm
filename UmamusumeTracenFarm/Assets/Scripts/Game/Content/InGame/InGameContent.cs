using Game.Character;
using Game.Content.InGame.UseCase;
using Game.Service.Farm;
using VContainer;
using VContainer.Unity;
using Game.Service.Gesture;
using UnityEngine;
using VitalRouter.VContainer;

namespace Game.Content.InGame
{
    public class InGameContent : ContentLifetimeScope
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private PlayerController _characterController;
        [SerializeField] private Actor _userActor;
        [SerializeField] private InGameMap _inGameMap;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.Register<PropsProgressService>(Lifetime.Singleton);
            builder.Register<FarmStore>(Lifetime.Singleton);
            builder.Register<ActorStore>(Lifetime.Singleton);
            builder.Register<ClickGesture>(Lifetime.Singleton);
            builder.Register<FarmWorkUseCase>(Lifetime.Singleton);
            builder.Register<InteractionUseCase>(Lifetime.Singleton);
            builder.RegisterEntryPoint<InGameFlow>(Lifetime.Singleton).WithParameter(_userActor);

            builder.RegisterComponent(_camera);
            builder.RegisterComponent(_characterController);
            builder.RegisterComponent(_inGameMap);
            
            
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<FarmWorkUseCase>();
            });
        }
    }
}