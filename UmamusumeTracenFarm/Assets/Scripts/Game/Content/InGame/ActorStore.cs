using System;
using System.Collections.Generic;
using DataType;
using Game.Character;

namespace Game.Content.InGame
{
    public class ActorStore : IActorStore
    {
        private readonly Dictionary<ActorId, IActor> _actors = new();

        public ActorId MyActorId { get; private set; }

        public IActor MyActor => _actors.GetValueOrDefault(MyActorId);

        public IReadOnlyCollection<IActor> All =>
            _actors.Values;

        public void Register(IActor actor)
        {
            _actors.Add(actor.Id, actor);
        }

        public void Unregister(ActorId id)
        {
            _actors.Remove(id);
        }

        public void SetMyActor(ActorId id)
        {
            if (!_actors.ContainsKey(id))
            {
                throw new InvalidOperationException($"등록되지 않은 Actor입니다: {id}");
            }

            MyActorId = id;
        }

        public bool TryGet(ActorId id, out IActor actor)
        {
            return _actors.TryGetValue(id, out actor);
        }
    }
    
    public interface IActorStore
    {
        ActorId MyActorId { get; }
        IActor MyActor { get; }

        IReadOnlyCollection<IActor> All { get; }

        bool TryGet(ActorId id, out IActor actor);
    }
}