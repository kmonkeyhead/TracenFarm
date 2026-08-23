using Game.Content.InGame.Payload;
using Game.Content.InGame.Props;
using Game.Service.Gesture;
using UnityEngine;

namespace Game.Content.InGame.UseCase
{
    public class InteractionUseCase
    {
        private readonly Camera _camera;
        private readonly ActorStore _actorStore;
        private bool _interact;
        private IProp _lastProp;
        private readonly FarmStore _farmStore;

        public InteractionUseCase(Camera camera, ActorStore actorStore, FarmStore farmStore)
        {
            _camera = camera;
            _actorStore = actorStore;
            _farmStore = farmStore;
        }

        public void UpdateGesture(HoldGesturePayload holdGesturePayload)
        {
            if (holdGesturePayload.GestureType is HoldGestureType.Start && !_interact)
            {
                var targetObject = GetGameObject(holdGesturePayload.Position);

                if (targetObject == null)
                {
                    _interact = false;
                    return;
                }


                _interact = true;

                if (targetObject.TryGetComponent<IProp>(out var prop))
                {
                    if (prop.CanInteract(_actorStore.MyActor.Position))
                    {
                        CheckAndInteract(prop);
                    }
                }
            }
            else if (holdGesturePayload.GestureType == HoldGestureType.End)
            {
                if (!_interact)
                {
                    return;
                }

                _interact = false;
                StopInteract(_lastProp);
                _lastProp = null;
            }
        }

        private void CheckAndInteract(IProp prop)
        {
            const int availableCount = 1;

            //TODO : 타입에 따른 Store를 따로 해야한다 ->Prop에 타입이 있어야 한다
            var entry = _farmStore.Get(prop.Id);
            _lastProp = prop;

            if (entry.WorkingCount <= availableCount)
            {
                entry.WorkingCount++;
            }
        }

        private void StopInteract(IProp prop)
        {
            if (prop == null)
            {
                return;
            }

            var entry = _farmStore.Get(prop.Id);
            entry.WorkingCount = Mathf.Max(0, entry.WorkingCount - 1);
        }

        private GameObject GetGameObject(Vector2 screenPosition)
        {
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.collider.gameObject;
            }

            return null;
        }
    }
}