using Game.Runtime.BoidsFeature;
using Game.Runtime.BoidsSystem;
using Game.Runtime.Configs;
using Game.Runtime.PianoFeature;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.Installers
{
    public class BoidsInstaller: MonoBehaviour, IInstaller
    {
        [SerializeField] private BoidsPianoBindConfig _boidsPianoBindConfig;
        [SerializeField] private BoidsGravityPoints _boidsGravityPoints;
        
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_boidsPianoBindConfig);
            builder.RegisterValue(_boidsGravityPoints);
            builder.RegisterType(typeof(BirdsPianoBinder),  new[] { typeof(IPianoStatisticBinder) }, Lifetime.Singleton, Resolution.Eager);
        }
    }
}