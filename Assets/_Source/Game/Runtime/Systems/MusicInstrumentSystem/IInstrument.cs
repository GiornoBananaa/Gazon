
namespace Game.Runtime.MusicInstrumentSystem
{
    public interface IInstrument
    {
        public MusicalInstrumentType Type { get; }
        public NotesPlayer NotesPlayer { get; }
    }
}