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

        public FarmService(FarmRepository farmRepository)
        {
            _farmRepository = farmRepository;

            var builder = Disposable.CreateBuilder();

            builder.Add(_farmRepository.Subscribe(OnFarmRepositoryChanged));
            _disposable = builder.Build();
        }

        public void Initialize()
        {
            for(int i = 0 ; i < FarmCount ; i++)
            {
                var model = new FarmModel(i + 1, 0);
                
                _farmRepository.AddOrReplace(model);
            }
        }

        public void GrowFarm(int farmId)
        {
            var model = _farmRepository.Models.First(x => x.Id == farmId);
            int value = model.Value + 1;
            if (value <= MaxFarmValue)
            {
                model = model with { Value = value };
                _farmRepository.AddOrReplace(model);
            }
        }

        private void OnFarmRepositoryChanged(in NotifyCollectionChangedEventArgs<FarmModel> e)
        {
            if(e.NewItem != null)
            {
                //언제나 갱신 된다는 가정하에 처리한다
            }
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}