using System;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Configs;
using Game.Runtime.PianoFeature;
using Game.Runtime.RhythmSystem;
using Game.Runtime.Utils;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class Orchestra : ITimer
    {
        private class InstrumentTrack
        {
            public IInstrument Instrument;
            public InstrumentId Id;
            public bool Finished;
            public Note[] Notes;
        }
        
        private readonly List<MainMusicInstrument> _mainInstruments = new();
        private readonly List<InstrumentTrack> _tracks = new();
        private readonly List<MusicInstrument> _spawnedInstruments = new();
        private readonly Dictionary<MusicalInstrumentType, ObjectPool<MusicInstrument>> _instrumentPools = new();
        private readonly Dictionary<MusicalInstrumentType, MusicInstrument> _prefabs = new();
        private readonly IRhythmSheet _rhythmSheet;
        private readonly float _arcRadius;
        private readonly float _arcAngle;
        
        private MainMusicInstrument _currentMainInstrument;
        private int _activeTracks;
        private int _finishedTracks;
        private float _time;
        
        public event Action<float> OnTimeChanged;
        public event Action OnCompleted;

        public Orchestra(IRhythmSheet rhythmSheet, OrchestraConfig orchestraConfig)
        {
            _rhythmSheet = rhythmSheet;
            _arcRadius = orchestraConfig.MinArcRadius;
            _arcAngle = orchestraConfig.MaxArcAngle;
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
                if (instrument.Type == instrumentId.Type && _tracks.All(t => !ReferenceEquals(t.Instrument, instrument)))
                {
                    trackInstrument = instrument;
                    break;
                }
            }

            if (trackInstrument == null)
                trackInstrument = _instrumentPools[instrumentId.Type].Get();
            
            if(_currentMainInstrument == null && trackInstrument is MainMusicInstrument musicInstrument)
                _currentMainInstrument = musicInstrument;
            
            _tracks.Add(new InstrumentTrack { Notes = notes, Instrument = trackInstrument, Id = instrumentId });
        }

        public void SetSheet(IEnumerable<IEnumerable<RhythmKey>> rhythmKeys)
        {
            _rhythmSheet.SetKeys(rhythmKeys);
        }
        
        public void Play(float delay = 0, float speed = 1)
        {
            _finishedTracks = 0;
            _activeTracks = 0;
            _time = 0;
            foreach (var track in _tracks)
            {
                if(track.Instrument == null) continue;
                track.Instrument.NotesPlayer.Play(track.Notes, delay, speed);
                track.Instrument.NotesPlayer.OnCompleted += OnFinishedInstrument;
                track.Instrument.NotesPlayer.OnTimeChanged += OnTimeChange;;
                _activeTracks++;
            }

            float maxFirstNoteNumber = float.MinValue;
            float minFirstNoteNumber = float.MaxValue;
            int supportInstrumentsCount = 0;
            
            foreach (var track in _tracks)
            {
                int note = track.Notes[0].NoteNumber;
                if(note > maxFirstNoteNumber)
                    maxFirstNoteNumber = note;
                if(note < minFirstNoteNumber)
                    minFirstNoteNumber = note;
                if (track.Instrument is MusicInstrument && !ReferenceEquals(track.Instrument, _currentMainInstrument))
                    supportInstrumentsCount++;
            }

            float angleStep = _arcAngle / supportInstrumentsCount;
            float angle = -(_arcAngle / 2) + 90 + angleStep / 2;
            Vector3 center = _currentMainInstrument.transform.position;
            
            foreach (var track in _tracks)
            {
                _rhythmSheet.SetInstrument(track.Id, track.Instrument);
                if (track.Instrument is MusicInstrument musicInstrument && !ReferenceEquals(track.Instrument, _currentMainInstrument))
                {
                    Vector2 circlePos = MathUtils.PointOnCircle(_arcRadius, angle - Mathf.Lerp(-angle / 2, angle / 2,Mathf.InverseLerp(minFirstNoteNumber, maxFirstNoteNumber, track.Notes[0].NoteNumber)));
                    musicInstrument.transform.position = center + circlePos.GetVectorXZ();
                    musicInstrument.transform.rotation = Quaternion.LookRotation(center - musicInstrument.transform.position);
                    angle += angleStep;
                }
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
            foreach (var track in _tracks)
            {
                track.Instrument.NotesPlayer.Continue();
            }
        }
        
        public void Stop()
        {
            foreach (var track in _tracks)
            {
                track.Instrument.NotesPlayer.Stop();
            }
        }
        
        public void Finish()
        {
            foreach (var track in _tracks)
            {
                track.Instrument.NotesPlayer.OnCompleted -= OnFinishedInstrument;
                track.Instrument.NotesPlayer.OnTimeChanged -= OnTimeChange;
                if(track.Instrument is MainMusicInstrument mainInstrument)
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
            foreach (var spawned in _spawnedInstruments.ToArray())
            {
                _instrumentPools[spawned.Type].Release(spawned);
            }

            _currentMainInstrument = null;
        }

        private void OnTimeChange(float time)
        {
            if(time > _time)
                OnTimeChanged?.Invoke(time);
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