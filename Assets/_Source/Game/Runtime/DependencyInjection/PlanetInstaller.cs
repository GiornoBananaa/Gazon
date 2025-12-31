using Game.Runtime.Planet.Generation;
using Game.Runtime.Planet.Movement;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.DependencyInjection
{
    public class PlanetInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(PlanetGenerator), typeof(IPlanetGenerator));
            builder.AddSingleton(typeof(PlanetMover));
            builder.AddSingleton(typeof(PlanetMap));
        }
    }
}