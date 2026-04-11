using System.Reflection;
using Game.Runtime.AudioSystem;
using Game.Runtime.Plugins.Array2D;
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
                    Play(config.AudioClip, config.MaxVolume);
                if (GUILayout.Button("Stop"))
                    Stop();
            }
        }
        
        public static void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;

            if (_audioSourceGameObject != null)
                Stop();
            
            GameObject tempGo = new GameObject("TempAudio");
            tempGo.hideFlags = HideFlags.HideAndDontSave; 
            _audioSourceGameObject = tempGo;
            
            AudioSource source = tempGo.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.playOnAwake = false;
            _audioSource = source;
            
            source.Play();
            
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