namespace Game.Runtime.PianoRhythmSystem
{
    public interface IPianoKeyPresser
    {
        void PressKey(int index);
        void ReleaseKey(int index);
        void OctaveUp();
        void OctaveDown();
        void PressPedal();
        void ReleasePedal();
    }
}