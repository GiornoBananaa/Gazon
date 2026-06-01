using System;
using Game.Runtime.BoidsFeature;
using Game.Runtime.Configs;
using Game.Runtime.InputFeature;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.PianoFeature;
using Game.Runtime.RhythmSystem;
using Game.Runtime.ServiceSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Runtime.Installers
{
    public class PianoInstaller: MonoBehaviour, IInstaller
    {
        [SerializeField] private PianoKeysConfig _pianoKeysConfig;
        [SerializeField] private WeatherPianoBindConfig _weatherPianoBindConfig;
        
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_pianoKeysConfig);
            builder.RegisterValue(_weatherPianoBindConfig);
            
            builder.RegisterType(typeof(MusicLibrary), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(MidiFileReader), new[] { typeof(IMusicFileReader) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(NotesPlayer), new[] { typeof(NotesPlayer) }, Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(InstrumentNoteTweener), new[] { typeof(IInstrumentNoteTweener) }, Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(Orchestra), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(BaseStateMachine<IInstrumentState>), new[] { typeof(IStateMachine<IInstrumentState>) }, Lifetime.Scoped, Resolution.Lazy);
            
            //Free mode
            builder.RegisterType(typeof(FreeInstrumentState), Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(FreeInstrumentInputListener), new[] { typeof(IInputListener), typeof(FreeInstrumentInputListener) }, Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(FreeInstrumentKeyPresser), new[] { typeof(IInstrumentKeyPresser), typeof(FreeInstrumentKeyPresser) }, Lifetime.Scoped, Resolution.Lazy);
            
            //Rhythm mode
            builder.RegisterType(typeof(RhythmInstrumentInputListener), new[] { typeof(IInputListener), typeof(RhythmInstrumentInputListener) }, Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(RhythmGameInstrumentKeyPresser), new[] { typeof(IInstrumentKeyPresser), typeof(RhythmGameInstrumentKeyPresser) }, Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(RhythmGameController), Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(RhythmInstrumentState), Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(RhythmKeyGenerator), new[] { typeof(IRhythmKeyGenerator) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(RhythmSheet), new[] { typeof(IRhythmSheet) }, Lifetime.Scoped, Resolution.Lazy);
            builder.RegisterType(typeof(RhythmGameSettings), Lifetime.Singleton, Resolution.Lazy);
            
            //Music magic
            builder.RegisterType(typeof(InstrumentPlayStatistics),  new[] { typeof(InstrumentPlayStatistics), typeof(IUpdatable) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(BirdsInstrumentBinder),  new[] { typeof(IInstrumentStatisticBinder) }, Lifetime.Singleton, Resolution.Eager);
        }
    }
}