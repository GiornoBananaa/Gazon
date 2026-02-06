using System.Collections.Generic;
using Game.Runtime.CameraSystem;
using Game.Runtime.Configs;
using Game.Runtime.PlanetSystem;
using Game.Runtime.PlanetSystem.Configs;
using Game.Runtime.PlanetSystem.Generation;
using Game.Runtime.PlanetSystem.Movement;
using Game.Runtime.WeatherFeature;
using Game.Runtime.WeatherSystem;
using Game.Runtime.WeatherSystem.WeatherTween;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Runtime.PlanetFeature
{
    public class PlanetController : MonoBehaviour
    {
        [Inject] private readonly IPlanetGenerator _planetGenerator;
        [Inject] private readonly PlanetMap _planetMap;
        [Inject] private readonly PlanetMover _planetMover;
        [Inject] private readonly TerrainBiomeBlender _terrainBiomeBlender;
        [Inject] private readonly ICurrentCamera _camera;
        [Inject] private readonly WeatherPropertyBlender _weatherPropertyBlender;
        
        private WeatherBiomeSetter _weatherBiomeSetter;
        
        private void Awake()
        {
            InitializePlanet();
        }

        private void InitializePlanet()
        {
            var biomesMap = RootConfig.Instance.PlanetConfig.Biomes;
            
            Biome[,] biomes = new Biome[biomesMap.GridSize.x, biomesMap.GridSize.y];
            
            Dictionary<Biome, WeatherState> biomeWeatherStates = new Dictionary<Biome, WeatherState>();
            
            Dictionary<BiomeConfig, Biome> biomeInstances = new Dictionary<BiomeConfig, Biome>();
            
            for (int x = 0; x < biomesMap.GridSize.x; x++)
            {
                for (int y = 0; y < biomesMap.GridSize.y; y++)
                {
                    var biomeConfig = biomesMap[x, y];
                    
                    if (!biomeInstances.ContainsKey(biomeConfig))
                    {
                        var biome = new Biome(biomeConfig.TerrainPrefab);
                        biomeInstances.Add(biomeConfig, biome);
                        biomeWeatherStates[biome] = biomeConfig.WeatherState;
                    }
                        
                    biomes[x, y] = biomeInstances[biomesMap[x, y]];
                }
            }
            
            float biomeSize = RootConfig.Instance.PlanetConfig.BiomeSize;
            float biomeSizeInChunks = RootConfig.Instance.PlanetConfig.BiomeSizeInChunks;
            float chunkSize = biomeSize / biomeSizeInChunks;
            
            TerrainChunk[,] chunks = _planetGenerator.Generate(biomes, biomeSize, chunkSize);
            
            foreach (var chunk in chunks)
            {
                chunk.transform.parent = transform;
            }
            
            _planetMap.SetMapData(biomes, chunks, biomeSize, chunkSize);
            
            
            float biomeBlendDistance = RootConfig.Instance.PlanetConfig.ChunkWeatherBlendDistance;
            _weatherBiomeSetter = new WeatherBiomeSetter(_weatherPropertyBlender, _planetMap, _camera, biomeWeatherStates, biomeBlendDistance);
        }
        
        public void Update()
        {
            _planetMap.UpdatePlayerLocation();
            _weatherBiomeSetter.UpdateBiomeWeather();
            _planetMover.Update();
        }
    }
}