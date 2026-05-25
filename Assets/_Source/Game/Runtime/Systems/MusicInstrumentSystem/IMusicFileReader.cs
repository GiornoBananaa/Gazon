using System.Collections.Generic;

namespace Game.Runtime.MusicInstrumentSystem
{
    public interface IMusicFileReader
    {
        bool FileIsValid(string path);
        IEnumerable<Note> GetNotes(string path);
    }
}