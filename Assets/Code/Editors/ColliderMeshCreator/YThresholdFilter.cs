using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Editors.ColliderMeshCreator
{
    public static class YThresholdFilter
    {
        public static List<Vector3> FilterTopPoints(List<Vector3> points, float thresholdPercent)
        {
            float minY = points.Min(p => p.y);
            float maxY = points.Max(p => p.y);
            float thresholdY = Mathf.Lerp(maxY, minY, thresholdPercent);
            
            return points
                .Where(p => p.y >= thresholdY)
                .Select(p => new Vector3(p.x, 0, p.z))
                .ToList();
        }
    }
}