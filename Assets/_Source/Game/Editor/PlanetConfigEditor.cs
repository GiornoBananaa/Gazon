using Game.Runtime.Array2D;
using Game.Runtime.Array2D.Array2DTypes.ObjectTypes;
using Game.Runtime.PlanetSystem.Configs;
using Game.Runtime.Plugins.Array2D;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(PlanetConfig))]
    public class PlanetConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _biomeSize;
        private SerializedProperty _biomeSizeInChunks;
        private SerializedProperty _biomes;
        private SerializedProperty _chunkWeatherBlendDistance;

        private void OnEnable()
        {
            _biomeSize = serializedObject.FindProperty("BiomeSize");
            _biomeSizeInChunks = serializedObject.FindProperty("BiomeSizeInChunks");
            _biomes = serializedObject.FindProperty("Biomes");
            _chunkWeatherBlendDistance = serializedObject.FindProperty("ChunkWeatherBlendDistance");
        }

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_biomeSize);
                EditorGUILayout.PropertyField(_biomeSizeInChunks);
                if(EditorGUI.EndChangeCheck())
                {
                    Array2DBiome biomes = (Array2DBiome)_biomes.boxedValue;
                    float size = _biomeSize.floatValue / _biomeSizeInChunks.floatValue;
                    
                    foreach (var biome in biomes)
                    {
                        Terrain terrain = (Terrain)new SerializedObject(biome.TerrainPrefab).FindProperty("_terrain").boxedValue;
                        
                        var terrainData = terrain.terrainData;
                        
                        Vector3 newSize = new Vector3(size, terrainData.size.y, size);
                        terrainData.size = newSize;
                    }
                }
                
                serializedObject.ApplyModifiedProperties(); 
            }
            
            EditorGUILayout.PropertyField(_biomes);
            
            EditorGUILayout.PropertyField(_chunkWeatherBlendDistance);
        }
    }
}