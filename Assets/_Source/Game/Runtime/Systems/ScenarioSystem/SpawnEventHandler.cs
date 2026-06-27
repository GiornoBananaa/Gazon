using System.Collections.Generic;
using Game.Runtime.TerrainChunkSystem;
using Game.Runtime.Utils;
using UnityEngine;

namespace Game.Runtime.ScenarioSystem
{
    public class SpawnEventHandler : IScenarioEventHandler
    {
        private readonly PlanetMap _planetMap;
        private Dictionary<int, GameObject> _gameObjects = new();
        
        public EventType EventType => EventType.Spawn;

        public SpawnEventHandler(PlanetMap planetMap)
        {
            _planetMap = planetMap;
        }
        
        public void StartEvent(int eventIndex, string value)
        {
            SpawnEventData spawnEventData;
            try
            {
                spawnEventData = JsonUtility.FromJson<SpawnEventData>(value);
            }
            catch
            {
                return;
            }
            GameObject gameObject = Object.Instantiate(spawnEventData.Prefab, _planetMap.Chunks.CurrentValue[spawnEventData.Chunk.x, spawnEventData.Chunk.y].transform.position + spawnEventData.LocalPosition.GetVectorXZ(), Quaternion.identity);
            _gameObjects.Add(eventIndex, gameObject);
        }

        public void EndEvent(int eventIndex, string value)
        {
            SpawnEventData spawnEventData;
            try
            {
                spawnEventData = JsonUtility.FromJson<SpawnEventData>(value);
            }
            catch
            {
                return;
            }
            if (!_gameObjects.TryGetValue(eventIndex, out GameObject gameObject)) return;
            Object.Destroy(gameObject);
        }
    }
}