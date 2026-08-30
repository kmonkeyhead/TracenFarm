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
        private readonly FarmWorkUseCase _farmWorkUseCase;

        public InteractionUseCase(Camera camera, ActorStore actorStore, FarmStore farmStore, FarmWorkUseCase farmWorkUseCase)
        {
            _camera = camera;
            _actorStore = actorStore;
            _farmStore = farmStore;
            _farmWorkUseCase = farmWorkUseCase;
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
            var entry = _farmStore.Get(prop.Id);
            _lastProp = prop;
            _farmWorkUseCase.StartInteracting(entry.Id, _actorStore.MyActor.Id);
        }

        private void StopInteract(IProp prop)
        {
            if (prop == null)
            {
                return;
            }

            var entry = _farmStore.Get(prop.Id);
            _farmWorkUseCase.StopInteracting(entry.Id, _actorStore.MyActor.Id);
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