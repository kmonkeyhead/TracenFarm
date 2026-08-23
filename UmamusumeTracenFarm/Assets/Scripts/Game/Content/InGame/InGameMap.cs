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
    }
}