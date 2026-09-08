using DataType;
using Game.Content.InGame.Payload;

namespace Game.Content.InGame.Farms
{
    public record FarmEntry(IFarmWorkState Payload, IFarm Farm)
    {
        public FarmId Id => Payload.Id;

        public int WorkingCount
        {
            get => Payload.WorkingCount;
            set => Payload.WorkingCount = value;
        }

        public float WorkingProgress
        {
            get => Payload.WorkingProgress.Value;
            set => Payload.WorkingProgress.Value = value;
        }

        public PropWorkingType WorkingType
        {
            get => Payload.WorkingType;
            set => Payload.WorkingType = value;
        }
    }
}