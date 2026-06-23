using System;
using Game.Runtime.Configs;
using Game.Runtime.WeatherSystem.WeatherTween;
using Game.Runtime.WeatherSystem.WeatherTween.WeatherEntityTweeners;
using UnityEngine;

namespace Game.Editor
{
    [CreateAssetMenu(fileName = "WeatherEditorScene", menuName = "Editor/Weather/WeatherEditorScene")]
    public class WeatherEditorSceneConfig : ScriptableObject, IDisposable
    {
        [Space()]
        [Header("Test")]
        public WeatherTween WeatherTween;

        private WeatherTweener _weatherTweener;
        private WindPositionMover _windPositionMover;
        private WeatherPianoBindConfig _weatherPianoBindConfig;

        private void OnEnable()
        {
            _weatherPianoBindConfig = CreateInstance<WeatherPianoBindConfig>();
        }

        public void PlayTween()
        {
            if (_weatherTweener == null)
            {
                _windPositionMover = new WindPositionMover();
                _weatherTweener = new WeatherTweener(new WeatherEntityTweener[] { new WindTweener(_windPositionMover, _weatherPianoBindConfig) });
            }
            _weatherTweener.Apply(WeatherTween);
        }

        public void StopTween()
        {
            _weatherTweener.Stop(WeatherTween.Type.Entity);
        }

        public void Dispose()
        {
            _weatherTweener.Dispose();
            if(_weatherPianoBindConfig)
                Destroy(_weatherPianoBindConfig);
        }
    }
}