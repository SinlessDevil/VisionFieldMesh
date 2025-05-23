using System;

namespace Plugins.ConcaveHull.Code
{
    public static class LineIntersectionFunctions
    {
        /// <summary>
        /// Returns true if line segments (p1,q1) and (p2,q2) intersect.
        /// </summary>
        public static bool DoIntersect(Node p1, Node q1, Node p2, Node q2)
        {
            int o1 = GetOrientation(p1, q1, p2);
            int o2 = GetOrientation(p1, q1, q2);
            int o3 = GetOrientation(p2, q2, p1);
            int o4 = GetOrientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special cases
            if (o1 == 0 && IsOnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && IsOnSegment(p1, q2, q1)) return true;
            if (o3 == 0 && IsOnSegment(p2, p1, q2)) return true;
            if (o4 == 0 && IsOnSegment(p2, q1, q2)) return true;

            return false;
        }

        /// <summary>
        /// Returns true if point q lies on the line segment (p, r), assuming all three are colinear.
        /// </summary>
        private static bool IsOnSegment(Node p, Node q, Node r)
        {
            return q.X >= Math.Min(p.X, r.X) && q.X <= Math.Max(p.X, r.X) &&
                   q.Y >= Math.Min(p.Y, r.Y) && q.Y <= Math.Max(p.Y, r.Y);
        }

        /// <summary>
        /// Computes the orientation of the ordered triplet (p, q, r):
        /// 0 --> colinear, 1 --> clockwise, 2 --> counterclockwise.
        /// </summary>
        private static int GetOrientation(Node p, Node q, Node r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (Math.Abs(val) < 1e-9)
                return 0; // colinear

            return (val > 0) ? 1 : 2; // 1 = clockwise, 2 = counterclockwise
        }
    }
}