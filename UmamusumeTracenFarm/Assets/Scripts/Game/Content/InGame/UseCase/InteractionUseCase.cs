using System;
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

                if (targetObject.TryGetComponent<IProp>(out var prop))
                {
                    if (prop.CanInteract(_actorStore.MyActor.Position))
                    {
                        CheckAndInteract(prop, TimeSpan.Zero);
                    }
                }
            }
            else if (holdGesturePayload.GestureType == HoldGestureType.Hold)
            {
                if (_interact || _lastProp == null)
                {
                    return;
                }

                CheckAndInteract(_lastProp, holdGesturePayload.HoldingTime);
            }
            else if (holdGesturePayload.GestureType == HoldGestureType.End)
            {
                if (_lastProp == null)
                {
                    return;
                }

                if (_interact)
                {
                    // Hold가 실제로 시작됐다.
                    StopInteract(_lastProp);
                }
                else if (_lastProp is IClickGestureReceiver)
                {
                    ClickInteract(_lastProp);
                }

                _interact = false;
                _lastProp = null;
            }
        }

        private void CheckAndInteract(IProp prop, TimeSpan holdingTime)
        {
            var clickReceiver = prop as IClickGestureReceiver;
            var holdReceiver = prop as IHoldGestureReceiver;

            if (clickReceiver == null && holdReceiver == null)
            {
                return;
            }

            _lastProp = prop;

            if (holdReceiver == null)
            {
                return;
            }

            if (holdingTime < holdReceiver.StartHoldTime)
            {
                return;
            }

            var entry = _farmStore.Get(prop.Id);
            _interact = true;
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

        private void ClickInteract(IProp prop)
        {
            if (prop == null)
            {
                return;
            }

            var entry = _farmStore.Get(prop.Id);
            _farmWorkUseCase.Harvest(entry.Id, _actorStore.MyActor.Id);
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