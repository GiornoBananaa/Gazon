using Game.Runtime.PianoFeature;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.Installers
{
    public class MusicInstrumentContainerInjector : SelfSubContainerInjector
    {
        [SerializeField] private MusicInstrument _musicInstrument;
        [SerializeField] private PianoModeMenu _pianoModeMenu;

        protected override void BuilderExtend(ContainerBuilder builder)
        {
            builder.RegisterValue(_pianoModeMenu, new []{typeof(IInstrumentMenu)});
            builder.RegisterValue(_musicInstrument);
        }
    }
}