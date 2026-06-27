using System.Collections.Generic;
using Game.Runtime.AudioSystem;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(AudioSound))]
    [CanEditMultipleObjects]
    public class AudioSoundEditor : UnityEditor.Editor
    {
        private static EditorAudioPlayer _audioPlayer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var config = (AudioSound)target;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Play"))
                    _audioPlayer.Play(config.AudioClip, config.MaxVolume, config.Pitch , config.MaxLowPassFrequency);
                if (GUILayout.Button("Stop"))
                    _audioPlayer.Stop();
            }
        }
    }
}