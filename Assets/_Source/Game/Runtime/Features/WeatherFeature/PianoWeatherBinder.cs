using Game.Runtime.Configs;
using Game.Runtime.PianoFeature;
using Game.Runtime.WeatherSystem.WeatherTween;

namespace Game.Runtime.WeatherFeature
{
    public abstract class PianoWeatherBinder
    {
        protected readonly PianoKeyPressStatistics PianoStatistics;
        protected readonly WeatherPianoBindConfig WeatherPianoBindConfig;
        private readonly WeatherTweener _weatherTweener;
        
        protected PianoWeatherBinder(PianoKeyPressStatistics statistics, WeatherTweener weatherTweener, WeatherPianoBindConfig weatherPianoBindConfig)
        {
            PianoStatistics = statistics;
            WeatherPianoBindConfig = weatherPianoBindConfig;
            _weatherTweener = weatherTweener;
        }

        protected virtual void OnPianoStop()
        {
            
        }
        
        protected void ApplyTweener(WeatherTween weatherTween)
        {
            _weatherTweener.Apply(weatherTween);
        }
    }
}