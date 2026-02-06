using System.Linq;
using Game.Runtime.Array2D;
using Game.Runtime.PlanetSystem.Configs;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Runtime.Plugins.Array2D
{
    [System.Serializable]
    public class Array2DBiome : Array2D<BiomeConfig>
    {
        [SerializeField]
        CellRowBiome[] cells = new CellRowBiome[Consts.defaultGridSize];

        protected override CellRow<BiomeConfig> GetCellRow(int idx)
        {
            return cells[idx];
        }

        /// <inheritdoc cref="Array2D{T}.Clone"/>
        protected override Array2D<BiomeConfig> Clone(CellRow<BiomeConfig>[] cells)
        {
            return new Array2DBiome() { cells = cells.Select(r => new CellRowBiome(r)).ToArray() };
        }
    }
    
    [System.Serializable]
    public class CellRowBiome : CellRow<BiomeConfig>
    {
        /// <inheritdoc/>
        [UsedImplicitly]
        public CellRowBiome() { }

        /// <inheritdoc/>
        public CellRowBiome(CellRow<BiomeConfig> row)
            : base(row) { }
    }
}