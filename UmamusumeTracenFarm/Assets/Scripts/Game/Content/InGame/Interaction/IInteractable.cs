using Game.Content.InGame.Payload;
using UnityEngine;

namespace Game.Content.InGame.Interaction
{
    public interface IInteractable
    {
        int Id { get; }
        bool CanInteract(Vector3 position);
    }
}