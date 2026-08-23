using DataType;
using UnityEngine;

namespace Game.Character
{
    public class Actor : MonoBehaviour, IActor
    {
        public ActorId Id { get; set; }
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Run = Animator.StringToHash("run");
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;

        [SerializeField] private float _turnSpeed = 720f;
        [SerializeField] private float _moveAcceleration = 30f;
        [SerializeField] private float _maxMoveSpeed = 5f;

        [SerializeField] private float _maxWalkSpeed = 1.5f;

        private const float WalkAnimationStartSpeed = 0.5f;
        private const float RunAnimationStartSpeed = 1.6f;

        private const float IdleAnimationEndSpeed = 0.1f;

        private Vector3 _aimPosition;
        private bool _moveRequested;
        private bool _walkRequested;
        private Quaternion _controlledRotation;

        private void Awake()
        {
            _controlledRotation = Quaternion.Euler(0f, _rigidbody.rotation.eulerAngles.y, 0f);
        }

        private void FixedUpdate()
        {
            UpdateRotation();
            UpdatePosition();
            UpdateAnimation();
        }
        
        public Vector3 Position => _rigidbody.position;

        public void SetAimDirection(Vector3 aimPosition)
        {
            _aimPosition = aimPosition;
        }

        public void SetMoveFlag(bool moveRequested)
        {
            _moveRequested = moveRequested;
        }

        public void SetWalkFlag(bool walkRequested)
        {
            _walkRequested = walkRequested;
        }

        private void UpdateAnimation()
        {
            Vector3 horizontalVelocity = _rigidbody.linearVelocity;
            horizontalVelocity.y = 0f;

            //Debug.Log($"Horizontal Velocity: {horizontalVelocity}, Magnitude: {horizontalVelocity.magnitude}");
            if (_moveRequested && horizontalVelocity.magnitude > IdleAnimationEndSpeed)
            {
                _animator.SetBool(Walk, true);
                if (horizontalVelocity.magnitude > RunAnimationStartSpeed)
                {
                    _animator.SetBool(Run, true);
                }
                else
                {
                    _animator.SetBool(Run, false);
                }
            }
            else
            {
                _animator.SetBool(Walk, false);
                _animator.SetBool(Run, false);
            }
        }

        private void UpdateRotation()
        {
            Vector3 direction = _aimPosition - _rigidbody.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            _controlledRotation = Quaternion.RotateTowards(_controlledRotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);

            // 벽 충돌로 발생한 물리 회전을 제거한다.
            _rigidbody.angularVelocity = Vector3.zero;

            // 충돌 상태와 관계없이 입력으로 계산한 회전을 적용한다.
            _rigidbody.rotation = _controlledRotation;
        }

        private void UpdatePosition()
        {
            Vector3 velocity = _rigidbody.linearVelocity;

            Vector3 currentPosition = new Vector3(_rigidbody.position.x, 0f, _rigidbody.position.z);

            Vector3 targetPosition = new Vector3(_aimPosition.x, 0f, _aimPosition.z);

            float distance = Vector3.Distance(currentPosition, targetPosition);

            if (!_moveRequested || distance <= 0.2f)
            {
                _rigidbody.linearVelocity = new Vector3(0f, velocity.y, 0f);

                return;
            }

            float maxSpeed = _walkRequested ? _maxWalkSpeed : _maxMoveSpeed;

            // linearVelocity는 월드 좌표이므로 수평 성분만 분리한다.
            Vector3 horizontalWorldVelocity = new Vector3(velocity.x, 0f, velocity.z);

            // AddRelativeForce 계산에 맞게 로컬 좌표로 변환한다.
            Vector3 horizontalLocalVelocity = Quaternion.Inverse(_rigidbody.rotation) * horizontalWorldVelocity;

            horizontalLocalVelocity.y = 0f;

            // ForceMode.Acceleration으로 이번 물리 틱에 발생할 속도 변화다.
            Vector3 requestedDeltaVelocity = Vector3.forward * (_moveAcceleration * Time.fixedDeltaTime);

            // AddRelativeForce가 반영된 후의 예상 속도다.
            Vector3 predictedLocalVelocity = horizontalLocalVelocity + requestedDeltaVelocity;

            // 예상 속도를 최대 속도로 제한한다.
            Vector3 limitedLocalVelocity = Vector3.ClampMagnitude(predictedLocalVelocity, maxSpeed);

            // 실제로 이번 틱에 허용할 속도 변화만 구한다.
            Vector3 allowedDeltaVelocity = limitedLocalVelocity - horizontalLocalVelocity;

            // 속도 변화량을 ForceMode.Acceleration용 가속도로 환산한다.
            Vector3 correctedAcceleration = allowedDeltaVelocity / Time.fixedDeltaTime;

            _rigidbody.AddRelativeForce(correctedAcceleration, ForceMode.Acceleration);
        }
    }
}