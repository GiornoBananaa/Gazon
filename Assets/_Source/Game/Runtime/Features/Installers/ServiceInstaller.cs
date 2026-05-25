using Game.Runtime.ServiceSystem;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.Installers
{
    public class ServiceInstaller: MonoBehaviour, IInstaller
    {
        [SerializeField] private ServiceUpdater _serviceUpdater;
        
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_serviceUpdater);
        }
    }
}