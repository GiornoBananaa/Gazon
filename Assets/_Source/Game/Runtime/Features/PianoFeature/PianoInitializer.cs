using Game.Runtime.InputFeature;
using Game.Runtime.PianoRhythmSystem;
using Game.Runtime.StateMachineSystem;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PianoFeature
{
    public class PianoInitializer : MonoBehaviour
    {
        private IStateMachine<IPianoState> _pianoStateMachine;
        private FreePianoState _pianoState;
        
        [Inject]
        public void Construct(PianoInputListener pianoInputListener, IStateMachine<IPianoState> pianoStateMachine, FreePianoState freePianoState)
        {
            _pianoStateMachine = pianoStateMachine;
            _pianoState = freePianoState;
        }

        private void Awake()
        {
            _pianoStateMachine.ChangeState(_pianoState);
        }
    }
}