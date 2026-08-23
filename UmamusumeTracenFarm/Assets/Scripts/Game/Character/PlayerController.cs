using Game.Input;
using Game.Service.Input;
using R3;
using UnityEngine;
using VContainer;

namespace Game.Character
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Actor _actor;

        [Inject]
        public void Construct(InputService inputService)
        {
            inputService.PointerPosition.Subscribe(OnPointerPositionChanged).AddTo(this);
            inputService.LeftClick.Subscribe(OnLeftClick).AddTo(this);
            inputService.Walk.Subscribe(OnWalk).AddTo(this);
        }

        private void OnLeftClick(bool isPressed)
        {
            _actor.SetMoveFlag(isPressed);
        }

        private void OnWalk(bool isPressed)
        {
            _actor.SetWalkFlag(isPressed);
        }

        private void OnPointerPositionChanged(Vector2 position)
        {
            var point = ScreenToWorldPoint(position);
            _actor.SetAimDirection(point);
        }

        private Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            Ray ray = _camera.ScreenPointToRay(screenPoint);
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            groundPlane.Raycast(ray, out float distance);

            return ray.GetPoint(distance);
        }
    }
}