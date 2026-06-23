using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEngine;
using UnityEngine.VFX;

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
        
        [Header("Snow")]
        public VisualEffectAsset SnowEffect;
        public float SnowMaxDensity = 10000;
        public float SnowFallSpeed = 0.03f;
        public float SnowWindSpeedBlend = 0.2f;
        
        [Header("Rain")]
        public VisualEffectAsset RainEffect;
        public float RainMaxDensity = 10000;
        public float RainFallSpeed = 0.2f;
        public float RainWindSpeedBlend = 0.2f;
    }
}