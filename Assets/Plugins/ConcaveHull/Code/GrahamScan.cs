using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.ConcaveHull.Code
{
    public static class GrahamScan
    {
        private const int TurnLeft = 1;

        private static int Turn(Node p, Node q, Node r) =>
            ((q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y))
            .CompareTo(0);

        private static void KeepLeft(List<Node> hull, Node point)
        {
            while (hull.Count > 1 && Turn(hull[^2], hull[^1], point) != TurnLeft)
                hull.RemoveAt(hull.Count - 1);

            if (hull.Count == 0 || hull[^1] != point)
                hull.Add(point);
        }

        private static double AngleFromBase(Node origin, Node target)
        {
            double dx = target.X - origin.X;
            double dy = target.Y - origin.Y;
            return Math.Atan2(dy, dx);
        }

        public static List<Node> ComputeConvexHull(List<Node> points)
        {
            if (points == null || points.Count < 3)
                throw new ArgumentException("At least 3 points are required to compute a convex hull.");
            
            Node p0 = FindLowestPoint(points);
            
            List<Node> sorted = points
                .Where(p => p != p0)
                .OrderBy(p => AngleFromBase(p0, p))
                .ToList();

            List<Node> hull = new List<Node> { p0 };
            foreach (var point in sorted)
                KeepLeft(hull, point);

            return hull;
        }
        
        private static Node FindLowestPoint(List<Node> points)
        {
            Node lowest = points[0];
            foreach (var p in points)
            {
                if (p.Y < lowest.Y || (p.Y == lowest.Y && p.X < lowest.X))
                {
                    lowest = p;
                }
            }
            return lowest;
        }
    }
}