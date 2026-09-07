using System;
using Game.Content.InGame.Payload;
using Game.Content.InGame.Interaction;
using Game.Content.InGame.Store;
using Game.Service.Gesture;
using UnityEngine;

namespace Game.Content.InGame.UseCase
{
    public class InteractionUseCase
    {
        private readonly Camera _camera;
        private readonly ActorStore _actorStore;
        private bool _interact;
        private IInteractable _lastInteractable;
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

                if (targetObject.TryGetComponent<IInteractable>(out var interactable))
                {
                    if (interactable.CanInteract(_actorStore.MyActor.Position))
                    {
                        CheckAndInteract(interactable, TimeSpan.Zero);
                    }
                }
            }
            else if (holdGesturePayload.GestureType == HoldGestureType.Hold)
            {
                if (_interact || _lastInteractable == null)
                {
                    return;
                }

                CheckAndInteract(_lastInteractable, holdGesturePayload.HoldingTime);
            }
            else if (holdGesturePayload.GestureType == HoldGestureType.End)
            {
                if (_lastInteractable == null)
                {
                    return;
                }

                if (_interact)
                {
                    // Hold가 실제로 시작됐다.
                    StopInteract(_lastInteractable);
                }
                else if (_lastInteractable is IClickGestureReceiver)
                {
                    ClickInteract(_lastInteractable);
                }

                _interact = false;
                _lastInteractable = null;
            }
        }

        private void CheckAndInteract(IInteractable interactable, TimeSpan holdingTime)
        {
            var clickReceiver = interactable as IClickGestureReceiver;
            var holdReceiver = interactable as IHoldGestureReceiver;

            if (clickReceiver == null && holdReceiver == null)
            {
                return;
            }

            _lastInteractable = interactable;

            if (holdReceiver == null)
            {
                return;
            }

            if (holdingTime < holdReceiver.StartHoldTime)
            {
                return;
            }

            var entry = _farmStore.Get(interactable.Id);
            _interact = true;
            _farmWorkUseCase.StartInteracting(entry.Id, _actorStore.MyActor.Id);
        }

        private void StopInteract(IInteractable interactable)
        {
            if (interactable == null)
            {
                return;
            }

            var entry = _farmStore.Get(interactable.Id);
            _farmWorkUseCase.StopInteracting(entry.Id, _actorStore.MyActor.Id);
        }

        private void ClickInteract(IInteractable interactable)
        {
            if (interactable == null)
            {
                return;
            }

            var entry = _farmStore.Get(interactable.Id);
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