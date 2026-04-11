using Game.Runtime.StateMachineSystem;

namespace Game.Runtime.PianoRhythmSystem
{
    public interface IPianoState : IState
    {
        public void PressKey(int index);
        public void ReleaseKey(int index);
        public void OctaveUp();
        public void OctaveDown();
        public void PressPedal();
        public void ReleasePedal();
    }
}