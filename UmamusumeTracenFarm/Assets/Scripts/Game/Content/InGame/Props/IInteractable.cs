using Game.Content.InGame.Payload;
using UnityEngine;

namespace Game.Content.InGame.Props
{
    public interface IProp
    {
        PropType PropType { get; }
        int Id { get; }
        bool CanInteract(Vector3 position);
        void Initialize(IPropState prop);
    }

    public enum PropType
    {
        Farm,
        Carrot,
    }
}