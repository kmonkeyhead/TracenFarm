using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Service.Gesture
{
    using System;
    using System.Collections.Generic;
    using Input;
    using R3;

    public sealed class ClickGesture : IDisposable
    {
        private Vector2 _lastInputPosition;
        private DateTime _lastInputTime;
        private bool _inputPressed;
        private bool _pressed;
        private bool _active;
        private readonly Queue<InteractionInput> _interactionEventQueue = new();
        private readonly IDisposable _disposable;

        private readonly Subject<HoldGesturePayload> _holdGestureSubject = new();
        public Observable<HoldGesturePayload> HoldGesture => _holdGestureSubject;

        private CancellationTokenSource _cts = new();
        private TimeSpan _holdingTime;

        public ClickGesture(InputService inputService)
        {
            var builder = Disposable.CreateBuilder();

            builder.Add(inputService.Interaction.Subscribe(OnInteraction));
            builder.Add(inputService.PointerPosition.Subscribe(OnPointerPosition));

            _disposable = builder.Build();
            Active();
        }

        public void Active()
        {
            if (_active)
            {
                return;
            }

            _active = true;
            UpdateAsync(_cts.Token).Forget();
        }

        private async UniTask UpdateAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    while (_interactionEventQueue.Count > 0)
                    {
                        var input = _interactionEventQueue.Dequeue();
                        if (input.IsPressed)
                        {
                            _lastInputPosition = input.ScreenPosition;

                            if (!_pressed)
                            {
                                _pressed = true;
                                _lastInputTime = DateTime.Now;
                                //홀드 시작
                                _holdingTime = TimeSpan.Zero;
                                _holdGestureSubject.OnNext(new HoldGesturePayload(HoldGestureType.Start, input.ScreenPosition, _holdingTime));
                            }
                        }
                        else
                        {
                            //마우스를 땠음
                            if (_pressed)
                            {
                                _pressed = false;
                                _lastInputPosition = input.ScreenPosition;
                                _holdingTime = DateTime.Now - _lastInputTime;
                                _holdGestureSubject.OnNext(new HoldGesturePayload(HoldGestureType.End, input.ScreenPosition, _holdingTime));
                            }
                        }
                    }

                    if (_pressed)
                    {
                        //홀드 중
                        _holdingTime = DateTime.Now - _lastInputTime;
                        _holdGestureSubject.OnNext(new HoldGesturePayload(HoldGestureType.Hold, _lastInputPosition, _holdingTime));
                    }

                    await UniTask.Yield(ct);
                }
            }
            finally
            {
                _active = false;
            }
        }

        private void OnInteraction(InteractionInput input)
        {
            _inputPressed = input.IsPressed;
            _interactionEventQueue.Enqueue(input);
        }

        private void OnPointerPosition(Vector2 position)
        {
            if (!_inputPressed)
            {
                return;
            }

            _interactionEventQueue.Enqueue(new InteractionInput(true, position, Time.unscaledTimeAsDouble));
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }
    }

    public enum HoldGestureType
    {
        Start,
        Hold,
        End
    }

    public record ClickGesturePayload(Vector2 Position);

    public record HoldGesturePayload(HoldGestureType GestureType,  Vector2 Position, TimeSpan HoldingTime);
}
