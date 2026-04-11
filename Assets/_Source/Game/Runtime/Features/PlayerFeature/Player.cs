using Game.Runtime.Configs;
using Game.Runtime.PianoRhythmSystem;
using UnityEngine;

namespace Game.Runtime.PlayerFeature
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public PlayerConfig Config { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Transform CameraPoint { get; private set; }
    }
}