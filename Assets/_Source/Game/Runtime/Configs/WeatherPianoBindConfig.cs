using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "WeatherPianoConfig", menuName = "Game/Weather/WeatherPianoConfig")]
    public class WeatherPianoBindConfig : ScriptableObject
    {
        [Header("Wind")]
        public TweenConfig WindDirectionTweenConfig;
        public Vector3 MinDirection;
        public Vector3 MaxDirection;
        public TweenConfig WindForceTweenConfig;
        public int NotesCountForMaxWindForce;
        public float MinBendForce;
        public float MaxBendForce;
        public float MinSpeed = 6;
        public float MaxSpeed = 10;
    }
}