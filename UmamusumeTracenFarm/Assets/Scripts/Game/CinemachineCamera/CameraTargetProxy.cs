using System;
using UnityEngine;

namespace Game.CinemachineCamera
{
    public class CameraTargetProxy : MonoBehaviour
    {
        private Vector3 _targetPosition;
        private Transform _targetTransform;
        private Vector3 _transitionStart;
        private CameraTargetMode _mode;

        private float _elapsed;
        private float _duration = 1f;

        public void SetTargetPosition(CameraTargetMode mode, Transform targetTransform)
        {
            _elapsed = 0f;
            _mode = mode;
            _targetPosition = targetTransform.position;
            _targetTransform = targetTransform;
            _transitionStart = transform.position;
        }

        private void LateUpdate()
        {
            switch (_mode)
            {
                case CameraTargetMode.Transition:
                {
                    if (_targetTransform == null)
                    {
                        _mode = CameraTargetMode.Hold;
                        return;
                    }

                    _elapsed += Time.deltaTime;

                    float t = Mathf.Clamp01(_elapsed / _duration);
                    t = Mathf.SmoothStep(0f, 1f, t);

                    transform.position = Vector3.Lerp(_transitionStart, _targetTransform.position, t);

                    if (_elapsed >= _duration)
                    {
                        _mode = CameraTargetMode.Follow;
                    }

                    break;
                }
                case CameraTargetMode.Follow:
                {
                    if (_targetTransform != null)
                    {
                        transform.position = _targetTransform.position;
                    }

                    break;
                }
                case CameraTargetMode.Hold:
                {
                    transform.position = _targetPosition;
                    break;
                }
            }
        }
    }
}