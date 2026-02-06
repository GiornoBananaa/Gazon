using Game.Runtime.WeatherSystem.WeatherTween;
using PrimeTween;
using UnityEngine;

namespace Game.Editor
{
    [CreateAssetMenu(fileName = "WeatherEditorScene", menuName = "Editor/Weather/WeatherEditorScene")]
    public class WeatherEditorSceneConfig : ScriptableObject
    {
        public Vector2 WindDirection;
        public float WindForce;
        [Space(10)]
        public Vector2 SunDirection;
        public Color SunLightColor;
        public Vector2 MoonDirection;
        [Space(10)]
        public Color EnvironmentLightColor;
        [Space(10)]
        public Material GrassMaterial;
        public Material SkyMaterial;
        
        [Space()]
        [Header("Test")]
        public WeatherTween WeatherTween;

        private WeatherTweener _weatherTweener;
        private Tween _tween;
        
        public void PlayTween()
        {
            if (_weatherTweener == null)
                _weatherTweener = new WeatherTweener();
            _weatherTweener.Apply(WeatherTween);
        }

        public void StopTween()
        {
            _tween.Stop();
        }
    }
}