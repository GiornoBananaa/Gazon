using Game.Runtime.TerrainChunkSystem;
using Game.Runtime.Utils;
using UnityEngine;

namespace Game.Runtime.ObjectCullingSystem.CullingRules
{
    public class RoundWorldViewCullingRule : IObjectCullingRule
    {
        public bool IsCullObject(Camera camera, ObjectCullingTarget target)
        {
            Bounds targetBounds = new Bounds(target.Transform.position + target.LocalBounds.center, target.LocalBounds.size);
            
            Vector3 p1 = camera.transform.position;
            if (targetBounds.Contains(new Vector3(p1.x, targetBounds.center.y, p1.z)))
            {
                return true;
            }
            
            Vector3 p2 = MathUtils.ConvertToRoundWorldPosition(targetBounds.max, p1);
            float maxHeight = p2.y;

            Vector3 heightPoint2 = MathUtils.ConvertToRoundWorldPosition(new Vector3(targetBounds.max.x, targetBounds.max.y, targetBounds.min.z), p1);
            Vector3 heightPoint3 = MathUtils.ConvertToRoundWorldPosition(new Vector3(targetBounds.min.x, targetBounds.max.y, targetBounds.max.z), p1);
            Vector3 heightPoint4 = MathUtils.ConvertToRoundWorldPosition(new Vector3(targetBounds.min.x, targetBounds.max.y, targetBounds.min.z), p1);
            
            if (heightPoint2.y > maxHeight)
            {
                p2 = heightPoint2;
                maxHeight = heightPoint2.y;
            }
            if (heightPoint3.y > maxHeight)
            {
                p2 = heightPoint3;
                maxHeight = heightPoint3.y;
            }
            if (heightPoint4.y > maxHeight)
            {
                p2 = heightPoint4;
                maxHeight = heightPoint4.y;
            }
            
            Vector3 worldPivot = p1;
            worldPivot.y = 0;
            p1 -= worldPivot;
            p2 -= worldPivot;

            float x1 = 0;
            float y1 = p1.y;
            float x2 = new Vector3(p2.x, 0, p2.z).magnitude;
            float y2 = p2.y;
            
            float a = -GlobalConstants.ROUND_WORLD_VALUE;
            
            // Coefficients for the quadratic equation At^2 + Bt + C = 0
            // Derived from substituting line equation into parabola equation
            float A = a * (x2 - x1) * (x2 - x1);
            float B = 2 * a * x1 * (x2 - x1) + 0 * (x2 - x1) - (y2 - y1);
            float C = a * x1 * x1 + 0 * x1 + 0 - y1;

            // Calculate the discriminant
            float discriminant = B * B - 4 * A * C;

            if (discriminant < 0)
            {
                // No real intersection points with the infinite line
                return true;
            }

            // Calculate the two possible t values
            float t1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
            float t2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

            // Check if either t value falls within the line segments range [0, 1]
            bool intersectsSegment = (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);

            if (!intersectsSegment)
            {
                return true;
            }
            
            return false;
        }
    }
}