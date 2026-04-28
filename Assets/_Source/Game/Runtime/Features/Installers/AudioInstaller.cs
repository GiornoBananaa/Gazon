using Game.Runtime.AudioSystem;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.Installers
{
    public class AudioInstaller: MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(AudioPlayer), new[] { typeof(IAudioPlayer) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}