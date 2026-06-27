using Game.Runtime.MusicInstrumentSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class MusicInstrument : MonoBehaviour, IInstrument
    {
        [field: SerializeField] public Transform SeatPoint { get; private set; }
        [field: SerializeField] public float SheetViewAngleY { get; private set; }
        [field: SerializeField] public float SheetViewAngleX { get; private set; }
        [field: SerializeField] public MusicalInstrumentType Type { get; private set; }
        [field: SerializeField] public Vector2 Area { get; private set; }
        public NotesPlayer NotesPlayer { get; private set; }

        [Inject]
        private void Construct(NotesPlayer notesPlayer)
        {
            NotesPlayer = notesPlayer;
        }
    }
}