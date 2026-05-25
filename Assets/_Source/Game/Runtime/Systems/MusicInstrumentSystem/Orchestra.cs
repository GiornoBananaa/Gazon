using System;
using System.Collections.Generic;
using Game.Runtime.RhythmSystem;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class Orchestra
    {
        private class InstrumentTrack
        {
            public MusicalInstrumentType Type;
            public IInstrument Instrument;
            public bool Finished;
            public Note[] Notes;
        }
        
        private readonly List<IInstrument> _instruments = new();
        private readonly HashSet<IInstrument> _usedInstruments = new();
        private readonly List<InstrumentTrack> _tracks = new();

        private int _activeTracks;
        private int _finishedTracks;
        
        public event Action OnCompleted;
        
        public void AddInstrument(IInstrument instrument)
        {
            _instruments.Add(instrument);
        }

        public void AddTrack(MusicalInstrumentType musicalInstrumentType, Note[] notes)
        {
            _tracks.Add(new InstrumentTrack { Type = musicalInstrumentType, Notes = notes });
        }

        public void SetSheet(MusicalInstrumentType musicalInstrumentType, List<RhythmKey>[] keys)
        {
            foreach (var instrument in _instruments)
            {
                if(instrument.Type != musicalInstrumentType) continue;
                instrument.RhythmSheet.SetKeys(keys);
                instrument.SheetVisualizer.SetLengthInSeconds(1.5f);
                instrument.SheetVisualizer.Show();
                break;
            }
        }
        
        public void Play(float delay = 0)
        {
            _usedInstruments.Clear();
            _finishedTracks = 0;
            _activeTracks = 0;
            foreach (var track in _tracks)
            {
                track.Instrument = null;
                foreach (var instrument in _instruments)
                {
                    if (instrument.Type == track.Type && !_usedInstruments.Contains(instrument))
                    {
                        instrument.NotesPlayer.Play(track.Notes, delay);
                        track.Instrument = instrument;
                        _usedInstruments.Add(instrument);
                        instrument.NotesPlayer.OnCompleted += OnFinishedInstrument;
                        _activeTracks++;
                        break;
                    }
                }
            }
        }

        public void Continue()
        {
            foreach (var instrument in _usedInstruments)
            {
                instrument.NotesPlayer.Continue();
            }
        }
        
        public void Stop()
        {
            foreach (var instrument in _usedInstruments)
            {
                instrument.NotesPlayer.Stop();
            }
        }
        
        public void ClearTracks()
        {
            _tracks.Clear();
            _usedInstruments.Clear();
        }

        private void OnFinishedInstrument()
        {
            _finishedTracks++;
            if (_finishedTracks >= _activeTracks)
            {
                foreach (var instrument in _usedInstruments)
                {
                    instrument.NotesPlayer.OnCompleted -= OnFinishedInstrument;
                    instrument.SheetVisualizer.Hide();
                }
                Stop();
                OnCompleted?.Invoke();
            }
        }
    }
}