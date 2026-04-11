using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using UnityEngine;

namespace Game.Runtime.PianoRhythmSystem
{
    public class PianoNoteTweener : IPianoNoteTweener
    {
        private readonly IAudioPlayer _audioPlayer;
        private readonly Dictionary<int, AudioSoundSource> _pressedSources = new();
        private readonly Dictionary<AudioSoundSource, Tween> _sourceTweens = new();
        
        private readonly float _noteEndDuration;
        private readonly float _noteEndSustainDuration;
        private readonly float _noteTransitionFadingDuration;
        
        private Vector3 _pianoPosition;
        private float _spatialBlend;
        private float _maxVolume;
        private bool _sustain;
        
        public PianoNoteTweener(IAudioPlayer audioPlayer, PianoKeysConfig data)
        {
            _audioPlayer = audioPlayer;
            _noteEndDuration = data.NoteEndDuration;
            _noteEndSustainDuration = data.PedalNoteEndDuration;
            _noteTransitionFadingDuration = data.NoteTransitionFadingDuration;
        }

        public void SetMaxVolume(float value)
        {
            _maxVolume = value;

            foreach (var source in _pressedSources.Values)
            {
                _audioPlayer.SetSoundVolume(source, value);
            }
        }

        public void SetPianoWorldPosition(Vector3 position)
        {
            _pianoPosition = position;
        }
        
        public void SetSpatialBlend(float spatialBlend)
        {
            _spatialBlend = spatialBlend;
            foreach (var source in _pressedSources.Values)
            {
                _audioPlayer.SetSpatialBlend(source, spatialBlend);
            }
        }
        
        public void StartNote(int id, AudioSound sound)
        {
            if(_pressedSources.TryGetValue(id, out var pressedSource))
            {
                var pressedSourceCopy = pressedSource;
                
                if (_sourceTweens.ContainsKey(pressedSourceCopy))
                {
                    _sourceTweens[pressedSourceCopy]?.Kill();
                    _sourceTweens.Remove(pressedSourceCopy);
                }
                
                Tween tween = DOTween.To(() => _audioPlayer.GetSoundVolume(pressedSourceCopy), v => _audioPlayer.SetSoundVolume(pressedSourceCopy, v), 0, _noteTransitionFadingDuration * _audioPlayer.GetSoundVolume(pressedSourceCopy));
                tween.OnComplete(() =>
                {
                    _audioPlayer.Stop(pressedSourceCopy);
                    if(_pressedSources.TryGetValue(id, out var pressed) && pressed.GetHashCode() == pressedSourceCopy.GetHashCode())
                        _pressedSources.Remove(id);
                });
                _sourceTweens[pressedSourceCopy] = tween;
            }
            AudioSoundSource source = _audioPlayer.Play(sound, _pianoPosition, _maxVolume, _spatialBlend);
            
            _pressedSources[id] = source;
        }
     
        public void EndNote(int id)
        {
            if(!_pressedSources.TryGetValue(id, out var source)) return;
            float endDuration = _sustain ? _noteEndSustainDuration : _noteEndDuration;
            Tween tween = DOTween.To(() => _audioPlayer.GetSoundVolume(source), v => _audioPlayer.SetSoundVolume(source, v), 0, endDuration * _audioPlayer.GetSoundVolume(source));
            tween.OnComplete(() =>
            {
                _audioPlayer.Stop(source);
                if(_pressedSources.TryGetValue(id, out var pressedSource) && pressedSource.GetHashCode() == source.GetHashCode())
                    _pressedSources.Remove(id);
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
        }
    }
}