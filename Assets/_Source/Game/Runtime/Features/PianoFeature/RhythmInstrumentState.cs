using Game.Runtime.InputFeature;
using Game.Runtime.RhythmSystem;

namespace Game.Runtime.PianoFeature
{
    public class RhythmInstrumentState : IInstrumentState
    {
        private readonly RhythmGameController _rhythmGameController;
        private readonly RhythmInstrumentInputListener _inputListener;
        private readonly InstrumentPlayStatistics _instrumentPlayStatistics;
        private readonly IRhythmSheet _rhythmSheet;
        private readonly RhythmGameSettings _rhythmGameSettings;
        
        public RhythmInstrumentState(RhythmGameController rhythmGameController, RhythmInstrumentInputListener inputListener, 
            InstrumentPlayStatistics instrumentPlayStatistics, IRhythmSheet rhythmSheet, RhythmGameSettings rhythmGameSettings)
        {
            _rhythmGameController = rhythmGameController;
            _inputListener = inputListener;
            _instrumentPlayStatistics = instrumentPlayStatistics;
            _rhythmSheet = rhythmSheet;
            _rhythmGameSettings = rhythmGameSettings;
        }
        
        public void Enter()
        {
            _instrumentPlayStatistics.SetPianoComponents(null, _rhythmSheet);
            _rhythmGameController.Start(_rhythmGameSettings.MusicTrackId, _rhythmGameSettings.KeysCount, _rhythmGameSettings.MusicSpeed, _rhythmGameSettings.MaxNotesPerSecond);
            _inputListener.SetKeysCount(_rhythmGameSettings.KeysCount);
            _inputListener.EnableInput();
        }

        public void Exit()
        {
            _inputListener.DisableInput();
            _rhythmGameController.Quit();
        }
    }
}