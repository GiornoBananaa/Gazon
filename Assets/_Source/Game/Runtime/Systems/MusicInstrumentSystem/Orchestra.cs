using System;
using System.Collections.Generic;
using Game.Runtime.Configs;
using Game.Runtime.PianoFeature;
using Game.Runtime.RhythmSystem;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class Orchestra
    {
        private class InstrumentTrack
        {
            public IInstrument Instrument;
            public bool Finished;
            public Note[] Notes;
        }
        
        private readonly List<MainMusicInstrument> _mainInstruments = new();
        private readonly Dictionary<InstrumentId,IInstrument> _usedInstruments = new();
        private readonly List<InstrumentTrack> _tracks = new();
        private readonly List<MusicInstrument> _spawnedInstruments = new();
        private readonly Dictionary<MusicalInstrumentType, ObjectPool<MusicInstrument>> _instrumentPools = new();
        private readonly Dictionary<MusicalInstrumentType, MusicInstrument> _prefabs = new();
        private readonly IRhythmSheet _rhythmSheet;
        
        private int _activeTracks;
        private int _finishedTracks;
        
        public event Action OnCompleted;

        public Orchestra(IRhythmSheet rhythmSheet, OrchestraConfig orchestraConfig)
        {
            _rhythmSheet = rhythmSheet;
            foreach (var instrument in orchestraConfig.Instruments)
            {
                _prefabs.Add(instrument.Type, instrument.Prefab);
                if(!_instrumentPools.ContainsKey(instrument.Type))
                    _instrumentPools.Add(instrument.Type, new ObjectPool<MusicInstrument>(() => CreateInstrument(instrument.Type), OnGetInstrument, OnReleaseInstrument));
            }
        }
        
        public void AddInstrument(MainMusicInstrument instrument)
        {
            _mainInstruments.Add(instrument);
        }

        public void AddTrack(InstrumentId instrumentId, Note[] notes)
        {
            IInstrument trackInstrument = null;
            foreach (var instrument in _mainInstruments)
            {
                if (instrument.Type == instrumentId.Type && !_usedInstruments.ContainsValue(instrument))
                {
                    trackInstrument = instrument;
                    break;
                }
            }

            if (trackInstrument == null)
                trackInstrument = _instrumentPools[instrumentId.Type].Get();
            
            _usedInstruments.Add(instrumentId, trackInstrument);
            
            _tracks.Add(new InstrumentTrack { Notes = notes, Instrument = trackInstrument});
        }

        public void SetSheet(IEnumerable<IEnumerable<RhythmKey>> rhythmKeys)
        {
            _rhythmSheet.SetKeys(rhythmKeys);
        }
        
        public void Play(float delay = 0, float speed = 1)
        {
            _finishedTracks = 0;
            _activeTracks = 0;
            foreach (var track in _tracks)
            {
                if(track.Instrument == null) continue;
                track.Instrument.NotesPlayer.Play(track.Notes, delay, speed);
                track.Instrument.NotesPlayer.OnCompleted += OnFinishedInstrument;
                _activeTracks++;
            }

            foreach (var usedInstrument in _usedInstruments)
            {
                _rhythmSheet.SetInstrument(usedInstrument.Key, usedInstrument.Value);
            }
            
            _rhythmSheet.StartSheet();
            foreach (var mainInstrument in _mainInstruments)
            {
                mainInstrument.SheetVisualizer.SetLengthInSeconds(1.5f);
                mainInstrument.SheetVisualizer.Show();
                break;
            }
        }

        public void Continue()
        {
            foreach (var instrument in _usedInstruments.Values)
            {
                instrument.NotesPlayer.Continue();
            }
        }
        
        public void Stop()
        {
            foreach (var instrument in _usedInstruments.Values)
            {
                instrument.NotesPlayer.Stop();
            }
        }
        
        public void Finish()
        {
            foreach (var instrument in _usedInstruments.Values)
            {
                instrument.NotesPlayer.OnCompleted -= OnFinishedInstrument;
                if(instrument is MainMusicInstrument mainInstrument)
                    mainInstrument.SheetVisualizer.Hide();
            }
            
            foreach (var spawned in _spawnedInstruments.ToArray())
            {
                _instrumentPools[spawned.Type].Release(spawned);
            }
            
            Stop();
            OnCompleted?.Invoke();
        }
        
        public void ClearTracks()
        {
            foreach (var track in _tracks)
            {
                track.Instrument.NotesPlayer.ClearModifications();
            }
            _rhythmSheet?.Clear();
            _tracks.Clear();
            _usedInstruments.Clear();
            foreach (var spawned in _spawnedInstruments.ToArray())
            {
                _instrumentPools[spawned.Type].Release(spawned);
            }
        }

        private void OnFinishedInstrument()
        {
            _finishedTracks++;
            if (_finishedTracks >= _activeTracks)
            {
                Finish();
            }
        }

        private MusicInstrument CreateInstrument(MusicalInstrumentType musicalInstrumentType)
        {
            return Object.Instantiate(_prefabs[musicalInstrumentType]);
        }
        
        private void OnGetInstrument(MusicInstrument instrument)
        {
            _spawnedInstruments.Add(instrument);
            instrument.gameObject.SetActive(true);
        }
        
        private void OnReleaseInstrument(MusicInstrument instrument)
        {
            _spawnedInstruments.Remove(instrument);
            instrument.gameObject.SetActive(false);
        }
    }
}