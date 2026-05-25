using Game.Runtime.InputFeature;
using Game.Runtime.RhythmSystem;

namespace Game.Runtime.PianoFeature
{
    public class RhythmInstrumentState : IPianoState
    {
        private readonly RhythmGameController _rhythmGameController;
        private readonly RhythmInstrumentInputListener _inputListener;
        private readonly InstrumentPlayStatistics _instrumentPlayStatistics;
        private readonly RhythmGameInstrumentKeyPresser _instrumentKeyPresser;
        private readonly IRhythmSheet _rhythmSheet;
        private readonly RhythmPianoSettings _rhythmPianoSettings;
        
        public RhythmInstrumentState(RhythmGameController rhythmGameController, RhythmInstrumentInputListener inputListener, 
            InstrumentPlayStatistics instrumentPlayStatistics, RhythmGameInstrumentKeyPresser instrumentKeyPresser, 
            IRhythmSheet rhythmSheet, RhythmPianoSettings rhythmPianoSettings)
        {
            _rhythmGameController = rhythmGameController;
            _inputListener = inputListener;
            _instrumentPlayStatistics = instrumentPlayStatistics;
            _instrumentKeyPresser = instrumentKeyPresser;
            _rhythmSheet = rhythmSheet;
            _rhythmPianoSettings = rhythmPianoSettings;
        }
        
        public void Enter()
        {
            _instrumentPlayStatistics.SetPianoComponents(null, _rhythmSheet);
            _instrumentKeyPresser.SetKeysCount(_rhythmPianoSettings.KeysCount);
            _rhythmGameController.Start(_rhythmPianoSettings.KeysCount, _rhythmPianoSettings.MaxKeysPerSecond);
            _inputListener.EnableInput();
        }

        public void Exit()
        {
            _inputListener.DisableInput();
            _rhythmGameController.Quit();
        }
    }
}