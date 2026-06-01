using System.Collections.Generic;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.RhythmSystem;

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
            var notes = _musicLibrary.GetMusicTrack(id).Notes;
            foreach (var typeNotesPair in notes)
            {
                List<RhythmKey>[] rhythmKeys = _keyGenerator.Generate(typeNotesPair.Value, keysCount, maxNotesPerSecond);
            
                _orchestra.AddTrack(typeNotesPair.Key, typeNotesPair.Value);
                _orchestra.Play(2, maxSpeed);
                if(typeNotesPair.Key == MusicalInstrumentType.MainPiano)
                    _orchestra.SetSheet(typeNotesPair.Key, rhythmKeys);
            }
            
            _orchestra.OnCompleted += Quit;
        }
        
        public void Quit()
        {
            _orchestra.OnCompleted -= Quit;
            _orchestra.Finish();
        }
    }
}