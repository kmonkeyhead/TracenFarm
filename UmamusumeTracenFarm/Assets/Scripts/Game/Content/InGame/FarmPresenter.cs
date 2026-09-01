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
            var builder = Disposable.CreateBuilder();
            builder.Add(_farmWorkUseCase.OnFarmGrowComplete.Subscribe(OnFarmGrowComplete));
            builder.Add(_farmWorkUseCase.OnFarmHarvest.Subscribe(OnFarmHarvest));
            _disposable = builder.Build();
        }

        private void OnFarmGrowComplete(FarmGrowCompleteMessage message)
        {
            // Handle the farm grow complete event
            int count = _farmService.GetVegetableCount(message.FarmId);
            _map.GrowFarm(message.FarmId, count);
        }

        private void OnFarmHarvest(FarmHarvestMessage message)
        {
            int count = _farmService.GetVegetableCount(message.FarmId);
            _map.HarvestFarm(message.FarmId, count);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}