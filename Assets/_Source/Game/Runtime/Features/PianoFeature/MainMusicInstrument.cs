using Game.Runtime.MusicInstrumentSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class MainMusicInstrument : MusicInstrument
    {
        [field: SerializeField] public RhythmSheetVisualizer SheetVisualizer { get; private set; }

        [Inject]
        private void Construct(Orchestra orchestra)
        {
            orchestra.AddInstrument(this);
        }
    }
}