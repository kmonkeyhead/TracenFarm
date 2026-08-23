using Game.Service.Farm;
using VContainer.Unity;

namespace Game.Service
{
    public class BootService
    {
        private readonly FarmService _farmService;

        public BootService(FarmService farmService)
        {
            _farmService = farmService;
        }

        public void Start()
        {
            _farmService.Initialize();
        }
    }
}