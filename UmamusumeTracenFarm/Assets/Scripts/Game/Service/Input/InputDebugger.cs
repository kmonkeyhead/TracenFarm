using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Game.Service.Input
{
    public sealed class InputDebugger : MonoBehaviour
    {
        public event Action<int> NumberPressed;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (WasPressed(keyboard.digit0Key, keyboard.numpad0Key)) Publish(0);
            if (WasPressed(keyboard.digit1Key, keyboard.numpad1Key)) Publish(1);
            if (WasPressed(keyboard.digit2Key, keyboard.numpad2Key)) Publish(2);
            if (WasPressed(keyboard.digit3Key, keyboard.numpad3Key)) Publish(3);
            if (WasPressed(keyboard.digit4Key, keyboard.numpad4Key)) Publish(4);
            if (WasPressed(keyboard.digit5Key, keyboard.numpad5Key)) Publish(5);
            if (WasPressed(keyboard.digit6Key, keyboard.numpad6Key)) Publish(6);
            if (WasPressed(keyboard.digit7Key, keyboard.numpad7Key)) Publish(7);
            if (WasPressed(keyboard.digit8Key, keyboard.numpad8Key)) Publish(8);
            if (WasPressed(keyboard.digit9Key, keyboard.numpad9Key)) Publish(9);
            if (keyboard.f10Key.wasPressedThisFrame) Publish(10);
        }

        private static bool WasPressed(KeyControl digit, KeyControl numpad)
        {
            return digit.wasPressedThisFrame || numpad.wasPressedThisFrame;
        }

        private void Publish(int number)
        {
            Debug.Log($"Debug input: {number}");
            NumberPressed?.Invoke(number);
        }
    }
}
