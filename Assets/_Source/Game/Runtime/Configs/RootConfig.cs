using Game.Runtime.PlanetSystem.Configs;
using Game.Runtime.WeatherSystem.Configs;
using UnityEngine;

namespace Game.Runtime.Configs
{
    [CreateAssetMenu(fileName = "WeatherAssetsConfig", menuName = "Game/RootConfig")]
    public class RootConfig : ScriptableObject
    {
        private static RootConfig _instance;
        
        public static RootConfig Instance
        {
            get
            {
                if (!_instance) _instance = Resources.Load<RootConfig>("RootConfig");
                return _instance;
            }
        }
        
        
        public WeatherAssetsConfig WeatherAssets;
        public PlanetConfig PlanetConfig;
    }
}