using R3;
using UnityEngine;

namespace Game.Content.InGame.Payload
{
    public interface IPropState
    {
        int Id { get; }
        int WorkingCount { get; set; }
        ReactiveProperty<float> WorkingProgress { get; }
        PropWorkingType WorkingType { get; set; }
    }

    public enum PropWorkingType
    {
        None,
        Working,
        Complete,
    }
    
}