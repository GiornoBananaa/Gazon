using Game.Runtime.Configs;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.PianoFeature;
using Reflex.Core;
using UnityEngine;

namespace Game.Runtime.Installers
{
    public class MusicInstrumentContainerInjector : SelfSubContainerInjector
    {
        [SerializeField] private InstrumentKeysConfig _instrumentConfig;
        [SerializeField] private MusicInstrument _musicInstrument;
        [SerializeField] private PianoModeMenu _pianoModeMenu;

        protected override void BuilderExtend(ContainerBuilder builder)
        {
            if(_pianoModeMenu != null)
                builder.RegisterValue(_pianoModeMenu, new []{typeof(IInstrumentMenu)});
            builder.RegisterValue(_musicInstrument, new []{typeof(IInstrument), typeof(MusicInstrument)});
            builder.RegisterValue(_instrumentConfig);
        }
    }
}