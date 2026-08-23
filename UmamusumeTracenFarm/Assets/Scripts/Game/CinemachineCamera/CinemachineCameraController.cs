using Game.Service.Input;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.CinemachineCamera
{
    public enum CameraTargetMode
    {
        Follow,
        Hold,
        Transition
    }

    public class CinemachineCameraController : MonoBehaviour
    {
        [SerializeField] private Unity.Cinemachine.CinemachineCamera _cinemachineCamera;
        [SerializeField] private CinemachineBrain _brain;
        [SerializeField] private InputDebugger _inputDebugger;
        [SerializeField] private Transform _mainTarget;
        [SerializeField] private CameraTargetProxy _proxy;
        [SerializeField] private Transform[] _secondaryTargets;

        private int _secondaryTargetIndex;

        private void Awake()
        {
            if (_inputDebugger == null)
            {
                _inputDebugger = GetComponent<InputDebugger>();
            }

            if (_inputDebugger == null)
            {
                Debug.LogError("InputDebugger가 연결되지 않았습니다.", this);
                enabled = false;
                return;
            }

            if (_cinemachineCamera == null)
            {
                Debug.LogError("CinemachineCamera가 연결되지 않았습니다.", this);
                enabled = false;
                return;
            }

            _inputDebugger.NumberPressed += OnDebugNumberPressed;
            _proxy.SetTargetPosition(CameraTargetMode.Follow, _mainTarget); // 원래 여기서 해서는 안된다
            
        }

        private void OnDestroy()
        {
            if (_inputDebugger != null)
            {
                _inputDebugger.NumberPressed -= OnDebugNumberPressed;
            }
        }

        private void OnDebugNumberPressed(int number)
        {
            switch (number)
            {
                case 1:
                {
                    _cinemachineCamera.PreviousStateIsValid = false;
                    _proxy.SetTargetPosition(CameraTargetMode.Follow, _mainTarget);
                    _secondaryTargetIndex = 0;
                    break;
                }
                case 2:
                {
                    ChangeTarget(_secondaryTargets[_secondaryTargetIndex]);
                    _secondaryTargetIndex++;
                    if (_secondaryTargetIndex >= _secondaryTargets.Length)
                    {
                        _secondaryTargetIndex = 0;
                    }

                    break;
                }
                case 3:
                {
                    ChangeTargetSmooth(_secondaryTargets[_secondaryTargetIndex]);
                    _secondaryTargetIndex++;
                    if (_secondaryTargetIndex >= _secondaryTargets.Length)
                    {
                        _secondaryTargetIndex = 0;
                    }

                    break;
                }
            }

            Debug.Log($"Cinemachine debug command: {number}");
        }

        private void ChangeTarget(Transform target)
        {
            _proxy.SetTargetPosition(CameraTargetMode.Hold, target);
            _cinemachineCamera.PreviousStateIsValid = false;
        }

        private void ChangeTargetSmooth(Transform target)
        {
            _proxy.SetTargetPosition(CameraTargetMode.Transition, target);
        }
    }
}