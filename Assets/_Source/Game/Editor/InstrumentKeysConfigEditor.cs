using System;
using Game.Runtime.AudioSystem;
using Game.Runtime.Configs;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(InstrumentKeysConfig))]
    [CanEditMultipleObjects]
    public class InstrumentKeysConfigEditor : UnityEditor.Editor
    {
        private string _folderPath = "FolderPath";
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var config = (InstrumentKeysConfig)target;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                _folderPath = GUILayout.TextField(_folderPath);
                if (GUILayout.Button("Parse"))
                    Parse(_folderPath, config);
            }
        }
        
        public static void Parse(string path, InstrumentKeysConfig config)
        {
            string filter = $"t:{nameof(AudioSound)}";
            string[] guids = AssetDatabase.FindAssets(filter, new [] { path });
            
            Undo.RecordObject(config, "AudioSound parse");
            int index = 0;
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AudioSound asset = AssetDatabase.LoadAssetAtPath<AudioSound>(assetPath);
            
                if (asset != null)
                {
                    int noteNumber = config.StartNoteNumber + index;
                    if (config.Notes.Length <= index)
                        Array.Resize(ref config.Notes, config.Notes.Length + 1);
                    config.Notes[index] = new NoteConfig()
                    {
                        Note = (NoteType)(noteNumber % 12),
                        Octave = noteNumber / 12 - 1,
                        Sound = asset,
                    };
                    index++;
                }
            }
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }
    }
}