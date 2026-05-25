using Game.Runtime.Configs;
using Game.Runtime.PianoFeature;
using Game.Runtime.WeatherSystem.WeatherTween;
using R3;
using UnityEngine;

namespace Game.Runtime.WeatherFeature
{
    public class WindInstrumentBinder : InstrumentWeatherBinder
    {
        private readonly WeatherTween _windForceTween;
        private readonly WeatherTween _windDirectionTween;
        
        public WindInstrumentBinder(InstrumentPlayStatistics statistics, WeatherTweener weatherTweener, WeatherPianoBindConfig weatherPianoBindConfig) : base(statistics, weatherTweener, weatherPianoBindConfig)
        {
            _windForceTween = new WeatherTween{ Type = new WeatherPropertyType(WeatherEntityType.Wind, WeatherParameterType.Force), TweenConfig = weatherPianoBindConfig.WindForceTweenConfig };
            _windDirectionTween = new WeatherTween{ Type = new WeatherPropertyType(WeatherEntityType.Wind, WeatherParameterType.Direction), TweenConfig = weatherPianoBindConfig.WindDirectionTweenConfig };
            statistics.NotePressCountOverTime.Skip(1).Subscribe(OnNotePressCountOverTime);
            statistics.AverageToneOverTime.Skip(1).Subscribe(OnAverageToneOverTime);
        }

        private void OnNotePressCountOverTime(int notePressCountOverTime)
        {
            _windForceTween.FloatValue = Mathf.Lerp(WeatherPianoBindConfig.MinBendForce, WeatherPianoBindConfig.MaxBendForce, (float)notePressCountOverTime / WeatherPianoBindConfig.NotesCountForMaxWindForce);
            ApplyTweener(_windForceTween);
        }
        
        private void OnAverageToneOverTime(float tone)
        {
            _windDirectionTween.VectorValue = Quaternion.Lerp(Quaternion.Euler(WeatherPianoBindConfig.MinDirection), 
                Quaternion.Euler(WeatherPianoBindConfig.MaxDirection), tone).eulerAngles;
            ApplyTweener(_windDirectionTween);
        }

        public override void OnInstrumentStop()
        {
            
        }
    }
}