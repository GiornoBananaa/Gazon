using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Runtime.SpatialHashing
{
    [BurstCompile]
    public static class SpatialHashUtils
    {
        public static int Hash(int3 gridPos) 
        {
            unchecked 
            {
                return gridPos.x * 73856093 ^ gridPos.y * 19349663 ^ gridPos.z * 83492791;
            }
        }

        public static int3 GridPosition(float3 position, float cellSize) 
        {
            return new int3(math.floor(position / cellSize));
        }
        
        public static int BinarySearchFirst(NativeArray<HashAndIndex> array, int hash) 
        {
            int left = 0;
            int right = array.Length - 1;
            int result = -1;
                
            while (left <= right) 
            {
                int mid = (left + right) / 2;
                int midHash = array[mid].Hash;

                if (midHash == hash) 
                {
                    result = mid;
                    right = mid - 1;
                } 
                else if (midHash < hash) 
                {
                    left = mid + 1;
                } 
                else 
                {
                    right = mid - 1;
                }
            }
               
            return result;
        }
    }
}