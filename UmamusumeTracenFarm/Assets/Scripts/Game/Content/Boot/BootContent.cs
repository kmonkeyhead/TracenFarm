using Game.Service;
using VContainer;
using VContainer.Unity;

namespace Game.Content.Boot
{
    public sealed class BootContent : ContentLifetimeScope
    {
        private sealed class BootEntryPoint : IStartable
        {
            private readonly BootService _bootService;

            public BootEntryPoint(BootService bootService)
            {
                _bootService = bootService;
            }

            public void Start()
            {
                _bootService.Start();
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BootEntryPoint>(Lifetime.Scoped);
        }
    }
}