using System.Collections.Generic;
using Game.Runtime.InputFeature;

namespace Game.Runtime.PianoFeature
{
    public class FreeInstrumentState : IInstrumentState
    {
        private readonly FreeInstrumentInputListener _inputListener;
        private readonly InstrumentPlayStatistics _instrumentPlayStatistics;
        private readonly IEnumerable<IInstrumentKeyPresser> _instrumentKeyPressers;
        private readonly IInstrumentMenu _instrumentMenu;
        
        public FreeInstrumentState(FreeInstrumentInputListener inputListener, InstrumentPlayStatistics instrumentPlayStatistics,
            IEnumerable<IInstrumentKeyPresser> instrumentKeyPressers, IInstrumentMenu instrumentMenu)
        {
            _inputListener  = inputListener;
            _instrumentPlayStatistics  = instrumentPlayStatistics;
            _instrumentKeyPressers  = instrumentKeyPressers;
            _instrumentMenu = instrumentMenu;
        }
        
        public void Enter()
        {
            _instrumentPlayStatistics.SetPianoComponents(_instrumentKeyPressers, null);
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