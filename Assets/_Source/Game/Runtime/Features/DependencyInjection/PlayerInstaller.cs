using Game.Runtime.InputFeature;
using Game.Runtime.PlayerFeature;
using Game.Runtime.PlayerInteractionSystem;
using Game.Runtime.PlayerMovementSystem;
using Game.Runtime.ServiceSystem;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.DependencyInjection
{
    public class PlayerInstaller: MonoBehaviour, IInstaller
    {
        [SerializeField] private Player _player;
        
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_player);
            builder.RegisterValue(_player.Config);
            builder.RegisterType(typeof(PlayerInputListener), new[] { typeof(IInputListener), typeof(PlayerInputListener) }, Lifetime.Singleton, Resolution.Eager);
            builder.RegisterType(typeof(RigidbodyPlayerMovement), new[] { typeof(IPlayerMovement), typeof(IFixedUpdatable) }, Lifetime.Singleton, Resolution.Eager);
            builder.RegisterType(typeof(PlayerRaycastInteraction), new[] { typeof(IPlayerInteraction) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}