using System.Collections.Generic;
using Game.Runtime.Configs;
using Game.Runtime.InputFeature;

namespace Game.Runtime.PianoFeature
{
    public class FreeInstrumentState : IInstrumentState
    {
        private readonly FreeInstrumentInputListener _inputListener;
        private readonly InstrumentPlayStatistics _instrumentPlayStatistics;
        private readonly IEnumerable<IInstrumentKeyPresser> _instrumentKeyPressers;
        private readonly IInstrumentMenu _instrumentMenu;
        private readonly InstrumentKeysConfig _keysConfig;
        
        public FreeInstrumentState(FreeInstrumentInputListener inputListener, InstrumentPlayStatistics instrumentPlayStatistics,
            IEnumerable<IInstrumentKeyPresser> instrumentKeyPressers, IInstrumentMenu instrumentMenu, InstrumentKeysConfig keysConfig)
        {
            _inputListener  = inputListener;
            _instrumentPlayStatistics  = instrumentPlayStatistics;
            _instrumentKeyPressers  = instrumentKeyPressers;
            _instrumentMenu = instrumentMenu;
            _keysConfig = keysConfig;
        }
        
        public void Enter()
        {
            _instrumentPlayStatistics.SetPianoComponents(_keysConfig.Notes.Length, _instrumentKeyPressers, null);
            _inputListener.EnableInput();
            _instrumentMenu.Show();
        }

        public void Exit()
        {
            _inputListener.DisableInput();
            _instrumentMenu.Hide();
        }
    }
}