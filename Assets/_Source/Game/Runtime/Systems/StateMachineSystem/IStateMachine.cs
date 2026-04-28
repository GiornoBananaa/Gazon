namespace Game.Runtime.StateMachineSystem
{
    public interface IStateMachine<T>
    {
        bool TryGetCurrentState(out T currentState);
        void ChangeState(T state);
    }
}