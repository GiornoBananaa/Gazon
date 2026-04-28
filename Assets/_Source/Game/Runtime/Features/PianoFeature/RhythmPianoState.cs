using Game.Runtime.PianoRhythmSystem;

namespace Game.Runtime.PianoFeature
{
    public class RhythmPianoState : IPianoState
    {
        private readonly IPianoKeyPresser _pianoKeyPresser;
        private readonly PianoKeysPressFacade _pianoKeysPressFacade;
        
        public RhythmPianoState(RhythmGamePianoKeyPresser pianoKeyPresser, PianoKeysPressFacade pianoKeysPressFacade)
        {
            _pianoKeyPresser = pianoKeyPresser;
            _pianoKeysPressFacade = pianoKeysPressFacade;
        }
        
        public void Enter()
        {
            _pianoKeysPressFacade.SetPianoKeyPresser(_pianoKeyPresser);
        }

        public void Exit()
        {
            
        }
    }
}