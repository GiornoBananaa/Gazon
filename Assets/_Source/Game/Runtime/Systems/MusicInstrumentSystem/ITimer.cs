using System;

namespace Game.Runtime.MusicInstrumentSystem
{
    public interface ITimer
    {
        event Action<float> OnTimeChanged;
        event Action OnCompleted;
    }
}