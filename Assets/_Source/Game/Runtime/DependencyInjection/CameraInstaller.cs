using Game.Runtime.CameraFeature;
using Game.Runtime.ObjectCulling;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.DependencyInjection
{
    public class CameraInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(CurrentCamera), typeof(ICurrentCamera));
            builder.AddSingleton(typeof(ObjectCuller), typeof(ObjectCuller), typeof(IUpdatable));
        }
    }
}