using Game.Runtime.CameraFeature;
using Game.Runtime.CameraSystem;
using Game.Runtime.InputFeature;
using Game.Runtime.ObjectCullingSystem;
using Game.Runtime.ServiceSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.DependencyInjection
{
    public class CameraInstaller: MonoBehaviour, IInstaller
    {
        [SerializeField] private CameraHandle _cameraHandle;
        
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_cameraHandle);
            builder.RegisterType(typeof(BaseStateMachine<ICameraState>), new[] { typeof(IStateMachine<ICameraState>)}, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(CameraInputListener), new[] { typeof(IInputListener) }, Lifetime.Singleton, Resolution.Eager);
            builder.RegisterType(typeof(CurrentCamera), new[] { typeof(ICurrentCamera) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(ObjectCuller), new[] { typeof(ObjectCuller), typeof(IUpdatable) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(CameraInputRotator), new[] { typeof(ICameraRotator), typeof(CameraInputRotator) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(CameraFollowTargetMover), new[] { typeof(ILateUpdatable), typeof(CameraFollowTargetMover) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}