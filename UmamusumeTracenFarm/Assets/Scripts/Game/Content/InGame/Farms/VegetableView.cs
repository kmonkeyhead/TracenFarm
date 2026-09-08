using System;
using UnityEngine;

namespace Game.Content.InGame.Farms
{
    public class VegetableView : MonoBehaviour
    {
        public string UniqueId => _uniqueId;
        private DateTime _startAt;
        private DateTime _endAt;
        private string _uniqueId;
        private bool _isGrowing;

        public void StartGrow(DateTime startAt, DateTime endAt, string uniqueId)
        {
            _isGrowing = true;
            _startAt = startAt;
            _endAt = endAt;
            _uniqueId = uniqueId;
            gameObject.SetActive(true);
        }

        private float GetGrowProgress()
        {
            var totalDuration = (_endAt - _startAt).TotalSeconds;
            var elapsedDuration = (DateTime.Now - _startAt).TotalSeconds;
            return Math.Min((float)(elapsedDuration / totalDuration), 1);
        }

        private void OnDisable()
        {
            _isGrowing = false;
        }

        private void Update()
        {
            if (!_isGrowing)
            {
                return;
            }

            float progress = GetGrowProgress();
            var pos = transform.localPosition;
            pos.y = progress * 0.4f - 0.4f;
            transform.localPosition = pos;
        }
    }
}