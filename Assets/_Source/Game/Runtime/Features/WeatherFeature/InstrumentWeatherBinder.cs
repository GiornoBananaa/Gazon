using Game.Runtime.Configs;
using Game.Runtime.PianoFeature;
using Game.Runtime.WeatherSystem.WeatherTween;

namespace Game.Runtime.WeatherFeature
{
    public abstract class InstrumentWeatherBinder : IInstrumentStatisticBinder
    {
        protected readonly InstrumentPlayStatistics PianoStatistics;
        protected readonly WeatherPianoBindConfig WeatherPianoBindConfig;
        private readonly WeatherTweener _weatherTweener;
        
        protected InstrumentWeatherBinder(InstrumentPlayStatistics statistics, WeatherTweener weatherTweener, WeatherPianoBindConfig weatherPianoBindConfig)
        {
            PianoStatistics = statistics;
            WeatherPianoBindConfig = weatherPianoBindConfig;
            _weatherTweener = weatherTweener;
        }

        public virtual void OnInstrumentStop()
        {
            
        }
        
        protected void ApplyTweener(WeatherTween weatherTween)
        {
            _weatherTweener.Apply(weatherTween);
        }
    }
}