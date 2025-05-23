using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.ConcaveHull.Code
{
    public static class HullFunctions
    {
        /// <summary>
        /// Attempts to divide a line into two new lines using nearby points,
        /// if the angle at the point meets the concavity threshold and the new lines do not intersect the hull.
        /// </summary>
        public static List<Line> GetDividedLine(Line line, List<Node> nearbyPoints, List<Line> concaveHull, double concavity)
        {
            Node bestPoint = null;
            double bestCos = double.MaxValue;

            foreach (var point in nearbyPoints)
            {
                double cos = GetCos(line.Nodes[0], line.Nodes[1], point);
                if (!(cos < concavity)) 
                    continue;
                Line lineA = new Line(line.Nodes[0], point);
                Line lineB = new Line(point, line.Nodes[1]);

                if (LineCollidesWithHull(lineA, concaveHull) ||
                    LineCollidesWithHull(lineB, concaveHull)) 
                    continue;
                
                if (!(cos < bestCos)) 
                    continue;
                
                bestCos = cos;
                bestPoint = point;
            }

            if (bestPoint != null)
            {
                return new List<Line>
                {
                    new(line.Nodes[0], bestPoint),
                    new(bestPoint, line.Nodes[1])
                };
            }

            return new List<Line>();
        }

        /// <summary>
        /// Checks whether a line intersects with any other lines in the current hull,
        /// ignoring cases where lines share a common node.
        /// </summary>
        private static bool LineCollidesWithHull(Line line, List<Line> concaveHull)
        {
            return concaveHull
                .Any(hullLine => !SharesNode(line, hullLine) && LineIntersectionFunctions
                    .DoIntersect(line.Nodes[0], line.Nodes[1], hullLine.Nodes[0], hullLine.Nodes[1]));
        }

        /// <summary>
        /// Determines if two lines share any node.
        /// </summary>
        private static bool SharesNode(Line a, Line b)
        {
            return a.Nodes[0].Id == b.Nodes[0].Id ||
                   a.Nodes[0].Id == b.Nodes[1].Id ||
                   a.Nodes[1].Id == b.Nodes[0].Id ||
                   a.Nodes[1].Id == b.Nodes[1].Id;
        }

        /// <summary>
        /// Calculates the cosine of the angle at point O formed by points A and B (A-O-B).
        /// A lower cosine value means a sharper inward angle.
        /// </summary>
        private static double GetCos(Node a, Node b, Node o)
        {
            double a2 = Math.Pow(a.X - o.X, 2) + Math.Pow(a.Y - o.Y, 2);
            double b2 = Math.Pow(b.X - o.X, 2) + Math.Pow(b.Y - o.Y, 2);
            double c2 = Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);

            double denominator = 2 * Math.Sqrt(a2 * b2);
            if (denominator == 0)
                return 1;

            return Math.Round((a2 + b2 - c2) / denominator, 4);
        }

        /// <summary>
        /// Returns points that fall inside an ellipse around the line,
        /// where scaleFactor controls the size of the ellipse.
        /// </summary>
        public static List<Node> GetNearbyPoints(Line line, List<Node> nodeList, double scaleFactor)
        {
            List<Node> nearbyPoints = new List<Node>();
            double lineLength = line.GetLength();
            double maxFocusSum = 2 * lineLength / Math.Sqrt(2) * scaleFactor;

            foreach (var node in nodeList)
            {
                double distA = GetDistance(line.Nodes[0], node);
                double distB = GetDistance(line.Nodes[1], node);

                if (distA + distB <= maxFocusSum)
                {
                    nearbyPoints.Add(node);
                }
            }

            return nearbyPoints;
        }

        /// <summary>
        /// Euclidean distance between two nodes.
        /// </summary>
        private static double GetDistance(Node a, Node b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
