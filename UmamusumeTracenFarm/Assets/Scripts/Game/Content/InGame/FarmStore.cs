using System.Collections.Generic;
using Game.Content.InGame.Payload;
using Game.Content.InGame.Props;

namespace Game.Content.InGame
{
    public class FarmStore
    {
        private readonly Dictionary<int, PropEntry> _payloads = new();
        public IEnumerable<PropEntry> Entries => _payloads.Values;
        public void Register(IPropState payload, IProp prop)
        {
            _payloads.Add(payload.Id, new PropEntry(payload, prop));
            prop.Initialize(payload);
        }
        
        public PropEntry Get(int id) => _payloads[id];
    }
}