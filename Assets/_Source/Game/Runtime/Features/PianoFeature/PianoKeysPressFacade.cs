using Game.Runtime.PianoRhythmSystem;

namespace Game.Runtime.PianoFeature
{
    public class PianoKeysPressFacade
    {
        private IPianoKeyPresser _pianoKeyPresser;
        private readonly PianoKeyPressStatistics _pianoStatistics;

        public PianoKeysPressFacade(PianoKeyPressStatistics pianoStatistics)
        {
            _pianoStatistics = pianoStatistics;
        }
        
        public void SetPianoKeyPresser(IPianoKeyPresser pianoKeyPresser)
        {
            _pianoKeyPresser = pianoKeyPresser;
            _pianoStatistics.SetPianoKeyPresser(pianoKeyPresser);
        }
        
        public void PressKey(int keyIndex) => _pianoKeyPresser.PressKey(keyIndex);
        
        public void ReleaseKey(int keyIndex) => _pianoKeyPresser.ReleaseKey(keyIndex);
        
        public void OctaveUp() => _pianoKeyPresser.OctaveUp();
        
        public void OctaveDown() => _pianoKeyPresser.OctaveDown();
        
        public void PressPedal() => _pianoKeyPresser.PressPedal();
        
        public void ReleasePedal() => _pianoKeyPresser.ReleasePedal();
        
    }
}