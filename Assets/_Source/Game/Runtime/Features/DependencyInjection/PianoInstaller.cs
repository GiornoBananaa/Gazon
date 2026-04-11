using Game.Runtime.Configs;
using Game.Runtime.InputFeature;
using Game.Runtime.PianoRhythmSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.DependencyInjection
{
    public class PianoInstaller: MonoBehaviour, IInstaller
    {
        [SerializeField] private PianoKeysConfig _pianoKeysConfig;
        
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_pianoKeysConfig);
            builder.RegisterType(typeof(BaseStateMachine<IPianoState>), new[] { typeof(IStateMachine<IPianoState>) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(PianoKeysPressPresenter), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(PianoInputListener), new[] { typeof(IInputListener), typeof(PianoInputListener) }, Lifetime.Singleton, Resolution.Eager);
            builder.RegisterType(typeof(FreePianoKeyPresser), new[] { typeof(IPianoKeyPresser), typeof(FreePianoKeyPresser) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(RhythmGamePianoKeyPresser), new[] { typeof(IPianoKeyPresser), typeof(RhythmGamePianoKeyPresser) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(PianoNoteTweener), new[] { typeof(IPianoNoteTweener) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}