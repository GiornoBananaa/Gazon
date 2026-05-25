using System.Collections.Generic;
using System.Linq;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class MusicLibrary
    {
        private readonly IEnumerable<IMusicFileReader> _musicFileReaders;
        private readonly List<Note[]> _notes = new();

        public MusicLibrary(IEnumerable<IMusicFileReader> musicFileReaders)
        {
            _musicFileReaders = musicFileReaders;
        }
        
        public void Initialize()
        {
            Note[] notes = null;
            //TODO: all music files loading from folder
            string path = @"C:\Users\alexe\Downloads\chopin-nocturne-op-9-no-2-e-flat-major.mid";
            foreach (var fileReader in _musicFileReaders)
            {
                if(!fileReader.FileIsValid(path)) continue;
                notes = fileReader.GetNotes(path).ToArray();
                break;
            }
            _notes.Add(notes);
        }
        
        public Note[] GetNotes(int id)
        {
            return _notes[id];
        }
    }
}