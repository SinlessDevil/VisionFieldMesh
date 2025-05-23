using System.Collections.Generic;
using Plugins.ConcaveHull.Code;
using UnityEngine;

namespace Code.Editors.ColliderMeshCreator
{
    public static class EdgeOutlineBuilder
    {
        private const float Epsilon = 0.001f;

        public static List<Vector3> BuildOutline(this List<Line> unorderedEdges)
        {
            List<Vector3> outline = new List<Vector3>();
            if (unorderedEdges == null || unorderedEdges.Count == 0) return outline;

            List<Line> edges = new List<Line>(unorderedEdges);
            Line current = edges[0];
            edges.RemoveAt(0);

            Vector2 start = new Vector2((float)current.Nodes[0].X, (float)current.Nodes[0].Y);
            Vector2 end = new Vector2((float)current.Nodes[1].X, (float)current.Nodes[1].Y);

            outline.Add(new Vector3(start.x, 0, start.y));
            outline.Add(new Vector3(end.x, 0, end.y));

            while (edges.Count > 0)
            {
                Vector2 last = new Vector2(outline[^1].x, outline[^1].z);

                int index = edges.FindIndex(e =>
                    Vector2.Distance(new Vector2((float)e.Nodes[0].X, (float)e.Nodes[0].Y), last) < Epsilon ||
                    Vector2.Distance(new Vector2((float)e.Nodes[1].X, (float)e.Nodes[1].Y), last) < Epsilon);

                if (index == -1) 
                    break;

                Line edge = edges[index];
                edges.RemoveAt(index);

                Node nextNode = Vector2.Distance(new Vector2((float)edge.Nodes[0].X, (float)edge.Nodes[0].Y), last) < Epsilon
                    ? edge.Nodes[1]
                    : edge.Nodes[0];

                outline.Add(new Vector3((float)nextNode.X, 0, (float)nextNode.Y));
            }

            return outline;
        }
        
        public static List<Vector3> SmoothOutlineCatmullRom(this List<Vector3> points, int segmentsPerCurve = 5)
        {
            List<Vector3> result = new();

            int count = points.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 p0 = points[(i - 1 + count) % count];
                Vector3 p1 = points[i];
                Vector3 p2 = points[(i + 1) % count];
                Vector3 p3 = points[(i + 2) % count];

                for (int j = 0; j <= segmentsPerCurve; j++)
                {
                    float t = j / (float)segmentsPerCurve;
                    Vector3 point = 0.5f * (
                        2f * p1 +
                        (p2 - p0) * t +
                        (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                        (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
                    result.Add(point);
                }
            }

            return result;
        }
    }
}