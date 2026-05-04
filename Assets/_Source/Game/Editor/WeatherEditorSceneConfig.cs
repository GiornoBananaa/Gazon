using Game.Runtime.WeatherSystem.WeatherTween;
using Game.Runtime.WeatherSystem.WeatherTween.Tweeners;
using UnityEngine;

namespace Game.Editor
{
    [CreateAssetMenu(fileName = "WeatherEditorScene", menuName = "Editor/Weather/WeatherEditorScene")]
    public class WeatherEditorSceneConfig : ScriptableObject
    {
        [Space()]
        [Header("Test")]
        public WeatherTween WeatherTween;

        private WeatherTweener _weatherTweener;
        
        public void PlayTween()
        {
            if (_weatherTweener == null)
                _weatherTweener = new WeatherTweener( new WeatherEntityTweener[]{ new WindTweener() });
            _weatherTweener.Apply(WeatherTween);
        }

        public void StopTween()
        {
            _weatherTweener.Stop(WeatherTween.Type.Entity);
        }
    }
}