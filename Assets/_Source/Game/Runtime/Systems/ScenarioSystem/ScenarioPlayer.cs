using System;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime.MusicInstrumentSystem;

namespace Game.Runtime.ScenarioSystem
{
    public class ScenarioPlayer : IDisposable
    {
        private readonly List<int> _startedEvents = new();
        private readonly IScenarioEventHandler[] _eventHandlers;
        private ITimer _timer;
        private Scenario _scenario;
        private int _currentEventIndex;

        public ScenarioPlayer(IEnumerable<IScenarioEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers.ToArray();
        }
        
        public void PlayScenario(Scenario scenario, ITimer timer)
        {
            _startedEvents.Clear();
            _scenario = scenario;
            _currentEventIndex = 0;
            _timer = timer;
            _timer.OnTimeChanged += OnTimeChanged;
            _timer.OnCompleted += OnCompleted;
        }

        private void OnTimeChanged(float time)
        {
            for (int i = 0; i < _startedEvents.Count; i++)
            {
                var started = _startedEvents[i];
                if (_scenario.Events[started].EndTime <= time)
                {
                    EndEvent(started);
                    i--;
                }
            }
            
            if(_currentEventIndex >= _scenario.Events.Length) return;
            
            if (_scenario.Events[_currentEventIndex].StartTime <= time)
            {
                StartEvent(_currentEventIndex);
                _currentEventIndex++;
            }
        }

        private void StartEvent(int eventIndex)
        {
            _startedEvents.Add(eventIndex);
            foreach (var eventHandler in _eventHandlers)
            {
                if(eventHandler.EventType == _scenario.Events[eventIndex].Type)
                    eventHandler.StartEvent(eventIndex, _scenario.Events[eventIndex].Value);
            }
        }
        
        private void EndEvent(int eventIndex)
        {
            _startedEvents.Remove(eventIndex);
            foreach (var eventHandler in _eventHandlers)
            {
                if(eventHandler.EventType == _scenario.Events[eventIndex].Type)
                    eventHandler.EndEvent(eventIndex, _scenario.Events[eventIndex].Value);
            }
        }
        
        private void OnCompleted()
        {
            for (int i = 0; i < _startedEvents.Count; i++)
            {
                var started = _startedEvents[i];
                if (!_scenario.Events[started].StayAfterEnd)
                {
                    EndEvent(started);
                    i--;
                }
            }
            
            _timer.OnTimeChanged -= OnTimeChanged;
            _timer.OnCompleted -= OnCompleted;
        }

        public void Dispose()
        {
            if(_timer != null)
            {
                _timer.OnTimeChanged -= OnTimeChanged;
                _timer.OnCompleted -= OnCompleted;
            }
        }
    }
}