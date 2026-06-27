using System;
using UnityEngine;

namespace Game.Runtime.ScenarioSystem
{
    [Serializable]
    public class SpawnEventData
    {
        public GameObject Prefab;
        public Vector2Int Chunk;
        public Vector2 LocalPosition;
    }
}