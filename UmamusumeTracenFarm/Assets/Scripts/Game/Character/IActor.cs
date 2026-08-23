using DataType;
using UnityEngine;

namespace Game.Character
{
    public interface IActor
    {
        ActorId Id { get; set; }
        Vector3 Position { get; }
    }
}