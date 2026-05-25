using System.Collections.Generic;
using System.Linq;
using Game.Runtime.RhythmSystem;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Runtime.PianoFeature
{
    public class RhythmSheetVisualizer : MonoBehaviour, ISheetVisualizer
    {
        [SerializeField] private RhythmNoteView _notePrefab;
        [SerializeField] private RhythmTrackView _trackPrefab;
        [SerializeField] private float _lengthInMeters = 1;
        [SerializeField] private float _distanceBetweenTracks = 0.1f;
        [SerializeField] private float _noteHeight = 0.2f;
        
        private readonly Dictionary<RhythmKey, RhythmNoteView> _showedNotes = new();
        private readonly List<RhythmTrackView> _tracks = new();
        private ObjectPool<RhythmNoteView> _pool;
        private IRhythmSheet _sheet;
        private bool _show;
        private float _lengthInSeconds;
        private int _trackCount;
        
        [Inject]
        public void Construct(IRhythmSheet sheet)
        {
            _sheet = sheet;
            _pool = new ObjectPool<RhythmNoteView>(
                () => Instantiate(_notePrefab),
                note =>
                {
                    note.ResetTrack();
                    note.gameObject.SetActive(true);
                },
                note =>
                {
                    note.gameObject.SetActive(false);
                });

            sheet.OnRhythmResult += OnRhythmResult;
        }

        private void Update()
        {
            if(!_show) return;
            int trackIndex = 0;
            foreach (var track in _sheet.RhythmKeys)
            {
                int keyIndex = 0;
                foreach (var rhythmKey in track)
                {
                    float offsetZ = (rhythmKey.StartTime - _sheet.Time) / _lengthInSeconds * _lengthInMeters;
                    float offsetX = trackIndex * _distanceBetweenTracks - (_trackCount - 1) * _distanceBetweenTracks / 2;
                    bool show = _sheet.Time + _lengthInSeconds > rhythmKey.StartTime  && _sheet.Time < rhythmKey.EndTime;
                    
                    if (show)
                    {
                        if(!_showedNotes.ContainsKey(rhythmKey)) 
                            _showedNotes[rhythmKey] = _pool.Get();

                        RhythmNoteView view = _showedNotes[rhythmKey];
                        float length = rhythmKey.Length / _lengthInSeconds * _lengthInMeters;
                        view.SetNoteLength(Mathf.Clamp(length + Mathf.Min(offsetZ, 0) + Mathf.Min(_lengthInMeters - (offsetZ + length), 0), 0.02f, _lengthInMeters));
                        //view.SetNoteLength(Mathf.Clamp(Mathf.Clamp(offsetZ + length, 0, _lengthInMeters) - offsetZ, 0, _lengthInMeters));
                        Color baseColor = keyIndex % 2 == 0 ? new Color(0.6f, 0.6f, 0.6f) : Color.white;
                        view.SetBaseColor(Color.Lerp(baseColor,  Color.gray, ( rhythmKey.EndTime - _sheet.Time) / _lengthInSeconds));
                        view.transform.position = new Vector3(transform.position.x + offsetX, transform.position.y + _noteHeight, transform.position.z + Mathf.Clamp(offsetZ, 0, _lengthInMeters));
                    }
                    else if(_showedNotes.ContainsKey(rhythmKey)) 
                    {
                        _pool.Release(_showedNotes[rhythmKey]);
                        _showedNotes.Remove(rhythmKey);
                    }

                    keyIndex++;
                }

                trackIndex++;
            }
        }

        private void OnDestroy()
        {
            _sheet.OnRhythmResult -= OnRhythmResult;
        }

        public void SetLengthInSeconds(float seconds)
        {
            _lengthInSeconds = seconds;
        }
        
        public void Show()
        {
            _trackCount = _sheet.RhythmKeys.Count();
            _show = true;
            for (int i = _tracks.Count; i < _trackCount; i++)
            {
                _tracks.Add(Instantiate(_trackPrefab));
            }
            
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (i >= _trackCount)
                {
                    _tracks[i].gameObject.SetActive(false);
                    continue;
                }
                Color baseColor = i % 2 == 0 ? new Color(0.5450981f, 0.2705882f, 0.07450981f, 1f) : new Color(0.7f, 0.4f, 0.24f, 1f);
                
                _tracks[i].gameObject.SetActive(true);
                _tracks[i].SetTrackSize(_lengthInMeters, _distanceBetweenTracks);
                _tracks[i].SetBaseColor(baseColor);
                _tracks[i].transform.position = new Vector3(
                    transform.position.x + _distanceBetweenTracks * i - _distanceBetweenTracks * (_trackCount - 1) / 2, 
                    transform.position.y, transform.position.z);
            }
        }
        
        public void Hide()
        {
            _show = false;
        }

        private void OnRhythmResult(RhythmKey rhythmKey, RhythmResult rhythmResult)
        {
            if (_showedNotes.TryGetValue(rhythmKey, out var view))
            {
                switch (rhythmResult)
                {
                    case RhythmResult.Success:
                        view.SetHighlightColor(Color.softGreen);
                        view.Highlight(true);
                        break;
                    case RhythmResult.Miss:
                        view.SetHighlightColor(Color.softYellow);
                        view.Highlight(true);
                        break;
                    case RhythmResult.Fail:
                        view.SetHighlightColor(Color.softRed);
                        view.Highlight(true);
                        break;
                }
            }
            
            _tracks[rhythmKey.KeyIndex].SetHighlightColor(new Color(1, 0.66f, 0.39f, 1f));
            _tracks[rhythmKey.KeyIndex].HighlightBlink();
        }
    }
}