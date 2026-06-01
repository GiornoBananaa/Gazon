using Game.Runtime.InputFeature;

namespace Game.Runtime.PianoFeature
{
    public class FreeInstrumentState : IInstrumentState
    {
        private readonly FreeInstrumentInputListener _inputListener;
        private readonly InstrumentPlayStatistics _instrumentPlayStatistics;
        private readonly FreeInstrumentKeyPresser _instrumentKeyPresser;
        private readonly IInstrumentMenu _instrumentMenu;
        
        public FreeInstrumentState(FreeInstrumentInputListener inputListener, InstrumentPlayStatistics instrumentPlayStatistics,
            FreeInstrumentKeyPresser instrumentKeyPresser, IInstrumentMenu instrumentMenu)
        {
            _inputListener  = inputListener;
            _instrumentPlayStatistics  = instrumentPlayStatistics;
            _instrumentKeyPresser  = instrumentKeyPresser;
            _instrumentMenu = instrumentMenu;
        }
        
        public void Enter()
        {
            _instrumentPlayStatistics.SetPianoComponents(_instrumentKeyPresser, null);
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