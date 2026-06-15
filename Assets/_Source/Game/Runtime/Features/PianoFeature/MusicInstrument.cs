using Game.Runtime.MusicInstrumentSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class MusicInstrument : MonoBehaviour, IInstrument
    {
        public Transform SeatPoint;
        public float MaxViewAngle;
        
        [field: SerializeField] public MusicalInstrumentType Type { get; private set; }
        public NotesPlayer NotesPlayer { get; private set; }

        [Inject]
        private void Construct(NotesPlayer notesPlayer)
        {
            NotesPlayer = notesPlayer;
        }
    }
}