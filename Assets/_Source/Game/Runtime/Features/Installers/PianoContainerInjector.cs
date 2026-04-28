using Game.Runtime.PianoFeature;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.Installers
{
    public class PianoContainerInjector : SelfSubContainerInjector
    {
        [SerializeField] private Piano _piano;

        protected override void BuilderExtend(ContainerBuilder builder)
        {
            builder.RegisterValue(_piano);
        }
    }
}