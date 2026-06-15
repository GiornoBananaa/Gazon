using Game.Runtime.AudioSystem;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(AudioSound))]
    [CanEditMultipleObjects]
    public class AudioSoundEditor : UnityEditor.Editor
    {
        //private SerializedProperty _biomeSize;

        private static AudioSource _audioSource;
        private static AudioLowPassFilter _lowPassFilter;
        private static GameObject _audioSourceGameObject;
        
        private void OnEnable()
        {
            //_biomeSize = serializedObject.FindProperty("BiomeSize");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var config = (AudioSound)target;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Play"))
                    Play(config.AudioClip, config.MaxVolume, config.Pitch , config.MaxLowPassFrequency);
                if (GUILayout.Button("Stop"))
                    Stop();
            }
        }
        
        public static void Play(AudioClip clip, float volume = 1f, float pitch = 1f, float cutOffFrequency = 22000f)
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

        public static void Stop()
        {
            if (_audioSourceGameObject == null) return;
            DestroyImmediate(_audioSourceGameObject);
            _audioSourceGameObject = null;
            EditorApplication.update -= UpdateSound;
        }
        
        private static void UpdateSound()
        {
            if (_audioSource == null || !_audioSource.isPlaying)
            {
                if(_audioSourceGameObject != null) 
                    DestroyImmediate(_audioSourceGameObject);
                EditorApplication.update -= UpdateSound;
            }
        }
    }
}