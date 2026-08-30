using System;
using System.Linq;
using Game.Command;
using Game.Content.InGame.Props;
using Game.UserData.Model;
using Game.UserData.Repository;
using ObservableCollections;
using R3;
using VitalRouter;

namespace Game.Service.Farm
{
    [Routes]
    public partial class FarmService : IDisposable
    {
        public Subject<FarmGrowCompleteMessage> OnFarmGrowComplete { get; } = new Subject<FarmGrowCompleteMessage>();
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
        
        public int GetVegetableCount(int farmId)
        {
            var model = _farmRepository.Models.First(x => x.Id == farmId);
            return model.Value;
        }

        private bool GrowFarm(int farmId)
        {
            var model = _farmRepository.Models.First(x => x.Id == farmId);
            int value = model.Value + 1;
            if (value <= MaxFarmValue)
            {
                model = model with { Value = value };
                _farmRepository.AddOrReplace(model);
                return true;
            }
            return false;
        }

        private void OnFarmRepositoryChanged(in NotifyCollectionChangedEventArgs<FarmModel> e)
        {
            if(e.NewItem != null)
            {
                //언제나 갱신 된다는 가정하에 처리한다
            }
        }
        [Route]
        public void OnPropComplete(PropWorkCompletedCommand command)
        {
            if (command.PropType != PropType.Farm)
            {
                return;
            }

            if (!GrowFarm(command.PropId))
            {
                return;
            }
            
            OnFarmGrowComplete.OnNext(new FarmGrowCompleteMessage(command.PropId));
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
    
    public record FarmGrowCompleteMessage(int FarmId);
}