using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Runtime.AudioSystem
{
    public class AudioPlayer : IAudioPlayer, IDisposable
    {
        private readonly struct ActiveSoundSource
        {
            public readonly AudioSource Source;
            public readonly CancellationTokenSource CancellationSource;

            public ActiveSoundSource(AudioSource source, CancellationTokenSource cancellationSource)
            {
                Source = source;
                CancellationSource = cancellationSource;
            }
        }
        
        
        private readonly Dictionary<int, AudioLowPassFilter> _lowPassFilters = new();
        private readonly ObjectPool<AudioSource> _audioSourcePool;
        private readonly Dictionary<int, ActiveSoundSource> _activeSources = new();
        private readonly Dictionary<int, Tween> _frequencyTweens = new();

        private int _allAudioSourcesCount;
        
        public AudioPlayer()
        {
            _audioSourcePool = new ObjectPool<AudioSource>(CreateAudioSource, ActivateAudioSource, DeactivateAudioSource);
        }
        
        public AudioSoundSource Play(AudioSound sound, Vector3 point, float volume = 1, float spatialBlend = 0, float pitch = 1, float maxCutoffFrequency = 22000f, bool loop = false)
        {
            var audioSource = _audioSourcePool.Get();
            audioSource.transform.position = point;
            audioSource.volume = Mathf.Lerp(0, sound.MaxVolume, volume);
            audioSource.spatialBlend = Mathf.Lerp(0, sound.MaxSpatialBlend, spatialBlend);
            audioSource.pitch = pitch;
            audioSource.clip = sound.AudioClip;
            audioSource.loop = loop;
            audioSource.Play();
            
            int id = audioSource.GetHashCode();
            
            Tween tween = DOTween.To(() => _lowPassFilters[id].cutoffFrequency, v => _lowPassFilters[id].cutoffFrequency = v, maxCutoffFrequency, 0.05f);
            _frequencyTweens[id] = tween;
            
            var cancellationToken = new CancellationTokenSource();
            _activeSources.Add(id, new ActiveSoundSource(audioSource, cancellationToken));
            var soundSource = new AudioSoundSource(audioSource, sound.MaxVolume, sound.MaxSpatialBlend, maxCutoffFrequency);
            _ = StopOnClipEnd(cancellationToken, soundSource, audioSource.clip.length);
            return soundSource;
        }
        
        public AudioSoundSource Play(AudioSound sound, Vector3 point, AudioSoundSource source, float volume = 1, float spatialBlend = 0, float pitch = 1, float maxCutoffFrequency = 22000f, bool loop = false)
        {
            if (!_activeSources.TryGetValue(source.GetHashCode(), out var activeSoundSource))
                return Play(sound, point, volume, spatialBlend);
            
            var audioSource = activeSoundSource.Source;
            
            audioSource.transform.position = point;
            audioSource.volume = Mathf.Lerp(0, sound.MaxVolume, volume);
            audioSource.spatialBlend = Mathf.Lerp(0, sound.MaxSpatialBlend, spatialBlend);
            audioSource.pitch = pitch;
            audioSource.clip = sound.AudioClip;
            audioSource.loop = loop;
            audioSource.Play();
            
            int id = audioSource.GetHashCode();
            Tween tween = DOTween.To(() => _lowPassFilters[id].cutoffFrequency, v => _lowPassFilters[id].cutoffFrequency = v, maxCutoffFrequency, 0.05f);
            _frequencyTweens[id] = tween;
            
            activeSoundSource.CancellationSource?.Cancel();
            activeSoundSource.CancellationSource?.Dispose();
            var cancellationToken = new CancellationTokenSource();
            _activeSources[source.GetHashCode()] = new ActiveSoundSource(audioSource, cancellationToken);
            
            var soundSource = new AudioSoundSource(audioSource, sound.MaxVolume, sound.MaxSpatialBlend, maxCutoffFrequency);
            _ = StopOnClipEnd(cancellationToken, soundSource, audioSource.clip.length);
            return soundSource;
        }
        
        public void Stop(AudioSoundSource sound)
        {
            int hash = sound.GetHashCode();
            if(!_activeSources.TryGetValue(hash, out ActiveSoundSource source)) return;
            source.Source.Stop();
            _activeSources.Remove(hash);
            _audioSourcePool.Release(source.Source);
            source.CancellationSource?.Cancel();
            source.CancellationSource?.Dispose();
        }

        public float GetSoundVolume(AudioSoundSource sound)
        {
            if(!_activeSources.TryGetValue(sound.GetHashCode(), out ActiveSoundSource source)) return 0;
            return source.Source.volume;
        }

        public void SetSoundVolume(AudioSoundSource sound, float volume)
        {
            if(!_activeSources.TryGetValue(sound.GetHashCode(), out ActiveSoundSource source)) return;
            source.Source.volume = Mathf.Lerp(0, sound.MaxVolume, volume);
        }
        
        public void SetCutoffFrequency(AudioSoundSource sound, float frequency)
        {
            int id = sound.GetHashCode();
            if(!_activeSources.TryGetValue(id, out ActiveSoundSource source)) return;
            if(_frequencyTweens.TryGetValue(id, out Tween lastTween))
                lastTween?.Kill();
            Tween tween = DOTween.To(() => _lowPassFilters[id].cutoffFrequency, v => _lowPassFilters[id].cutoffFrequency = v, Mathf.Clamp(frequency, 800f, sound.MaxLowPassFrequency), 0.05f);
            _frequencyTweens.Add(id, tween);
        }
        
        public void SetSpatialBlend(AudioSoundSource sound, float spatialBlend)
        {
            if(!_activeSources.TryGetValue(sound.GetHashCode(), out ActiveSoundSource source)) return;
            source.Source.spatialBlend = Mathf.Lerp(0, sound.MaxSpatialBlend, spatialBlend);
        }
        
        private AudioSource CreateAudioSource()
        {
            AudioSource source = new GameObject().AddComponent<AudioSource>();
            AudioLowPassFilter audioLowPassFilter = source.gameObject.AddComponent<AudioLowPassFilter>();
            AudioReverbFilter audioReverbFilter = source.gameObject.AddComponent<AudioReverbFilter>();
            audioReverbFilter.reverbPreset = AudioReverbPreset.Forest;
            audioReverbFilter.reverbLevel = -1000;
            audioReverbFilter.diffusion = 100;
            source.playOnAwake = false;
            _allAudioSourcesCount++;
            source.gameObject.name = $"AudioSource {_allAudioSourcesCount.ToString()}";
            _lowPassFilters[source.GetHashCode()] = audioLowPassFilter;
            return source;
        }

        private void ActivateAudioSource(AudioSource source) => source.enabled = true;
        
        private void DeactivateAudioSource(AudioSource source)
        {
            source.enabled = false;
        }
        
        
        private async UniTaskVoid StopOnClipEnd(CancellationTokenSource cancellationTokenSource, AudioSoundSource source, float clipLength)
        {
            await UniTask.WaitForSeconds(clipLength, cancellationToken: cancellationTokenSource.Token);
            Stop(source);
        }
        
        public void Dispose()
        {
            foreach (var source in _activeSources.Values)
            {
                source.CancellationSource?.Cancel();
                source.CancellationSource?.Dispose();
            }
        }
    }
}