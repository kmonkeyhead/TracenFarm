using Game.Character;
using Game.Content.Boot;
using Game.Service;
using Game.Service.Farm;
using Game.Service.Input;
using Game.UserData.Repository;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public sealed class RootLifeTimeScope : LifetimeScope
    {
        [SerializeField] private BootContent _bootContent;
  
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterRepository(builder);
            RegisterService(builder);

            builder.Register<InputService>(Lifetime.Singleton).AsSelf();
            builder.RegisterBuildCallback(container =>
            {
                container.Resolve<InputService>();
            });
        }
        
        private void RegisterService(IContainerBuilder builder)
        {
            builder.Register<BootService>(Lifetime.Singleton);
            builder.Register<FarmService>(Lifetime.Singleton);
        }

        private void RegisterRepository(IContainerBuilder builder)
        {
            builder.Register<FarmRepository>(Lifetime.Singleton);
        }
        
        protected override void Awake()
        {
            base.Awake();
            _bootContent.Build();
        }
    }
}
