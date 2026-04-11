using Game.Runtime.Configs;
using Game.Runtime.Plugins.Array2D;
using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DBiome))]
    public class Array2DWeatherDrawer : Array2DObjectDrawer<BiomeConfig> { }
}