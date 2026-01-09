using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.Weather.WeatherTween
{
    public class WeatherController : MonoBehaviour
    {
        [Inject] private WeatherBiomeSetter _weatherBiomeSetter;

        private void Update()
        {
            _weatherBiomeSetter.UpdateBiomeWeather();
        }
    }
}