using Game.Runtime.Configs;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.RhythmSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class MusicInstrument : MonoBehaviour, IInstrument
    {
        public PianoKeysConfig PianoKeysConfig;
        public Transform SeatPoint;
        public float MaxViewAngle;
        [field: SerializeField] private RhythmSheetVisualizer _sheetVisualizer;
        
        public MusicalInstrumentType Type => MusicalInstrumentType.MainPiano;
        public NotesPlayer NotesPlayer { get; private set; }
        public IRhythmSheet RhythmSheet { get; private set; }
        public ISheetVisualizer SheetVisualizer => _sheetVisualizer;

        [Inject]
        private void Construct(NotesPlayer notesPlayer, 
            Orchestra orchestra, IRhythmSheet rhythmSheet)
        {
            NotesPlayer = notesPlayer;
            RhythmSheet = rhythmSheet;
            orchestra.AddInstrument(this);
        }
    }
}