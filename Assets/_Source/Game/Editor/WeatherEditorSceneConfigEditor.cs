using System;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(WeatherEditorSceneConfig))]
    public class WeatherEditorSceneConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var config = (WeatherEditorSceneConfig)target;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Play"))
                    config.PlayTween();
                if (GUILayout.Button("Stop"))
                    config.PlayTween();
            }
        }
    }
}