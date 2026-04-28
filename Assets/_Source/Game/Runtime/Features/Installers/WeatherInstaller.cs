using Game.Runtime.ServiceSystem;
using Game.Runtime.WeatherFeature;
using Game.Runtime.WeatherSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using Game.Runtime.WeatherSystem.WeatherTween.Tweeners;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.Installers
{
    public class WeatherInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(WindTweener), new[] { typeof(WeatherEntityTweener) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(WeatherTweener), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(WeatherPropertyBlender), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(WeatherBiomeSetter),  new[] { typeof(WeatherBiomeSetter), typeof(IUpdatable) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}