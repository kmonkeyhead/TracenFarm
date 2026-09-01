using System.Collections.Generic;
using Game.Content.InGame.Props;
using UnityEngine;

namespace Game.Content.InGame
{
    public class InGameMap : MonoBehaviour
    {
        [SerializeField] private List<Transform> _farmLocations;
        [SerializeField] private FarmProp _farmProp;

        public FarmProp FarmProp => _farmProp;

        public void GrowFarm(int farmId, int count)
        {
            //TODO : 실제로는 farmId와 매칭되는걸 가져와야 한다.
            if (_farmProp.Id != farmId)
            {
                return;
            }

            _farmProp.Grow(count);
        }

        public void HarvestFarm(int farmId, int count)
        {
            if (_farmProp.Id != farmId)
            {
                return;
            }

            _farmProp.Harvest(count);
        }
    }
}