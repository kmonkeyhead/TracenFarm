using DataType;
using Game.Content.InGame.Payload;
using R3;

namespace Game.Content.InGame.State
{
    public class FarmWorkState : IFarmWorkState
    {
        public FarmId Id { get; }
        public int WorkingCount { get; set; }
        public ReactiveProperty<float> WorkingProgress { get; }
        public PropWorkingType WorkingType { get; set; }

        public FarmWorkState(FarmId id, int workingCount, float workingProgress)
        {
            Id = id;
            WorkingCount = workingCount;
            WorkingProgress = new ReactiveProperty<float>(workingProgress);
        }
    }
}