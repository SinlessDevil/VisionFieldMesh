using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Editors.ColliderMeshCreator
{
    public static class MeshPointCollector
    {
        public static List<Vector3> CollectWorldPoints(List<MeshFilter> filters)
        {
            List<Vector3> points = new List<Vector3>();

            foreach (MeshFilter filter in filters)
            {
                if (filter == null || filter.sharedMesh == null) 
                    continue;
                
                Matrix4x4 matrix = filter.transform.localToWorldMatrix;
                points
                    .AddRange(filter.sharedMesh.vertices
                    .Select(v => matrix.MultiplyPoint3x4(v)));
            }

            return points;
        }
    }
}