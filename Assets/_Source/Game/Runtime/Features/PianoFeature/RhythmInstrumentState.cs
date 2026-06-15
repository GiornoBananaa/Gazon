using Game.Runtime.Configs;
using Game.Runtime.InputFeature;
using Game.Runtime.RhythmSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class RhythmInstrumentState : IInstrumentState
    {
        private readonly RhythmGameController _rhythmGameController;
        private readonly RhythmInstrumentInputListener _inputListener;
        private readonly InstrumentPlayStatistics _instrumentPlayStatistics;
        private readonly IRhythmSheet _rhythmSheet;
        private readonly RhythmGameSettings _rhythmGameSettings;
        private readonly InstrumentKeysConfig _keysConfig;
        
        public RhythmInstrumentState(RhythmGameController rhythmGameController, RhythmInstrumentInputListener inputListener, 
            InstrumentPlayStatistics instrumentPlayStatistics, IRhythmSheet rhythmSheet, RhythmGameSettings rhythmGameSettings, InstrumentKeysConfig keysConfig)
        {
            _rhythmGameController = rhythmGameController;
            _inputListener = inputListener;
            _instrumentPlayStatistics = instrumentPlayStatistics;
            _rhythmSheet = rhythmSheet;
            _rhythmGameSettings = rhythmGameSettings;
            _keysConfig = keysConfig;
        }
        
        public void Enter()
        {
            _instrumentPlayStatistics.SetPianoComponents(_keysConfig.Notes.Length, null, _rhythmSheet);
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