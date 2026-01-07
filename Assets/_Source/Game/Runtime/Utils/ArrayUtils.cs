using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Utils
{
    public static class ArrayUtils
    {
        public static IEnumerable<T> GetNeighborsWithoutBorders<T>(T[,] array, int row, int col, bool includeCenter = false)
        {
            foreach (var index in GetNeighborIndexesWithoutBorders(array.GetLength(0), array.GetLength(1), row, col))
            {
                yield return array[index.x, index.y];
            }
        }

        public static IEnumerable<Vector2Int> GetNeighborIndexesWithoutBorders(int boundX, int boundY, int row, int col, bool includeCenter = false)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!includeCenter && i == 0 && j == 0)
                        continue;

                    yield return GetIndexWithoutBorders(boundX, boundY, row + i, col + j);
                }
            }
        }
        
        public static Vector2Int GetIndexWithoutBorders<T>(T[,] array, int row, int col)
        {
            return GetIndexWithoutBorders(array.GetLength(0), array.GetLength(1), row, col);
        }
        public static Vector2Int GetIndexWithoutBorders(int boundX, int boundY, int row, int col)
        {
            return new Vector2Int((row < 0 ? boundX + row : row) % boundX, (col < 0 ? boundY + col : col) % boundY);
        }
    }
}