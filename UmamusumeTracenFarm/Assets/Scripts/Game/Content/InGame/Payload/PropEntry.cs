using Game.Content.InGame.Props;

namespace Game.Content.InGame.Payload
{
    public record PropEntry(IPropState Payload, IProp Prop)
    {
        public int Id => Payload.Id;
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