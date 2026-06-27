using System.Collections.Generic;
using System.Linq;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.RhythmSystem;
using Game.Runtime.ScenarioSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class RhythmGameController
    {
        private readonly IEnumerable<IMusicFileReader> _musicFilePlayers;
        private readonly Orchestra _orchestra;
        private readonly IRhythmKeyGenerator _keyGenerator;
        private readonly MusicLibrary _musicLibrary;
        private readonly ScenarioPlayer _scenarioPlayer;
        
        public RhythmGameController(MusicLibrary musicLibrary, Orchestra orchestra, IRhythmKeyGenerator keyGenerator, ScenarioPlayer scenarioPlayer)
        {
            _musicLibrary = musicLibrary;
            _keyGenerator = keyGenerator;
            _orchestra = orchestra;
            _scenarioPlayer = scenarioPlayer;
        }
        
        public void Start(int id, int keysCount, float maxSpeed, float maxNotesPerSecond)
        {
            _orchestra.ClearTracks();
            _keyGenerator.Clear();
            var track = _musicLibrary.GetMusicTrack(id);
            
            foreach (var typeNotesPair in track.Notes)
            {
                _orchestra.AddTrack(typeNotesPair.Key, typeNotesPair.Value);
                if(track.InstrumentsInSheet.Contains(typeNotesPair.Key.Type))
                    _keyGenerator.AddNotes(typeNotesPair.Key, typeNotesPair.Value);
            }
            
            List<RhythmKey>[] rhythmKeys = _keyGenerator.Generate(keysCount, maxNotesPerSecond);
            _orchestra.SetSheet(rhythmKeys);
            _orchestra.Play(2, maxSpeed);
            
            if(track.Scenario != null)
                _scenarioPlayer.PlayScenario(track.Scenario, _orchestra);
            
            _orchestra.OnCompleted += Quit;
        }
        
        public void Quit()
        {
            _orchestra.OnCompleted -= Quit;
            _orchestra.Finish();
        }
    }
}