using Game.Runtime.ServiceSystem;
using Game.Runtime.TerrainChunkSystem;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.Installers
{
    public class PlanetInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(PlanetGenerator), new[] { typeof(IPlanetGenerator) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(PlanetMover), new[] { typeof(PlanetMover), typeof(IUpdatable) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(PlanetMap), new[] { typeof(PlanetMap), typeof(IUpdatable) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(TerrainBiomeBlender), Lifetime.Singleton, Resolution.Lazy);
        }
    }
}