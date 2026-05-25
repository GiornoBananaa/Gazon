using Game.Runtime.InputFeature;

namespace Game.Runtime.PianoFeature
{
    public class FreeInstrumentState : IPianoState
    {
        private readonly FreeInstrumentInputListener _inputListener;
        private readonly InstrumentPlayStatistics _instrumentPlayStatistics;
        private readonly FreeInstrumentKeyPresser _instrumentKeyPresser;
        
        public FreeInstrumentState(FreeInstrumentInputListener inputListener, InstrumentPlayStatistics instrumentPlayStatistics, FreeInstrumentKeyPresser instrumentKeyPresser)
        {
            _inputListener  = inputListener;
            _instrumentPlayStatistics  = instrumentPlayStatistics;
            _instrumentKeyPresser  = instrumentKeyPresser;
        }
        
        public void Enter()
        {
            _instrumentPlayStatistics.SetPianoComponents(_instrumentKeyPresser, null);
            _inputListener.EnableInput();
        }

        public void Exit()
        {
            _inputListener.DisableInput();
        }
    }
}