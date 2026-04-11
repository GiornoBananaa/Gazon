using Game.Runtime.StateMachineSystem;

namespace Game.Runtime.PianoRhythmSystem
{
    public class PianoKeysPressPresenter
    {
        private readonly IStateMachine<IPianoState> _pianoStateMachine;

        public PianoKeysPressPresenter(IStateMachine<IPianoState> pianoStateMachine)
        {
            _pianoStateMachine = pianoStateMachine;
        }
        
        public void PressKey(int index)
        {
            if(!_pianoStateMachine.TryGetCurrentState(out var pianoState)) return;
            pianoState.PressKey(index);
        }
        
        public void ReleaseKey(int index)
        {
            if(!_pianoStateMachine.TryGetCurrentState(out var pianoState)) return;
            pianoState.ReleaseKey(index);
        }
        
        public void OctaveUp()
        {
            if(!_pianoStateMachine.TryGetCurrentState(out var pianoState)) return;
            pianoState.OctaveUp();
        }
        
        public void OctaveDown()
        {
            if(!_pianoStateMachine.TryGetCurrentState(out var pianoState)) return;
            pianoState.OctaveDown();
        }
        
        public void PressPedal()
        {
            if(!_pianoStateMachine.TryGetCurrentState(out var pianoState)) return;
            pianoState.PressPedal();
        }
        
        public void ReleasePedal()
        {
            if(!_pianoStateMachine.TryGetCurrentState(out var pianoState)) return;
            pianoState.ReleasePedal();
        }
    }
}