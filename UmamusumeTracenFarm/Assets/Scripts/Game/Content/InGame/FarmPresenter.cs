using System;
using Game.Content.InGame.UseCase;
using Game.Service.Farm;
using R3;

namespace Game.Content.InGame
{
    public class FarmPresenter : IDisposable
    {
        private readonly InGameMap _map;
        private readonly FarmService _farmService;
        private readonly FarmWorkUseCase _farmWorkUseCase;
        private readonly IDisposable _disposable;

        public FarmPresenter(InGameMap map, FarmService farmService, FarmWorkUseCase farmWorkUseCase)
        {
            _map = map;
            _farmService = farmService;
            _farmWorkUseCase = farmWorkUseCase;
            _disposable = _farmWorkUseCase.OnFarmGrowComplete.Subscribe(OnFarmGrowComplete);
        }

        private void OnFarmGrowComplete(FarmGrowCompleteMessage message)
        {
            // Handle the farm grow complete event
            int count = _farmService.GetVegetableCount(message.FarmId);
            _map.GrowFarm(message.FarmId, count);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}