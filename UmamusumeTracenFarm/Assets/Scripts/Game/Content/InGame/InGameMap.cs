using System.Collections.Generic;
using Game.Content.InGame.Farms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Content.InGame
{
    public class InGameMap : MonoBehaviour
    {
        [SerializeField] private List<Transform> _farmLocations;
        [FormerlySerializedAs("_farmProp")]
        [SerializeField] private FarmView _farmView;

        public FarmView FarmView => _farmView;

        public void GrowFarm(int farmId, int count)
        {
            //TODO : 실제로는 farmId와 매칭되는걸 가져와야 한다.
            if (_farmView.Id != farmId)
            {
                return;
            }

            _farmView.Grow(count);
        }

        public void HarvestFarm(int farmId, int count)
        {
            if (_farmView.Id != farmId)
            {
                return;
            }

            _farmView.Harvest(count);
        }
    }
}