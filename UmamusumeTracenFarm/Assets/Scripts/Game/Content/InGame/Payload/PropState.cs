using R3;
using UnityEngine;

namespace Game.Content.InGame.Payload
{
    public class PropState : IPropState
    {
        public int Id { get; }
        public int WorkingCount { get; set; }
        public ReactiveProperty<float> WorkingProgress { get; }
        public PropWorkingType WorkingType { get; set; }

        public PropState(int id, int workingCount, float workingProgress)
        {
            Id = id;
            WorkingCount = workingCount;
            WorkingProgress = new ReactiveProperty<float>(workingProgress);
        }
    }
}