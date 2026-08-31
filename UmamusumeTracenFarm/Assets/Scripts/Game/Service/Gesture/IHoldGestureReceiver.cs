using System;

namespace Game.Service.Gesture
{
    public interface IHoldGestureReceiver
    {
        TimeSpan StartHoldTime { get; }
    }
}