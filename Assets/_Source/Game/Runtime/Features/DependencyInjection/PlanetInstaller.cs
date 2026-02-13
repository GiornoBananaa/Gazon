using Game.Runtime.ServiceSystem;
using Game.Runtime.TerrainChunkSystem;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.DependencyInjection
{
    public class PlanetInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(PlanetGenerator), typeof(IPlanetGenerator));
            builder.AddSingleton(typeof(PlanetMover), typeof(PlanetMover), typeof(IUpdatable));
            builder.AddSingleton(typeof(PlanetMap), typeof(PlanetMap), typeof(IUpdatable));
            builder.AddSingleton(typeof(TerrainBiomeBlender));
        }
    }
}