using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class MidiFileReader : IMusicFileReader
    {
        public bool FileIsValid(string path)
        {
            return path.EndsWith(".mid");
        }
        
        public IEnumerable<Note> GetNotes(string path)
        {
            var midiFile = MidiFile.Read(path, new ReadingSettings
            {
                UnknownChunkIdPolicy = UnknownChunkIdPolicy.Skip,
                NotEnoughBytesPolicy = NotEnoughBytesPolicy.Abort,
                SilentNoteOnPolicy = SilentNoteOnPolicy.NoteOff
            });
            
            if(midiFile == null) throw new ArgumentException(nameof(path));
            
            var tempoMap = midiFile.GetTempoMap();
            
            foreach (var note in midiFile.GetNotes())
            {
                yield return new Note
                (
                    note.NoteNumber,
                    note.Velocity / 127f,
                    (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds,
                    (float)TimeConverter.ConvertTo<MetricTimeSpan>(note.EndTime, tempoMap).TotalSeconds
                );
            }
        }
    }
}