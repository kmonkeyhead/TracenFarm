using DataType;
using Game.Command;
using Game.Content.InGame.Payload;
using Game.Content.InGame.Props;
using Game.Service.Farm;
using R3;
using VitalRouter;

namespace Game.Content.InGame.UseCase
{
    [Routes]
    public partial class FarmWorkUseCase
    {
        private readonly FarmService _farmService;
        private readonly FarmStore _farmStore;

        public Subject<FarmGrowCompleteMessage> OnFarmGrowComplete { get; } = new Subject<FarmGrowCompleteMessage>();

        public FarmWorkUseCase(FarmService farmService, FarmStore farmStore, ICommandPublisher commandPublisher)
        {
            _farmService = farmService;
            _farmStore = farmStore;
        }

        public void StartInteracting(int farmId, ActorId actorId)
        {
            var farm = _farmStore.Get(farmId);
            if (farm == null)
            {
                return;
            }

            if(!_farmService.CheckStorageSpace(farmId))
            {
                return;
            }
            
            farm.WorkingCount++;
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

            OnFarmGrowComplete.OnNext(new FarmGrowCompleteMessage(command.PropId));

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