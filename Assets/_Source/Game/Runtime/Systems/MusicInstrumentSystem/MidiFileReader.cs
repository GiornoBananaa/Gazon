using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class MidiFileReader : IMusicFileReader
    {
        private MidiFile _midiFile;
        private string _pathFile;
        private const int NOTE_NUMBER_START = 21;
        
        public bool FileIsValid(string path)
        {
            _pathFile = path;
            _midiFile = MidiFile.Read(path);
            return IsValid(_midiFile);
        }
        
        public IEnumerable<Note> GetNotes(string path)
        {
            if(_midiFile != null && _pathFile != path)
            {
                _midiFile = MidiFile.Read(path, new ReadingSettings
                {
                    UnknownChunkIdPolicy = UnknownChunkIdPolicy.Skip
                });
            }
            
            if(!IsValid(_midiFile)) throw new ArgumentNullException(nameof(path));
            foreach (var note in _midiFile.GetNotes())
            {
                yield return new Note
                {
                    NoteNumber = note.NoteNumber - NOTE_NUMBER_START,
                    StartTime = (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, _midiFile.GetTempoMap()).TotalSeconds,
                    EndTime = (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.EndTime, _midiFile.GetTempoMap()).TotalSeconds
                };
            }
        }

        private bool IsValid(MidiFile midiFile)
        {
            return midiFile != null;
        }
    }
}