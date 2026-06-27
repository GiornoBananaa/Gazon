using System;

namespace Game.Runtime.ScenarioSystem
{
    [Serializable]
    public struct ScenarioEvent : IEquatable<ScenarioEvent>
    {
        public float StartTime;
        public float EndTime;
        public bool StayAfterEnd;
        public EventType Type;
        public string Value;

        public ScenarioEvent(float startTime, float endTime, bool stayAfterEnd, EventType type, string value)
        {
            StartTime = startTime;
            EndTime = endTime;
            StayAfterEnd = stayAfterEnd;
            Type = type;
            Value = value;
        }

        public bool Equals(ScenarioEvent other)
        {
            return StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime) && StayAfterEnd == other.StayAfterEnd && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is ScenarioEvent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime, StayAfterEnd, (int)Type);
        }
    }

    [Serializable]
    public class Scenario
    {
        public ScenarioEvent[] Events;
    }
}