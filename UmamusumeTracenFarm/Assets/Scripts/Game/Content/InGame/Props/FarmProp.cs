using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Content.InGame.Payload;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Content.InGame.Props
{
    /// <summary>
    /// 밭의 심는 자리마다 당근을 미리 만들어 두고 인덱스로 켜고 끈다.
    /// 수확과 재배치 때마다 Instantiate/Destroy하지 않고 활성 상태만 바꾼다.
    /// </summary>
    public class FarmProp : MonoBehaviour, IProp, IDisposable
    {
        [SerializeField] private Image _progress;
        [SerializeField] private Collider _interactionArea;
        [SerializeField] private float _interactionRange = 1.5f;
        [SerializeField] private GameObject _carrotPrefab;
        [SerializeField] private Transform _slotRoot;

        // 한 줄에 놓이는 칸 수. 줄/칸 번호를 1차 리스트 인덱스로 바꿀 때 쓴다.
        [SerializeField] private int _columnCount = 5;

        // 당근을 흙에 묻을 깊이. Carrot 프리팹의 SoilLine 높이를 넣으면 잎만 보인다.
        [SerializeField] private float _plantDepth;

        [SerializeField] private bool _activeOnStart = true;
        public PropType PropType => PropType.Farm;
        private readonly List<GameObject> _carrots = new List<GameObject>();
        public int Id => _propState.Id;
        private IPropState _propState;
        private bool _disposed;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private IDisposable _disposable;
        

        private void Awake()
        {
            _progress.fillAmount = 0;
            if (_carrotPrefab == null)
            {
                Debug.LogError("당근 프리팹이 연결되지 않았습니다.", this);
                enabled = false;
                return;
            }

            if (_slotRoot == null)
            {
                Debug.LogError("Slot 루트가 연결되지 않았습니다.", this);
                enabled = false;
                return;
            }

            CreateCarrots();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _cts?.Dispose();
                _disposable?.Dispose();
            }
        }


        public void Initialize(IPropState propState)
        {
            _propState = propState;
            _disposable = propState.WorkingProgress.Subscribe(FillProgress);
        }

        /// <summary>
        /// Slots의 자식 순서가 그대로 리스트 인덱스가 된다.
        /// 첫 줄 왼쪽에서 오른쪽으로 채운 뒤 다음 줄로 넘어간다.
        /// </summary>
        private void CreateCarrots()
        {
            for (int i = 0; i < _slotRoot.childCount; i++)
            {
                Transform slot = _slotRoot.GetChild(i);

                GameObject carrot = Instantiate(_carrotPrefab, slot);
                carrot.name = $"Carrot_{i:00}";
                carrot.transform.localPosition = new Vector3(0f, -_plantDepth, 0f);
                carrot.transform.localRotation = Quaternion.identity;
                carrot.SetActive(false);

                _carrots.Add(carrot);
            }
        }

        public void SetCarrotActive(int index, bool active)
        {
            if (!IsValidIndex(index))
            {
                return;
            }

            _carrots[index].SetActive(active);
        }

        public bool IsCarrotActive(int index)
        {
            return IsValidIndex(index) && _carrots[index].activeSelf;
        }
        private bool IsValidIndex(int index)
        {
            if (index >= 0 && index < _carrots.Count)
            {
                return true;
            }

            Debug.LogWarning($"당근 인덱스 {index}가 범위를 벗어났습니다. (0 ~ {_carrots.Count - 1})", this);
            return false;
        }

        public bool CanInteract(Vector3 actorPosition)
        {
            Vector3 closestPoint = _interactionArea.ClosestPoint(actorPosition);
            Vector3 offset = closestPoint - actorPosition;

            // 농장 게임에서 높이 차이를 무시한다면
            offset.y = 0f;

            return offset.sqrMagnitude <= _interactionRange * _interactionRange;
        }
        private int _testIndex = 0;

        private void FillProgress(float progress)
        {
            _progress.fillAmount = progress;
        }

        public void Grow(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _carrots[i].SetActive(true);
            }
        }
    }
}