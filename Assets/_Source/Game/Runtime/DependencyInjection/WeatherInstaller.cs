using System;
using Game.Runtime.Weather.WeatherTween;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.DependencyInjection
{
    public class WeatherInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(WeatherTweener));
            builder.AddSingleton(typeof(WeatherBiomeSetter));
        }
    }
}