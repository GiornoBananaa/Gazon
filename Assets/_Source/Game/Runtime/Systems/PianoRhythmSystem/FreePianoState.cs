namespace Game.Runtime.PianoRhythmSystem
{
    public class FreePianoState : IPianoState
    {
        private readonly IPianoKeyPresser _pianoKeyPresser;
        
        public FreePianoState(FreePianoKeyPresser pianoKeyPresser)
        {
            _pianoKeyPresser = pianoKeyPresser;
        }
        
        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }

        public void PressKey(int index) => _pianoKeyPresser.PressKey(index);

        public void ReleaseKey(int index) => _pianoKeyPresser.ReleaseKey(index);
        
        public void OctaveUp() => _pianoKeyPresser.OctaveUp();

        public void OctaveDown() => _pianoKeyPresser.OctaveDown();
        
        public void PressPedal() => _pianoKeyPresser.PressPedal();

        public void ReleasePedal() => _pianoKeyPresser.ReleasePedal();
    }
}