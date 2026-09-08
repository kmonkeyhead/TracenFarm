using System.Collections.Generic;
using DataType;
using Game.Content.InGame.Farms;
using Game.UserData.Model;
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

        public void GrowFarm(FarmId farmId, VegetableModel vegetableModel)
        {
            //TODO : 실제로는 farmId와 매칭되는걸 가져와야 한다.
            if (_farmView.Id != farmId.AsPrimitive())
            {
                return;
            }
            
            _farmView.Grow(vegetableModel);
        }

        public void HarvestFarm(FarmId farmId, VegetableModel vegetableModel)
        {
            if (_farmView.Id != farmId.AsPrimitive())
            {
                return;
            }

            _farmView.Harvest(vegetableModel);
        }
    }
}