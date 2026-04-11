using Game.Runtime.InputFeature;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.DependencyInjection
{
    public class ProjectInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(GameInputActions), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(InputManager), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(GeneralNavigationInputListener), new[] { typeof(IInputListener), typeof(GeneralNavigationInputListener) }, Lifetime.Singleton, Resolution.Eager);
        }
    }
}