using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Utils;
using R3;
using UnityEngine;

namespace Game.Runtime.TerrainChunkSystem
{
    public class TerrainBiomeBlender
    {
        // Grass
        private static readonly int _grassFarColorProperty =  Shader.PropertyToID("_FarColor");
        private static readonly int _grassNearColorProperty =  Shader.PropertyToID("_NearColor");
        private static readonly int _grassShadowColorProperty =  Shader.PropertyToID("_ShadowColor");
        private static readonly int _grassBottomColorProperty =  Shader.PropertyToID("_BottomColor");
        
        private static readonly int _grassBorderXProperty =  Shader.PropertyToID("_OtherX_Coordinate");
        private static readonly int _grassBorderYProperty =  Shader.PropertyToID("_OtherY_Coordinate");
        
        private static readonly int _grassFarColorXForwardProperty =  Shader.PropertyToID("_OtherX_FarColor");
        private static readonly int _grassNearColorXForwardProperty =  Shader.PropertyToID("_OtherX_NearColor");
        private static readonly int _grassShadowColorXForwardProperty =  Shader.PropertyToID("_OtherX_ShadowColor");
        private static readonly int _grassBottomColorXForwardProperty =  Shader.PropertyToID("_OtherX_BottomColor");
        private static readonly int _grassFarColorYForwardProperty =  Shader.PropertyToID("_OtherY_FarColor");
        private static readonly int _grassNearColorYForwardProperty =  Shader.PropertyToID("_OtherY_NearColor");
        private static readonly int _grassShadowColorYForwardProperty =  Shader.PropertyToID("_OtherY_ShadowColor");
        private static readonly int _grassBottomColorYForwardProperty =  Shader.PropertyToID("_OtherY_BottomColor");
        
        private static readonly int _grassFarColorXBackProperty =  Shader.PropertyToID("_OtherXBack_FarColor");
        private static readonly int _grassNearColorXBackProperty =  Shader.PropertyToID("_OtherXBack_NearColor");
        private static readonly int _grassShadowColorXBackProperty =  Shader.PropertyToID("_OtherXBack_ShadowColor");
        private static readonly int _grassBottomColorXBackProperty =  Shader.PropertyToID("_OtherXBack_BottomColor");
        private static readonly int _grassFarColorYBackProperty =  Shader.PropertyToID("_OtherYBack_FarColor");
        private static readonly int _grassNearColorYBackProperty =  Shader.PropertyToID("_OtherYBack_NearColor");
        private static readonly int _grassShadowColorYBackProperty =  Shader.PropertyToID("_OtherYBack_ShadowColor");
        private static readonly int _grassBottomColorYBackProperty =  Shader.PropertyToID("_OtherYBack_BottomColor");
        
        //Terrain
        private static readonly int _terrainMainColorProperty =  Shader.PropertyToID("_MainColor");
        private static readonly int _terrainShadowColorProperty =  Shader.PropertyToID("_ShadowColor");
        private static readonly int _terrainBorderXProperty =  Shader.PropertyToID("_OtherX_Coordinate");
        private static readonly int _terrainBorderYProperty =  Shader.PropertyToID("_OtherY_Coordinate");
        
        private static readonly int _terrainMainColorXProperty =  Shader.PropertyToID("_OtherX_MainColor");
        private static readonly int _terrainShadowColorXProperty =  Shader.PropertyToID("_OtherX_ShadowColor");
        private static readonly int _terrainMainColorYProperty =  Shader.PropertyToID("_OtherY_MainColor");
        private static readonly int _terrainShadowColorYProperty =  Shader.PropertyToID("_OtherY_ShadowColor");
        
        private static readonly int _terrainMainColorXBackProperty =  Shader.PropertyToID("_OtherXBack_MainColor");
        private static readonly int _terrainShadowColorXBackProperty =  Shader.PropertyToID("_OtherXBack_ShadowColor");
        private static readonly int _terrainMainColorYBackProperty =  Shader.PropertyToID("_OtherYBack_MainColor");
        private static readonly int _terrainShadowColorYBackProperty =  Shader.PropertyToID("_OtherYBack_ShadowColor");
        
        
        private readonly PlanetMap _planetMap;
        
        public TerrainBiomeBlender(PlanetMap planetMap)
        {
            _planetMap = planetMap;
            _planetMap.CurrentChunkIndex.Skip(1).Subscribe(OnCurrentChunkIndexChanged);
        }
        
        private void OnCurrentChunkIndexChanged(Vector2Int index)
        {
            foreach (var materialGrouping in ArrayUtils.GetNeighborIndexesWithoutBorders(
                         _planetMap.Chunks.CurrentValue.GetLength(0), _planetMap.Chunks.CurrentValue.GetLength(1), index.x, index.y, true)
                         .OrderBy(i => i.x == index.x || i.y == index.y).ToLookup(i => _planetMap.Chunks.CurrentValue[i.x, i.y].TerrainMaterial))
            {
                SetBiomeMaterial(index, materialGrouping);
            }
        }

        private void SetBiomeMaterial(Vector2Int currentIndex, IEnumerable<Vector2Int> group)
        {
            float xBlendBorder = 0;
            float yBlendBorder = 0;
            
            float minXDistance = float.MaxValue;
            float minYDistance = float.MaxValue;
            
            bool currentIsNearX = false;
            bool currentIsNearY = false;
            
            Vector2Int materialIndex = currentIndex;
            Vector3 currentChunkPos = _planetMap.GetChunkCenterPosition(currentIndex);
            
            foreach (var chunk in group)
            {
                materialIndex = chunk;
                
                Vector2Int relativePos = _planetMap.GetChunkRelativePosition(chunk, currentIndex);
                Vector3 chunkPosition = currentChunkPos + new Vector3(relativePos.x, 0, relativePos.y) * _planetMap.ChunkSize.CurrentValue;
                
                foreach(Vector2Int nearIndex in ArrayUtils.GetNeighborIndexesWithoutBorders(
                             _planetMap.Chunks.CurrentValue.GetLength(0), _planetMap.Chunks.CurrentValue.GetLength(1),
                             chunk.x, chunk.y))
                {
                    if (_planetMap.GetBiomeByChunk(nearIndex) == _planetMap.GetBiomeByChunk(chunk)) continue;

                    Vector2Int nearRelativePos = _planetMap.GetChunkRelativePosition(nearIndex, currentIndex);
                    Vector3 nearChunkPosition = currentChunkPos + new Vector3(nearRelativePos.x, 0, nearRelativePos.y) * _planetMap.ChunkSize.CurrentValue;
                    
                    float currentChunkDistance = nearRelativePos.sqrMagnitude;
                    
                    if (chunk.y == nearIndex.y 
                        && !(currentIsNearX && nearIndex != currentIndex) 
                        && (nearIndex == currentIndex || currentChunkDistance < minXDistance))
                    {
                        minXDistance = currentChunkDistance;
                        xBlendBorder = Mathf.Lerp(chunkPosition.x, nearChunkPosition.x, 0.5f);
                        if (nearIndex == currentIndex)
                            currentIsNearX = true;
                    }

                    if (chunk.x == nearIndex.x 
                        && !(currentIsNearY && nearIndex != currentIndex) 
                        && (nearIndex == currentIndex || currentChunkDistance < minYDistance))
                    {
                        minYDistance = currentChunkDistance;
                        yBlendBorder = Mathf.Lerp(chunkPosition.z, nearChunkPosition.z, 0.5f);
                        if (nearIndex == currentIndex)
                            currentIsNearY = true;
                    }
                }
            }
            
            Vector2Int biome = _planetMap.GetBiomeFromChunk(materialIndex);
            Vector2Int forwardNearChunkX = _planetMap.GetBiomeClampedIndex(biome + new Vector2Int(0, 1));
            Vector2Int forwardNearChunkY = _planetMap.GetBiomeClampedIndex(biome + new Vector2Int(1, 0));
            Vector2Int backNearChunkX = _planetMap.GetBiomeClampedIndex(biome + new Vector2Int(0, -1));
            Vector2Int backNearChunkY = _planetMap.GetBiomeClampedIndex(biome + new Vector2Int(-1, 0));
            
            Material grassMaterial = _planetMap.Biomes.CurrentValue[biome.x, biome.y].TerrainPrefab.GrassMaterial;
            Material borderXGrassMaterial = _planetMap.Biomes.CurrentValue[forwardNearChunkX.x, forwardNearChunkX.y].TerrainPrefab.GrassMaterial;
            Material borderYGrassMaterial = _planetMap.Biomes.CurrentValue[forwardNearChunkY.x, forwardNearChunkY.y].TerrainPrefab.GrassMaterial;
            Material borderXBackGrassMaterial = _planetMap.Biomes.CurrentValue[backNearChunkX.x, backNearChunkX.y].TerrainPrefab.GrassMaterial;
            Material borderYBackGrassMaterial = _planetMap.Biomes.CurrentValue[backNearChunkY.x, backNearChunkY.y].TerrainPrefab.GrassMaterial;
            
            grassMaterial.SetFloat(_grassBorderXProperty, xBlendBorder);
            grassMaterial.SetFloat(_grassBorderYProperty, yBlendBorder);
            
            grassMaterial.SetColor(_grassFarColorXForwardProperty, borderXGrassMaterial.GetColor(_grassFarColorProperty));
            grassMaterial.SetColor(_grassNearColorXForwardProperty, borderXGrassMaterial.GetColor(_grassNearColorProperty));
            grassMaterial.SetColor(_grassShadowColorXForwardProperty, borderXGrassMaterial.GetColor(_grassShadowColorProperty));
            grassMaterial.SetColor(_grassBottomColorXForwardProperty, borderXGrassMaterial.GetColor(_grassBottomColorProperty));
            
            grassMaterial.SetColor(_grassFarColorYForwardProperty, borderYGrassMaterial.GetColor(_grassFarColorProperty));
            grassMaterial.SetColor(_grassNearColorYForwardProperty, borderYGrassMaterial.GetColor(_grassNearColorProperty));
            grassMaterial.SetColor(_grassShadowColorYForwardProperty, borderYGrassMaterial.GetColor(_grassShadowColorProperty));
            grassMaterial.SetColor(_grassBottomColorYForwardProperty, borderYGrassMaterial.GetColor(_grassBottomColorProperty));
            
            grassMaterial.SetColor(_grassFarColorXBackProperty, borderXBackGrassMaterial.GetColor(_grassFarColorProperty));
            grassMaterial.SetColor(_grassNearColorXBackProperty, borderXBackGrassMaterial.GetColor(_grassNearColorProperty));
            grassMaterial.SetColor(_grassShadowColorXBackProperty, borderXBackGrassMaterial.GetColor(_grassShadowColorProperty));
            grassMaterial.SetColor(_grassBottomColorXBackProperty, borderXBackGrassMaterial.GetColor(_grassBottomColorProperty));
            
            grassMaterial.SetColor(_grassFarColorYBackProperty, borderYBackGrassMaterial.GetColor(_grassFarColorProperty));
            grassMaterial.SetColor(_grassNearColorYBackProperty, borderYBackGrassMaterial.GetColor(_grassNearColorProperty));
            grassMaterial.SetColor(_grassShadowColorYBackProperty, borderYBackGrassMaterial.GetColor(_grassShadowColorProperty));
            grassMaterial.SetColor(_grassBottomColorYBackProperty, borderYBackGrassMaterial.GetColor(_grassBottomColorProperty));
            
            
            Material terrainMaterial = _planetMap.Biomes.CurrentValue[biome.x, biome.y].TerrainPrefab.TerrainMaterial;
            Material borderXTerrainMaterial = _planetMap.Biomes.CurrentValue[forwardNearChunkX.x, forwardNearChunkX.y].TerrainPrefab.TerrainMaterial;
            Material borderYTerrainMaterial = _planetMap.Biomes.CurrentValue[forwardNearChunkY.x, forwardNearChunkY.y].TerrainPrefab.TerrainMaterial;
            Material borderXBackTerrainMaterial = _planetMap.Biomes.CurrentValue[backNearChunkX.x, backNearChunkX.y].TerrainPrefab.TerrainMaterial;
            Material borderYBackTerrainMaterial = _planetMap.Biomes.CurrentValue[backNearChunkY.x, backNearChunkY.y].TerrainPrefab.TerrainMaterial;
            
            terrainMaterial.SetFloat(_terrainBorderXProperty, xBlendBorder);
            terrainMaterial.SetFloat(_terrainBorderYProperty, yBlendBorder);
            
            terrainMaterial.SetColor(_terrainMainColorXProperty, borderXTerrainMaterial.GetColor(_terrainMainColorProperty));
            terrainMaterial.SetColor(_terrainShadowColorXProperty, borderXTerrainMaterial.GetColor(_terrainShadowColorProperty));
            
            terrainMaterial.SetColor(_terrainMainColorYProperty, borderYTerrainMaterial.GetColor(_terrainMainColorProperty));
            terrainMaterial.SetColor(_terrainShadowColorYProperty, borderYTerrainMaterial.GetColor(_terrainShadowColorProperty));
            
            terrainMaterial.SetColor(_terrainMainColorXBackProperty, borderXBackTerrainMaterial.GetColor(_terrainMainColorProperty));
            terrainMaterial.SetColor(_terrainShadowColorXBackProperty, borderXBackTerrainMaterial.GetColor(_terrainShadowColorProperty));
            
            terrainMaterial.SetColor(_terrainMainColorYBackProperty, borderYBackTerrainMaterial.GetColor(_terrainMainColorProperty));
            terrainMaterial.SetColor(_terrainShadowColorYBackProperty, borderYBackTerrainMaterial.GetColor(_terrainShadowColorProperty));
        }
    }
}