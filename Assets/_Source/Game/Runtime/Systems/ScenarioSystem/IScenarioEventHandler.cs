namespace Game.Runtime.ScenarioSystem
{
    public interface IScenarioEventHandler
    {
        EventType EventType { get; }
        void StartEvent(int eventIndex, string value);
        void EndEvent(int eventIndex, string value);
    }
}