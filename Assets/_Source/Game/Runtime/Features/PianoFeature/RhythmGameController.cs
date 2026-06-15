using System.Collections.Generic;
using System.Linq;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.RhythmSystem;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class RhythmGameController
    {
        private readonly IEnumerable<IMusicFileReader> _musicFilePlayers;
        private readonly Orchestra _orchestra;
        private readonly IRhythmKeyGenerator _keyGenerator;
        private readonly MusicLibrary _musicLibrary;
        
        public RhythmGameController(MusicLibrary musicLibrary, Orchestra orchestra, IRhythmKeyGenerator keyGenerator)
        {
            _musicLibrary = musicLibrary;
            _keyGenerator = keyGenerator;
            _orchestra = orchestra;
        }
        
        public void Start(int id, int keysCount, float maxSpeed, float maxNotesPerSecond)
        {
            _orchestra.ClearTracks();
            var track = _musicLibrary.GetMusicTrack(id);
            List<Note> sheetNotes = new List<Note>();
            
            foreach (var typeNotesPair in track.Notes)
            {
                _orchestra.AddTrack(typeNotesPair.Key, typeNotesPair.Value);
                if(track.InstrumentsInSheet.Contains(typeNotesPair.Key.Type))
                {
                    sheetNotes.AddRange(typeNotesPair.Value);
                }
            }
            
            sheetNotes = sheetNotes.OrderBy(n=>n.StartTime).ToList();
            
            _orchestra.Play(2, maxSpeed);

            List<RhythmKey>[] rhythmKeys = _keyGenerator.Generate(sheetNotes.ToArray(), keysCount, maxNotesPerSecond);
            
            _orchestra.SetSheet(rhythmKeys);
            
            _orchestra.OnCompleted += Quit;
        }
        
        public void Quit()
        {
            _orchestra.OnCompleted -= Quit;
            _orchestra.Finish();
        }
    }
}