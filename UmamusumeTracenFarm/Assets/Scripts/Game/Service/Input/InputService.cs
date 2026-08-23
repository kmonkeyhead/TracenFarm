using System;
using Game.Input;
using Game.Service.Gesture;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Service.Input
{
    public class InputService : IDisposable
    {
        private readonly PlayerControls _controls;
        private readonly Subject<bool> _walk = new Subject<bool>();
        private readonly Subject<Vector2> _move  = new Subject<Vector2>();
        private readonly Subject<bool> _leftClick  = new Subject<bool>();
        private readonly Subject<InteractionInput> _interaction = new();
        private readonly ReactiveProperty<Vector2> _pointerPosition  = new ReactiveProperty<Vector2>();
        public Observable<InteractionInput> Interaction => _interaction;
        public Observable<Vector2> PointerPosition => _pointerPosition;
        public Observable<bool> LeftClick => _leftClick;
        
        public Observable<Vector2> Move => _move;
        public Observable<bool> Walk => _walk;

        public InputService()
        {
            _controls = new PlayerControls();
            EnableInput();
        }

        public void EnableInput()
        {
            _controls.Player.Enable();

            _controls.Player.Move.performed += OnMove;
            _controls.Player.Move.canceled += OnMove;
            
            _controls.Player.Interactwalk.performed += OnWalk;
            _controls.Player.Interactwalk.canceled += OnWalk;

            _controls.Player.LeftClick.performed += OnLeftClick;
            _controls.Player.LeftClick.canceled += OnLeftClick;
            _controls.Player.Interact.performed += OnInteraction;
            _controls.Player.Interact.canceled += OnInteraction;

            _controls.Player.PointerPosition.performed += OnPointerPosition;
        }

        private void OnPointerPosition(InputAction.CallbackContext obj)
        {
            _pointerPosition.OnNext(obj.ReadValue<Vector2>());
        }

        private void OnLeftClick(InputAction.CallbackContext obj)
        {
            _leftClick.OnNext(obj.ReadValueAsButton());
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            _move.OnNext(obj.ReadValue<Vector2>());
        }
        
        private void OnWalk(InputAction.CallbackContext obj)
        {
            _walk.OnNext(obj.ReadValueAsButton());
        }

        private void OnInteraction(InputAction.CallbackContext context)
        {
            Vector2 screenPosition = _controls.Player.PointerPosition.ReadValue<Vector2>();
            var input = new InteractionInput(context.ReadValueAsButton(), screenPosition, context.time);

            _interaction.OnNext(input);
        }

        public void DisableInput()
        {
            _controls.Player.Move.performed -= OnMove;
            _controls.Player.Move.canceled -= OnMove;
            
            _controls.Player.Interactwalk.performed -= OnWalk;
            _controls.Player.Interactwalk.canceled -= OnWalk;

            _controls.Player.LeftClick.performed -= OnLeftClick;
            _controls.Player.LeftClick.canceled -= OnLeftClick;
            _controls.Player.Interact.performed -= OnInteraction;
            _controls.Player.Interact.canceled -= OnInteraction;

            _controls.Player.PointerPosition.performed -= OnPointerPosition;

            _controls.Player.Disable();
        }


        public void Dispose()
        {
            DisableInput();
            _controls?.Dispose();
            _pointerPosition?.Dispose();
        }
    }
    
    public  record  InteractionInput(bool IsPressed, Vector2 ScreenPosition, double Time);
}
