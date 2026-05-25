using System;
using System.Collections.Generic;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using Game.Runtime.ServiceSystem;
using UnityEngine;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class NotesPlayer : IUpdatable
    {
        private readonly AudioSound[] _sounds;
        private readonly IInstrumentNoteTweener _instrumentNoteTweener;
        private readonly List<Note> _pressedNotes = new();
        private readonly List<Note> _remainingNotes = new();
        private readonly HashSet<Note> _mutedNotes = new();
        private readonly Dictionary<Note, Note> _forcedChangedNotes = new();
        private float _time;
        private bool _isPlaying;
        
        public event Action<float> OnTimeChanged;
        public event Action OnCompleted;
        
        public NotesPlayer(IInstrumentNoteTweener noteTweener, ServiceUpdater serviceUpdater, PianoKeysConfig pianoKeysConfig)
        {
            _sounds = pianoKeysConfig.Notes;
            _instrumentNoteTweener = noteTweener;
            serviceUpdater.Add(this);
        }

        public void Play(Note[] notes, float delay = 0)
        {
            _remainingNotes.Clear();
            _remainingNotes.AddRange(notes);
            _isPlaying = true;
            _time = -delay;
        }
        
        public void Continue()
        {
            _isPlaying = true;
        }
        
        public void Stop()
        {
            _isPlaying = false;
        }

        public void MuteNote(Note note)
        {
            _mutedNotes.Add(note);
        }
        
        public void UnmuteNote(Note note)
        {
            _mutedNotes.Remove(note);
        }
        
        public void ForceNoteChange(Note note, Note newNote)
        {
            Note searchNote = note;
            if (_forcedChangedNotes.ContainsKey(note))
                searchNote = _forcedChangedNotes[note];

            bool changed = false;
            for (int i = 0; i < _remainingNotes.Count; i++)
            {
                if(!_remainingNotes[i].Equals(searchNote)) continue;
                _remainingNotes[i] = newNote;
                changed = true;
                break;
            }
            if(!changed)
                _remainingNotes.Add(newNote);
            for (int i = 0; i < _pressedNotes.Count; i++)
            {
                if(!_pressedNotes[i].Equals(searchNote)) continue;
                _pressedNotes[i] = newNote;
                break;
            }
            
            _forcedChangedNotes[note] = newNote;
        }
        
        public void ForceStopNote(Note note)
        {
            for (int i = 0; i < _pressedNotes.Count; i++)
            {
                if(_mutedNotes.Contains(_remainingNotes[i])) continue;
                if(!_pressedNotes[i].Equals(_forcedChangedNotes[note]) && !_pressedNotes[i].Equals(note)) continue;
                OnNoteEnded(_remainingNotes[i]);
                _pressedNotes.RemoveAt(i);
                break;
            }
        }
        
        void IUpdatable.Update()
        {
            if (!_isPlaying) return;

            for (int i = 0; i < _pressedNotes.Count; i++)
            {
                if(_mutedNotes.Contains(_pressedNotes[i])) continue;
                if(_time < _pressedNotes[i].EndTime) break;
                OnNoteEnded(_pressedNotes[i]);
                _pressedNotes.RemoveAt(i);
            }
            
            for (int i = 0; i < _remainingNotes.Count; i++)
            {
                if(_mutedNotes.Contains(_remainingNotes[i])) continue;
                if(_time < _remainingNotes[i].StartTime) break;
                OnNoteStarted(_remainingNotes[i]);
                _pressedNotes.Add(_remainingNotes[i]);
                _remainingNotes.RemoveAt(i);
            }

            if (_remainingNotes.Count <= 0)
            {
                OnCompleted?.Invoke();
                Stop();
            }
            
            _time += Time.deltaTime;
            OnTimeChanged?.Invoke(_time);
        }
        
        private void OnNoteStarted(Note note)
        {
            _instrumentNoteTweener.StartNote(note.NoteNumber, _sounds[note.NoteNumber]);
        }
        
        private void OnNoteEnded(Note note)
        {
            _instrumentNoteTweener.EndNote(note.NoteNumber);
        }
    }
}