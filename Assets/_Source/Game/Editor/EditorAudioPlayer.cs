using System;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class EditorAudioPlayer : IDisposable
    {
        private AudioSource _audioSource;
        private AudioLowPassFilter _lowPassFilter;
        private GameObject _audioSourceGameObject;
        
        public void Play(AudioClip clip, float volume = 1f, float pitch = 1f, float cutOffFrequency = 22000f)
        {
            if (clip == null) return;

            if (_audioSourceGameObject != null)
                Stop();
            
            GameObject tempGo = new GameObject("TempAudio");
            tempGo.hideFlags = HideFlags.HideAndDontSave;
            _audioSourceGameObject = tempGo;
            
            _audioSource = tempGo.AddComponent<AudioSource>();
            _lowPassFilter = tempGo.AddComponent<AudioLowPassFilter>();
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.playOnAwake = false;
            _lowPassFilter.cutoffFrequency = cutOffFrequency;
            _audioSource.Play();
            
            EditorApplication.update -= UpdateSound;
            EditorApplication.update += UpdateSound;
        }

        public void Stop()
        {
            if (_audioSourceGameObject == null) return;
            UnityEngine.Object.DestroyImmediate(_audioSourceGameObject);
            _audioSourceGameObject = null;
            EditorApplication.update -= UpdateSound;
        }
        
        private void UpdateSound()
        {
            if (_audioSource == null || !_audioSource.isPlaying)
            {
                if(_audioSourceGameObject != null) 
                    UnityEngine.Object.DestroyImmediate(_audioSourceGameObject);
                EditorApplication.update -= UpdateSound;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}