using Game.Runtime.RhythmSystem;

namespace Game.Runtime.MusicInstrumentSystem
{
    public interface IInstrument
    {
        public MusicalInstrumentType Type { get; }
        public NotesPlayer NotesPlayer { get; }
        public IRhythmSheet RhythmSheet { get; }
        public ISheetVisualizer SheetVisualizer { get; }
    }
}