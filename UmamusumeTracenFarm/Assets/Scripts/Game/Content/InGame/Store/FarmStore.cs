using System.Collections.Generic;
using Game.Content.InGame.Farms;
using Game.Content.InGame.Payload;

namespace Game.Content.InGame.Store
{
    public class FarmStore
    {
        private readonly Dictionary<int, FarmEntry> _payloads = new();
        public IEnumerable<FarmEntry> Entries => _payloads.Values;
        public void Register(IFarmWorkState payload, IFarm farm)
        {
            _payloads.Add(payload.Id, new FarmEntry(payload, farm));
            farm.Initialize(payload);
        }
        
        public FarmEntry Get(int id) => _payloads[id];
    }
}