using Game.Runtime.PianoRhythmSystem;

namespace Game.Runtime.PianoFeature
{
    public class FreePianoState : IPianoState
    {
        private readonly IPianoKeyPresser _pianoKeyPresser;
        private readonly PianoKeysPressFacade _pianoKeysPressFacade;
        
        public FreePianoState(FreePianoKeyPresser pianoKeyPresser, PianoKeysPressFacade pianoKeysPressFacade)
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