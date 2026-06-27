using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class InstrumentNoteTweener : IInstrumentNoteTweener
    {
        private readonly IAudioPlayer _audioPlayer;
        private readonly Dictionary<int, AudioSoundSource> _pressedSources = new();
        private readonly Dictionary<int, AudioSoundSource> _sustainedSources = new();
        private readonly Dictionary<AudioSoundSource, Tween> _sourceTweens = new();
        
        private readonly float _noteEndDuration;
        private readonly float _noteTransitionFadingDuration;
        private readonly float _noteMinVelocityVolumeStartFadeRatio;
        private readonly int _notesCount;
        private readonly float _minLowPassFrequency;
        private readonly float _maxLowPassFrequency;
        private readonly float _maxVelocity;
        private readonly float _shortNoteVolumeThreshold;
        private readonly bool _loopNote;
        
        private Vector3 _pianoLineStart;
        private Vector3 _pianoLineEnd;
        private float _spatialBlend;
        private float _maxVolume;
        private float _lastNoteTime;
        private int _lastNoteId;
        private bool _sustain;
        
        public InstrumentNoteTweener(IAudioPlayer audioPlayer, InstrumentKeysConfig data)
        {
            _audioPlayer = audioPlayer;
            _noteEndDuration = data.NoteEndDuration;
            _noteTransitionFadingDuration = data.NoteTransitionFadingDuration;
            _noteMinVelocityVolumeStartFadeRatio = data.NoteMinVelocityVolumeStartFadeRatio;
            _shortNoteVolumeThreshold = data.ShortNoteVolumeThreshold;
            _minLowPassFrequency = data.MinLowPassFrequency;
            _maxLowPassFrequency = data.MaxLowPassFrequency;
            _notesCount = data.Notes.Length;
            _maxVelocity = data.MaxKeyVelocity;
            _loopNote = data.LoopNote;
        }

        public void SetMaxVolume(float value)
        {
            _maxVolume = value;
        }

        public void SetPianoWorldPosition(Vector3 start, Vector3 end)
        {
            _pianoLineStart = start;
            _pianoLineEnd = end;
        }
        
        public void SetSpatialBlend(float spatialBlend)
        {
            _spatialBlend = spatialBlend;
            foreach (var source in _pressedSources.Values)
            {
                _audioPlayer.SetSpatialBlend(source, spatialBlend);
            }
        }
        
        public void StartNote(int id, AudioSound sound, float velocity, float targetLength = 0)
        {
            velocity *= _maxVelocity;
            
            float velocityKoef = (1 - (1 - velocity) * (1 - velocity));
            float volume = _maxVolume * velocity;
            float maxLowCutFrequency = Mathf.Lerp(_minLowPassFrequency, _maxLowPassFrequency, velocityKoef);
            bool continueNote = _lastNoteId == id && Time.time - _lastNoteTime > _noteEndDuration * (1 - (1 - volume) * (1 - volume));
            bool fadeStart = !continueNote && _noteMinVelocityVolumeStartFadeRatio > 0 && targetLength > _shortNoteVolumeThreshold;
            float startVolume = fadeStart ? 0 : volume;
            
            if(_pressedSources.TryGetValue(id, out var pressedSource))
            {
                var pressedSourceCopy = pressedSource;
                
                if (_sourceTweens.ContainsKey(pressedSourceCopy))
                {
                    _sourceTweens[pressedSourceCopy]?.Kill();
                    _sourceTweens.Remove(pressedSourceCopy);
                }
                
                Tween tween = DOTween.To(() => _audioPlayer.GetSoundVolume(pressedSourceCopy), v => SetNoteVolume(pressedSourceCopy, v), 
                    0, _noteTransitionFadingDuration * (continueNote ? 2 : 1) * _audioPlayer.GetSoundVolume(pressedSourceCopy));
                tween.OnComplete(() =>
                {
                    _audioPlayer.Stop(pressedSourceCopy);
                    if(_pressedSources.TryGetValue(id, out var pressed) && pressed.GetHashCode() == pressedSourceCopy.GetHashCode())
                        _pressedSources.Remove(id);
                });
                _sourceTweens[pressedSourceCopy] = tween;
            }
            
            AudioSoundSource source = _audioPlayer.Play(sound, Vector3.Lerp(_pianoLineStart, _pianoLineEnd, (float)id / _notesCount), 
                startVolume, _spatialBlend, 1, maxLowCutFrequency, _loopNote);
            _pressedSources[id] = source;
            _sustainedSources.Remove(id);
            
            if(fadeStart)
            {
                if (_sourceTweens.ContainsKey(source))
                {
                    _sourceTweens[source]?.Kill();
                    _sourceTweens.Remove(source);
                }
                
                float fadeDuration = Mathf.Lerp(targetLength * _noteMinVelocityVolumeStartFadeRatio, 0, velocity);
                _sourceTweens[source] = DOTween.To(() => _audioPlayer.GetSoundVolume(source), v => SetNoteVolume(source, v), volume, fadeDuration);
            }

            _lastNoteId = id;
            _lastNoteTime = Time.time;
        }
     
        public void EndNote(int id)
        {
            if(!_pressedSources.TryGetValue(id, out var source)) return;
            
            if (_sustain)
            {
                _sustainedSources.Add(id, source);
                return;
            }
            
            float volume = _audioPlayer.GetSoundVolume(source);
            float endDuration = _noteEndDuration * (1 - (1 - volume) * (1 - volume));
            Tween tween = DOTween.To(() => _audioPlayer.GetSoundVolume(source), v => SetNoteVolume(source, v), 0, endDuration);
            tween.OnComplete(() =>
            {
                _audioPlayer.Stop(source);
                if(_pressedSources.TryGetValue(id, out var pressedSource) && pressedSource.GetHashCode() == source.GetHashCode())
                    _pressedSources.Remove(id);
                if(_sustainedSources.ContainsKey(id))
                    _sustainedSources.Remove(id);
            });
            _sourceTweens[source] = tween;
        }

        public void EnableSustain()
        {
            _sustain = true;
        }

        public void DisableSustain()
        {
            _sustain = false;
            foreach (var pair in _sustainedSources)
            {
                if(_sourceTweens.TryGetValue(pair.Value, out var sourceTween))
                    sourceTween.Kill();
                float volume = _audioPlayer.GetSoundVolume(pair.Value);
                float endDuration = _noteEndDuration * (1 - (1 - volume) * (1 - volume));
                Tween tween = DOTween.To(() => _audioPlayer.GetSoundVolume(pair.Value), v => SetNoteVolume(pair.Value, v), 0, endDuration);
                tween.OnComplete(() =>
                {
                    _audioPlayer.Stop(pair.Value);
                    if(_pressedSources.TryGetValue(pair.Key, out var pressedSource) && pressedSource.GetHashCode() == pair.Value.GetHashCode())
                        _pressedSources.Remove(pair.Key);
                });
            }
            _sustainedSources.Clear();
        }
        
        private void SetNoteVolume(AudioSoundSource sound, float volume)
        {
            _audioPlayer.SetSoundVolume(sound, volume);
        }
    }
}