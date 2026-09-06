using System;
using System.Linq;
using Game.UserData.Model;
using Game.UserData.Repository;
using ObservableCollections;
using R3;

namespace Game.Service.Farm
{
    public class FarmService : IDisposable
    {
        private const int FarmCount = 1;
        private const int MaxFarmValue = 15;
        private readonly FarmRepository _farmRepository;
        private readonly IDisposable _disposable;
        private readonly VegetableRepository _vegetableRepository;

        public FarmService(FarmRepository farmRepository, VegetableRepository vegetableRepository)
        {
            _farmRepository = farmRepository;
            _vegetableRepository = vegetableRepository;

            var builder = Disposable.CreateBuilder();

            builder.Add(_farmRepository.Subscribe(OnFarmRepositoryChanged));
            _disposable = builder.Build();
        }

        public void Initialize()
        {
            for (int i = 0; i < FarmCount; i++)
            {
                var model = new FarmModel(i + 1);

                _farmRepository.AddOrReplace(model);
            }
        }

        public int GetVegetableCount(int farmId)
        {
            return _vegetableRepository.Count(x => x.FarmId == farmId);
        }

        public bool CheckStorageSpace(int farmId)
        {
            var count = _vegetableRepository.Count(x => x.FarmId == farmId);
            return MaxFarmValue - count > 0;
        }

        public bool HarvestVegetable(int farmId)
        {
            var now = DateTime.Now;
            var vegetable = _vegetableRepository.FirstOrDefault(x => x.FarmId == farmId && x.EndAt <= now);
            if (vegetable == null)
            {
                return false;
            }

            _vegetableRepository.Remove(vegetable);
            return true;
        }

        public bool GrowFarm(int farmId)
        {
            if (!CheckStorageSpace(farmId))
            {
                return false;
            }

            const float Duration = 2f;
            var vegetable = new VegetableModel(Guid.NewGuid().ToString(), farmId, DateTime.Now, DateTime.Now.AddSeconds(Duration));
            _vegetableRepository.AddOrReplace(vegetable);

            return true;
        }

        private void OnFarmRepositoryChanged(in NotifyCollectionChangedEventArgs<FarmModel> e)
        {
            if (e.NewItem != null)
            {
                //언제나 갱신 된다는 가정하에 처리한다
            }
        }


        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }

    public record FarmGrowCompleteMessage(int FarmId);

    public record FarmHarvestMessage(int FarmId);
}