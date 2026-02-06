using Game.Runtime.Array2D.Array2DTypes.ObjectTypes;
using UnityEngine;
using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DSprite))]
    public class Array2DSpriteDrawer : Array2DObjectDrawer<Sprite> { }
}
