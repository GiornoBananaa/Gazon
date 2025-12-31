using Game.Runtime.Planet.Movement;
using R3;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.Weather.WeatherTween
{
    public class WeatherController : MonoBehaviour
    {
        [Inject] private WeatherBiomeSetter _weatherBiomeSetter;
        [Inject] private PlanetMap _planetMap;

        private void Awake()
        {
            _planetMap.CurrentBiome.Skip(1).Subscribe(_weatherBiomeSetter.SetBiomeWeather).AddTo(this);
        }
    }
}