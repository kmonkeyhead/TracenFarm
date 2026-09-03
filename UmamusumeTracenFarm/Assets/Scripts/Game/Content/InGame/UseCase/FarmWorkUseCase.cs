using DataType;
using Game.Command;
using Game.Content.InGame.Payload;
using Game.Content.InGame.Props;
using Game.Service.Farm;
using R3;
using VitalRouter;

namespace Game.Content.InGame.UseCase
{
    //로직과 뷰 상태까지 같이 처리한다.
    [Routes]
    public partial class FarmWorkUseCase
    {
        private readonly FarmService _farmService;
        private readonly FarmStore _farmStore;
        private readonly InGameMap _inGameMap;

        public Subject<FarmGrowCompleteMessage> OnFarmGrowComplete { get; } = new Subject<FarmGrowCompleteMessage>();
        public Subject<FarmHarvestMessage> OnFarmHarvest { get; } = new Subject<FarmHarvestMessage>();

        public FarmWorkUseCase(FarmService farmService, FarmStore farmStore, InGameMap inGameMap, ICommandPublisher commandPublisher)
        {
            _farmService = farmService;
            _farmStore = farmStore;
            _inGameMap = inGameMap;
        }

        public void StartInteracting(int farmId, ActorId actorId)
        {
            var farm = _farmStore.Get(farmId);
            if (farm == null)
            {
                return;
            }

            if (!_farmService.CheckStorageSpace(farmId))
            {
                return;
            }

            farm.WorkingCount++;
        }

        public void Harvest(int farmId, ActorId actorId)
        {
            //현재 라우터 필요 없다
            if (_farmService.HarvestVegetable(farmId))
            {
                _inGameMap.HarvestFarm(farmId, _farmService.GetVegetableCount(farmId)); 
                //OnFarmHarvest.OnNext(new FarmHarvestMessage(farmId));
            }
        }

        public void StopInteracting(int farmId, ActorId actorId)
        {
            //TODO : WorkingCount가 아닌 ActorId를 가지고 있어야 한다
            var farm = _farmStore.Get(farmId);
            if (farm == null)
            {
                return;
            }

            if (farm.WorkingCount == 0)
            {
                return;
            }

            farm.WorkingCount--;
        }

        [Route]
        public void OnPropComplete(PropWorkCompletedCommand command)
        {
            if (command.PropType != PropType.Farm)
            {
                return;
            }

            var entry = _farmStore.Get(command.PropId);
            entry.WorkingProgress = 0f;
            if (!_farmService.GrowFarm(command.PropId))
            {
                entry.WorkingType = PropWorkingType.None;
                entry.WorkingCount = 0;
                return;
            }

            _inGameMap.GrowFarm(command.PropId, _farmService.GetVegetableCount(command.PropId));
            //현재 라우터가 필요 없다.
            //OnFarmGrowComplete.OnNext(new FarmGrowCompleteMessage(command.PropId)); 

            bool available = _farmService.CheckStorageSpace(command.PropId);
            if (available)
            {
                entry.WorkingType = PropWorkingType.Working;
            }
            else
            {
                entry.WorkingType = PropWorkingType.None;
                entry.WorkingCount = 0;
            }
        }
    }
}