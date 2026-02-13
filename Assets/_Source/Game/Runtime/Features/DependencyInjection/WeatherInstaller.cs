using Game.Runtime.ServiceSystem;
using Game.Runtime.WeatherFeature;
using Game.Runtime.WeatherSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.DependencyInjection
{
    public class WeatherInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(WeatherTweener));
            builder.AddSingleton(typeof(WeatherPropertyBlender));
            builder.AddSingleton(typeof(WeatherBiomeSetter), typeof(WeatherBiomeSetter), typeof(IUpdatable));
        }
    }
}