using System;

namespace Plugins.ConcaveHull.Code
{
    public class Line
    {
        public Node Start { get; }
        public Node End { get; }

        public Line(Node start, Node end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Returns the length of this line segment.
        /// </summary>
        public double GetLength() => GetLength(Start, End);

        /// <summary>
        /// Returns the Euclidean distance between two nodes.
        /// </summary>
        public static double GetLength(Node a, Node b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        
        public Node[] Nodes => new[] { Start, End };
    }
}