using UnityEngine;

namespace Game.Runtime.WeatherSystem.Configs
{
    [CreateAssetMenu(fileName = "WeatherAssets", menuName = "Game/Weather/WeatherAssets")]
    public class WeatherAssetsConfig : ScriptableObject
    {
        public Material[] GrassMaterial;
        public Material[] SkyMaterial;
    }
}