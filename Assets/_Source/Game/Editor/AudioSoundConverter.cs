using System.Collections.Generic;
using System.Linq;
using Game.Runtime.AudioSystem;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class AudioSoundConverter : EditorWindow
    {
        private string sourcePath = "Source path";
        private string targetPath = "Target path";
        private string baseName = "base name";
        private int startIndex = 1;

        [MenuItem("Tools/AudioSoundConverter")]
        public static void ShowWindow()
        {
            GetWindow<AudioSoundConverter>("AudioSoundConverter");
        }

        private void OnGUI()
        {
            sourcePath = EditorGUILayout.TextField("Source Path", sourcePath);
            targetPath = EditorGUILayout.TextField("Target Path", targetPath);
            startIndex = EditorGUILayout.IntField("Start Index", startIndex);
            baseName = EditorGUILayout.TextField("Base Name", baseName);

            if (GUILayout.Button("Parse"))
            {
                Parse();
            }
        }

        private void Parse()
        {
            var clips = GetAllAssets<AudioClip>(sourcePath, $"t:{nameof(AudioClip)}");
            var sounds = GetAllAssets<AudioSound>(targetPath, $"t:{nameof(AudioSound)}");
            
            for (int i = 0; i < clips.Count; i++)
            {
                string assetName = baseName + " " + (i + startIndex);
                AudioSound asset = sounds.FirstOrDefault(s => s.name == assetName);
                if (asset == null)
                {
                    asset = CreateInstance<AudioSound>();
                    AssetDatabase.CreateAsset(asset, targetPath + "/" + assetName + ".asset");
                }
                else
                {
                    Undo.RecordObject(asset, "AudioSoundConverter edit");
                }
                asset.AudioClip = clips[i];
                EditorUtility.SetDirty(asset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private List<T> GetAllAssets<T>(string path, string filter) where T : Object
        {
            List<T> list = new List<T>();
            
            string[] guids = AssetDatabase.FindAssets(filter, new [] { path });
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                
                if (asset != null)
                {
                    list.Add(asset);
                }
            }

            return list;
        }
    }
}