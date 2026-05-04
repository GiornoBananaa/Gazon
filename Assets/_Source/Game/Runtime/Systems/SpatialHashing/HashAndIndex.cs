using System;

namespace Game.Runtime.SpatialHashing
{
    public struct HashAndIndex : IComparable<HashAndIndex> 
    {
        public int Hash;
        public int Index;
            
        public int CompareTo(HashAndIndex other) 
        {
            return Hash.CompareTo(other.Hash);
        }
    }
}