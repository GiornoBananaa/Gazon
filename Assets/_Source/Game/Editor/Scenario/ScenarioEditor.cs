using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Runtime.Configs;
using Game.Runtime.MusicInstrumentSystem;
using Game.Runtime.ScenarioSystem;
using Game.Runtime.ServiceSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using UnityEditor;
using UnityEngine;
using EventType = UnityEngine.EventType;

namespace Game.Editor.Scenario
{
    public class ScenarioEditor : EditorWindow
    {
        private bool opened;
        private MusicLibrary _library;
        private MusicTrack _musicTrack;
        private List<List<ScenarioEvent>> _events = new();
        private List<List<Rect>> _eventRects = new();
        private List<ScenarioEvent> _addedEvents = new();
        private Dictionary<Vector2Int, ScenarioEvent> _changedEvents = new();
        private string[] _trackOptions = new []{"none"};
        private int _trackIndex;
        private SerializedObject serializedObject;
        private List<InstrumentKeysConfig> instrumentConfigs = new();
        
        [MenuItem("Tools/ScenarioEditor")]
        public static void ShowWindow()
        {
            GetWindow<ScenarioEditor>("ScenarioEditor");
        }

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            _weatherPropertySerialized = serializedObject.FindProperty("_weatherProperty");
            _spawnEventDataSerialized = serializedObject.FindProperty("_spawnEventData");

            string filter = $"t:{nameof(InstrumentKeysConfig)}";
            string[] guids = AssetDatabase.FindAssets(filter, new [] { "Assets/_Presentation/Configs" });
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                InstrumentKeysConfig asset = AssetDatabase.LoadAssetAtPath<InstrumentKeysConfig>(assetPath); 
                instrumentConfigs.Add(asset);
            }

            Open();
        }

        private void OnDisable()
        {
            if(_notesPlayer == null) return;
            foreach (var notesPlayer in _notesPlayer)
            {
                notesPlayer.Value.Stop();
            }
        }

        private void OnGUI()
        {
            serializedObject.Update();
            
            var lastIndex = _trackIndex;
            _trackIndex = EditorGUILayout.Popup(_trackIndex, _trackOptions);
            if(lastIndex != _trackIndex)
            {
                Open();
                currentTime = 0;
            }

            if (opened && _musicTrack != null)
            {
                DrawTimeLine();
                DrawAudioPanel();
                DrawEventControlPanel();
            }
        }

        private void Open()
        {
            if (_library == null)
            {
                _library = new MusicLibrary(new[] { new MidiFileReader() });
            }
            
            _library.Initialize();
            _trackOptions = _library.MusicTracks.Select(t => t.Name).ToArray();
            _musicTrack = _library.GetMusicTrack(_trackIndex);
            if(_musicTrack == null) return;
            _events = new List<List<ScenarioEvent>>();
            _addedEvents.Clear();
            _eventRects.Clear();
            currentTime = 0;
            if(_musicTrack.Scenario.Events != null)
            {
                var remainingEvents = _musicTrack.Scenario.Events.ToList();
                for (int row = 0; row < 50; row++)
                {
                    if (remainingEvents.Count == 0) break;
                    if (_events.Count <= row)
                        _events.Add(new List<ScenarioEvent>());
                    for (int i = 0; i < remainingEvents.Count; i++)
                    {
                        if (_events[row].Count == 0 || _events[row][^1].EndTime <= remainingEvents[i].StartTime)
                        {
                            _events[row].Add(remainingEvents[i]);
                            remainingEvents.Remove(remainingEvents[i]);
                            i--;
                        }
                    }
                }
            }

            if(_notesPlayer != null)
            {
                foreach (var notesPlayer in _notesPlayer)
                {
                    notesPlayer.Value.Stop();
                }
            }
            
            int index = 0;
            foreach (var pair in _musicTrack.Notes)
            {
                if (index >= instrumentConfigs.Count) break;
                _instrumentConfigs[pair.Key] = instrumentConfigs[index];
                index++;
            }
            
            serializedObject.Update();
            opened = true;
        }
        
        private float currentTime = 0f;
        private float maxTime = 1;
        private const float LeftPadding = 60f;
        private const float RightPadding = 20f;
        private const float TimelineHeight = 40f;
        private const float MaxTickWidth = 30f;
        private float widthPerSecond;
        private float startX;
        
        private void DrawTimeLine()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Timeline", EditorStyles.boldLabel);
            maxTime = _musicTrack.GetLength();

            // Define the workspace bounding rectangle
            Rect timelineRect = GUILayoutUtility.GetRect(10, position.width, TimelineHeight + _eventTimelineHeight * (_events.Count + 1), TimelineHeight + _eventTimelineHeight * (_events.Count + 1));
    
            // Draw the background frame
            EditorGUI.DrawRect(timelineRect, new Color(0.2f, 0.2f, 0.2f));

            // Calculate visual boundaries for ticking increments
            float timelineVisualWidth = timelineRect.width - LeftPadding - RightPadding;
            startX = timelineRect.x + LeftPadding;
            int maxDrawnTicks = (int)(timelineVisualWidth / MaxTickWidth);
            int secondsInTick = Mathf.CeilToInt(Mathf.Max(1, maxTime / maxDrawnTicks));
            widthPerSecond = timelineVisualWidth * (1 / maxTime);
            // Draw time increments (Ruler ticks)
            int totalTicks = (int)(maxTime / secondsInTick);
            for (int i = 0; i <= totalTicks; i++)
            {
                float progress = (float)(i* secondsInTick) / maxTime;
                float tickX = startX + (progress * timelineVisualWidth);
        
                // Render major time ticks vertically
                Rect tickRect = new Rect(tickX, timelineRect.y, 1f, timelineRect.height * 0.4f);
                EditorGUI.DrawRect(tickRect, Color.gray);
        
                // Render text string labels under the ticks
                Rect labelRect = new Rect(tickX - 10f, timelineRect.y + 18f, 30f, 20f);
                EditorGUI.LabelField(labelRect, $"{(i * secondsInTick)}", EditorStyles.miniLabel);
            }
            
            DrawEventTimeLine(timelineRect);
            
            // Draw the Playhead (Scrubber bar)
            float playheadProgress = currentTime / maxTime;
            float playheadX = startX + (playheadProgress * timelineVisualWidth);
            Rect playheadRect = new Rect(playheadX - 1f, timelineRect.y, 3f, timelineRect.height);
            EditorGUI.DrawRect(playheadRect, Color.red);

            // Process Mouse scrubbing functionality inside the box
            ProcessTimeLineUIEvents(timelineRect, startX, timelineVisualWidth);

            // Optional bottom feedback display
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time:", GUILayout.Width(50));
            currentTime = EditorGUILayout.FloatField(currentTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                Repaint(); // Forces the UI to update immediately
            }
        }
        
        private void ProcessTimeLineUIEvents(Rect timelineRect, float startX, float visualWidth)
        {
            Event currentEvent = Event.current;
            
            // Check if mouse interacts inside the timeline coordinates
            if (timelineRect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag)
                {
                    // Convert screen mouse position to percentage relative to time scale
                    float mouseRelativeX = currentEvent.mousePosition.x - startX;
                    float normalizedProgress = Mathf.Clamp01(mouseRelativeX / visualWidth);
                    
                    currentTime = normalizedProgress * maxTime;
                    
                    // Redraw UI step actively
                    currentEvent.Use();
                    Repaint();
                }
            }
        }


        #region Notes Player

        private Dictionary<InstrumentId, EditorNotesPlayer> _notesPlayer;
        private Dictionary<InstrumentId, InstrumentKeysConfig> _instrumentConfigs = new();

        private void Update()
        {
            if(_notesPlayer == null) return;
            foreach (var notesPlayer in _notesPlayer)
            {
                notesPlayer.Value.Update();
            }
        }

        private void DrawAudioPanel()
        {

            if (_notesPlayer != null)
            {
                foreach (var notesPlayer in _notesPlayer)
                {
                    if(notesPlayer.Value.IsPlaying)
                        currentTime = (float)notesPlayer.Value.Time;
                    break;
                }
            }
            
            foreach (var pair in _musicTrack.Notes)
            {
                _instrumentConfigs.TryAdd(pair.Key, null);
                _instrumentConfigs[pair.Key] = (InstrumentKeysConfig)EditorGUILayout.ObjectField(pair.Key.ToString(), _instrumentConfigs[pair.Key], typeof(InstrumentKeysConfig), false);
            }
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Play", GUILayout.Width(100)))
            {
                _notesPlayer = new Dictionary<InstrumentId, EditorNotesPlayer>();
                foreach (var pair in _musicTrack.Notes)
                {
                    if(_instrumentConfigs[pair.Key] == null) continue;
                    var notesPlayer = new EditorNotesPlayer(_instrumentConfigs[pair.Key]);
                    _notesPlayer.Add(pair.Key, notesPlayer);
                    notesPlayer.Play(pair.Value, currentTime);
                }
            }
            if (GUILayout.Button("Stop", GUILayout.Width(100)) && _notesPlayer != null)
            {
                foreach (var notesPlayer in _notesPlayer)
                {
                    notesPlayer.Value.Stop();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private class EditorNotesPlayer : IDisposable, IUpdatable
        {
            private Dictionary<int, NoteConfig> _sounds;
            private readonly List<Note> _remainingNotes = new();
            private double _startTime;
            public double Time => EditorApplication.timeSinceStartup - _startTime;
            public bool IsPlaying { get; private set; }
            private List<EditorAudioPlayer> _audioPlayers = new();
            private List<Note> _currentNotes = new();
            private Queue<EditorAudioPlayer> _audioPlayerPool = new();

            public EditorNotesPlayer(InstrumentKeysConfig config)
            {
                _sounds = config.Notes.ToDictionary(n => n.GetHashCode());
            }
            
            public void Play(Note[] notes, double startTime)
            {
                _startTime = EditorApplication.timeSinceStartup - startTime;
                _remainingNotes.Clear();
                _remainingNotes.AddRange(notes.Where(n => n.StartTime >= Time));
                IsPlaying = true;
            }
        
            public void Continue()
            {
                IsPlaying = true;
            }
        
            public void Stop()
            {
                IsPlaying = false;
                foreach (var audioPlayer in _audioPlayers)
                {
                    audioPlayer.Stop();
                }
            }
        
            public void Update()
            {
                if (!IsPlaying) return;

                for (int i = 0; i < _audioPlayers.Count; i++)
                {
                    if(Time < _currentNotes[i].EndTime) continue;
                    OnNoteEnded(_currentNotes[i]);
                    i--;
                }
                
                for (int i = 0; i < _remainingNotes.Count; i++)
                {
                    if(Time < _remainingNotes[i].StartTime) break;
                    OnNoteStarted(_remainingNotes[i]);
                    _remainingNotes.RemoveAt(i);
                }

                if (_remainingNotes.Count <= 0 && _currentNotes.Count <= 0)
                {
                    Stop();
                }
            }
        
            private void OnNoteStarted(Note note)
            {
                int hash = HashCode.Combine((int)note.NoteType, note.Octave);
                if (!_sounds.ContainsKey(hash))
                {
                    Debug.LogWarning($"Sound not found: { note.NoteType }{ note.Octave }");
                    return;
                }
                EditorAudioPlayer audioPlayer = null;
                if (_audioPlayerPool.Count > 0)
                {
                    audioPlayer = _audioPlayerPool.Dequeue();
                }
                else
                    audioPlayer = new EditorAudioPlayer();
                audioPlayer.Play(_sounds[hash].Sound.AudioClip);
                _audioPlayers.Add(audioPlayer);
                _currentNotes.Add(note);
            }
        
            private void OnNoteEnded(Note note)
            {
                int index = -1;
                for (int i = 0; i < _currentNotes.Count; i++)
                {
                    if(_currentNotes[i] == note)
                    {
                        index = i;
                        break;
                    }
                }
                if(index < 0) return;
                _audioPlayers[index].Stop();
                _audioPlayerPool.Enqueue(_audioPlayers[index]);
                _audioPlayers.RemoveAt(index);
                _currentNotes.RemoveAt(index);
            }

            public void Dispose()
            {
                Stop();
                foreach (var audioPlayer in _audioPlayerPool)
                {
                    audioPlayer.Dispose();
                }
                
                foreach (var audioPlayer in _audioPlayers)
                {
                    audioPlayer.Dispose();
                }
            }
        }

        #endregion
        
        #region Events

        private const float _eventTimelineHeight = 10f;
        private float _startTime;
        private float _endTime;
        private bool _stayAfterEnd;
        private Game.Runtime.ScenarioSystem.EventType _eventType;
        private SerializedProperty _weatherPropertySerialized;
        private SerializedProperty _spawnEventDataSerialized;
        public WeatherPropertyWrapper _weatherProperty = new(default);
        public SpawnEventData _spawnEventData = new();

        [Serializable]
        public class WeatherPropertyWrapper
        {
            public WeatherProperty WeatherProperty;

            public WeatherPropertyWrapper(WeatherProperty weatherProperty)
            {
                WeatherProperty = weatherProperty;
            }
        }
        
        private void DrawEventControlPanel()
        {
            if(!_selectedEvent)
            {
                if (GUILayout.Button("Add New Event", GUILayout.Width(120)))
                {
                    ScenarioEvent scenarioEvent = new ScenarioEvent(currentTime, currentTime + 5, false, Runtime.ScenarioSystem.EventType.WeatherProperty, null);
                    _addedEvents.Add(scenarioEvent);
                    _selectedEvent = true;
                    _currentEventEdit = new Vector2Int(_events.Count, 0);
                    _startTime = scenarioEvent.StartTime;
                    _endTime = scenarioEvent.EndTime;
                    _stayAfterEnd = scenarioEvent.StayAfterEnd;
                    _eventType = scenarioEvent.Type;
                    _weatherProperty = new WeatherPropertyWrapper(default);
                    _spawnEventData = new SpawnEventData();
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    return;
                }
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start Time", GUILayout.Width(100));
            _startTime = EditorGUILayout.FloatField(_startTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("End Time", GUILayout.Width(100));
            _endTime = EditorGUILayout.FloatField(_endTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stay After End", GUILayout.Width(100));
            _stayAfterEnd = EditorGUILayout.Toggle(_stayAfterEnd, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Event Type", GUILayout.Width(100));
            _eventType = (Game.Runtime.ScenarioSystem.EventType)EditorGUILayout.EnumPopup(_eventType, GUILayout.Width(250));
            EditorGUILayout.EndHorizontal();
            
            if (_eventType == Runtime.ScenarioSystem.EventType.WeatherProperty)
            {
                EditorGUILayout.PropertyField(_weatherPropertySerialized, true);
            }
            else if (_eventType == Runtime.ScenarioSystem.EventType.Spawn)
            {
                EditorGUILayout.PropertyField(_spawnEventDataSerialized, true);
            }
            
            serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Save Event", GUILayout.Width(100)))
            {
                SaveEvent();
            }
            
            if (GUILayout.Button("Remove Event", GUILayout.Width(100)))
            {
                RemoveEvent();
            }
        }
        
        private void DrawEventTimeLine(Rect timelineRect)
        {
            float startY = timelineRect.y + TimelineHeight;
            for (int i = 0; i < _events.Count + 1; i++)
            {
                bool addedEvent = i >= _events.Count;
                    
                if(_eventRects.Count >= i)
                    _eventRects.Add(new List<Rect>());
                for(int j = 0; j < (addedEvent ? _addedEvents.Count : _events[i].Count); j++)
                {
                    var scenarioEvent = (addedEvent ? _addedEvents[j] : _events[i][j]);
                    Rect eventRect = new Rect(startX + (scenarioEvent.StartTime * widthPerSecond), startY + (i * _eventTimelineHeight),
                        (scenarioEvent.EndTime - scenarioEvent.StartTime) * widthPerSecond, _eventTimelineHeight - 2);
                    Rect borderLeft = new Rect(startX + (scenarioEvent.StartTime * widthPerSecond), startY + (i * _eventTimelineHeight),
                        1, _eventTimelineHeight - 2);
                    Rect borderRight = new Rect(startX + (scenarioEvent.StartTime * widthPerSecond) + (scenarioEvent.EndTime - scenarioEvent.StartTime) * widthPerSecond - 1, startY + (i * _eventTimelineHeight),
                        1, _eventTimelineHeight - 2);
                    
                    if(_eventRects[i].Count <= j)
                        _eventRects[i].Add(eventRect);
                    else
                        _eventRects[i][j] = eventRect;
                    
                    Color color = GetColor(scenarioEvent);
                    EditorGUI.DrawRect(eventRect, color);
                    Color borderColor = _currentEventEdit.x == i && _currentEventEdit.y == j ? Color.red : Color.orange;
                    EditorGUI.DrawRect(borderLeft, borderColor);
                    EditorGUI.DrawRect(borderRight, borderColor);
                }
            }
            
            ProcessEventsUIEvents(timelineRect);
        }

        private void SaveEvent()
        {
            string value = null;
            
            serializedObject.Update();
            
            if (_eventType == Runtime.ScenarioSystem.EventType.WeatherProperty)
            {
                value = JsonUtility.ToJson(((WeatherPropertyWrapper)_weatherPropertySerialized.boxedValue).WeatherProperty);
            }
            else if (_eventType == Runtime.ScenarioSystem.EventType.Spawn)
            {
                value = JsonUtility.ToJson((SpawnEventData)_spawnEventDataSerialized.boxedValue);
            }
            ScenarioEvent scenarioEvent = new ScenarioEvent(_startTime, _endTime, _stayAfterEnd, _eventType, value);
            
            var events = _musicTrack.Scenario.Events;
            if (_musicTrack.Scenario.Events == null)
                _musicTrack.Scenario.Events = Array.Empty<ScenarioEvent>();
            
            if(_currentEventEdit.x < _events.Count)
            {
                
                for (int i = 0; i < events.Length; i++)
                {
                    if (!events[i].Equals(_events[_currentEventEdit.x][_currentEventEdit.y])) continue;
                    events[i] = scenarioEvent;
                }
            }
            else
            {
                _musicTrack.Scenario.Events = _musicTrack.Scenario.Events.Append(scenarioEvent).OrderBy(e => e.StartTime).ToArray();
            }
                        
            File.WriteAllText(Path.Combine(_musicTrack.Path, "Events.json"), JsonUtility.ToJson(_musicTrack.Scenario));
            Open();
        }
        
        private void RemoveEvent()
        {
            serializedObject.Update();
            
            if(_currentEventEdit.x < _events.Count)
            {
                _musicTrack.Scenario.Events = _musicTrack.Scenario.Events.Where(e => !e.Equals(_events[_currentEventEdit.x][_currentEventEdit.y])).ToArray();
                File.WriteAllText(Path.Combine(_musicTrack.Path, "Events.json"), JsonUtility.ToJson(_musicTrack.Scenario));
            }
            else
            {
                _addedEvents.Clear();
            }
            _selectedEvent = false;
            _currentEventEdit = new Vector2Int(-1, -1);
            
            Open();
        }
        
        private bool _selectedEvent;
        private Vector2Int _currentEventEdit;
        
        private void ProcessEventsUIEvents(Rect timelineRect)
        {
            if(_addedEvents.Count > 0) return;
            Event currentEvent = Event.current;
            
            // Check if mouse interacts inside the timeline coordinates
            if (timelineRect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    _selectedEvent = false;
                    _currentEventEdit = new Vector2Int(-1, -1);
                    for (int i = 0; i < _eventRects.Count; i++)
                    {
                        for (int j = 0; j < _eventRects[i].Count; j++)
                        {
                            if (_eventRects[i][j].Contains(currentEvent.mousePosition))
                            {
                                _selectedEvent = true;
                                _currentEventEdit = new Vector2Int(i, j);
                                
                                _startTime = _events[i][j].StartTime;
                                _endTime = _events[i][j].EndTime;
                                _stayAfterEnd = _events[i][j].StayAfterEnd;
                                _eventType = _events[i][j].Type;
                                try
                                {
                                    _weatherProperty = new WeatherPropertyWrapper(JsonUtility.FromJson<WeatherProperty>(_events[i][j].Value));
                                }
                                catch { }

                                try
                                {
                                    _spawnEventData = JsonUtility.FromJson<SpawnEventData>(_events[i][j].Value);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }
        
        private Color GetColor(ScenarioEvent scenarioEvent)
        {
            switch (scenarioEvent.Type)
            {
                case Runtime.ScenarioSystem.EventType.WeatherProperty: return Color.skyBlue; 
                case Runtime.ScenarioSystem.EventType.Spawn: return Color.softGreen; 
                case Runtime.ScenarioSystem.EventType.Animation: return Color.darkOrange;
            }
            return Color.softYellow;
        }
        
        #endregion
    }
}