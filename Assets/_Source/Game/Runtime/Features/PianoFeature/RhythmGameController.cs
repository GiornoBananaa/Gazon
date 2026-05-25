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
        
        public void Start(int keysCount, int maxSpeed)
        {
            Note[] notes = _musicLibrary.GetNotes(0);
            List<RhythmKey>[] rhythmKeys = _keyGenerator.Generate(notes, keysCount, maxSpeed);
            
            _orchestra.SetSheet(MusicalInstrumentType.MainPiano, rhythmKeys);
            _orchestra.AddTrack(MusicalInstrumentType.MainPiano, notes); //TODO: Multiple tracks with different instruments
            _orchestra.Play(2);
            
            _orchestra.OnCompleted += Quit;
        }
        
        public void Quit()
        {
            _orchestra.OnCompleted -= Quit;
        }
    }
}