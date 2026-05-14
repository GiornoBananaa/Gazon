using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "BoidsPianoConfig", menuName = "Game/Boids/BoidsPianoConfig")]
    public class BoidsPianoBindConfig : ScriptableObject
    {
        [Header("Birds")]
        public int NotesCountForMaxForce = 3;
        public float MinForce = 0;
        public float MaxForce = 30;
    }
}