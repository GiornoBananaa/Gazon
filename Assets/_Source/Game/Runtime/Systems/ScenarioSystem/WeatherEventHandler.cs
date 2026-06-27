using Game.Runtime.WeatherSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEngine;

namespace Game.Runtime.ScenarioSystem
{
    public class WeatherEventHandler : IScenarioEventHandler, IWeatherPropertySetter
    {
        private readonly WeatherPropertyBlender _weatherPropertyBlender;
        
        public EventType EventType => EventType.WeatherProperty;

        public WeatherEventHandler(WeatherPropertyBlender weatherPropertyBlender)
        {
            _weatherPropertyBlender = weatherPropertyBlender;
        }
        
        public void StartEvent(int eventIndex, string value)
        {
            WeatherProperty weatherProperty;
            try
            {
                weatherProperty = JsonUtility.FromJson<WeatherProperty>(value);
            }
            catch
            {
                return;
            }
            
            _weatherPropertyBlender.ApplyProperty(this, weatherProperty, 1);
        }

        public void EndEvent(int eventIndex, string value)
        {
            WeatherProperty weatherProperty;
            try
            {
                weatherProperty = JsonUtility.FromJson<WeatherProperty>(value);
            }
            catch
            {
                return;
            }
            _weatherPropertyBlender.RemoveProperty(this, weatherProperty.Type);
        }
    }
}