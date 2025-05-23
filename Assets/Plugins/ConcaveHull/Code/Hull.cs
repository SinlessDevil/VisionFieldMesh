using System.Collections.Generic;
using System.Linq;

namespace Plugins.ConcaveHull.Code
{
    public static class Hull
    {
        private static readonly List<Node> _unusedNodes = new();
        private static readonly List<Line> _hullEdges = new();
        private static readonly List<Line> _hullConcaveEdges = new();

        public static IReadOnlyList<Line> ConvexEdges => _hullEdges;
        public static IReadOnlyList<Line> ConcaveEdges => _hullConcaveEdges;
        public static IReadOnlyList<Node> UnusedNodes => _unusedNodes;

        public static void SetConvexHull(List<Node> nodes)
        {
            _unusedNodes.Clear();
            _unusedNodes.AddRange(nodes);

            List<Node> convexHull = GrahamScan.ComputeConvexHull(nodes);
            _hullEdges.Clear();

            for (int i = 0; i < convexHull.Count; i++)
            {
                Node a = convexHull[i];
                Node b = convexHull[(i + 1) % convexHull.Count];
                _hullEdges.Add(new Line(a, b));
            }

            foreach (Node node in _hullEdges.SelectMany(line => line.Nodes))
            {
                _unusedNodes.RemoveAll(n => n.Id == node.Id);
            }
        }

        public static List<Line> SetConcaveHull(double concavity, double scaleFactor)
        {
            _hullConcaveEdges.Clear();
            _hullConcaveEdges.AddRange(_hullEdges);

            bool lineWasDivided;
            do
            {
                lineWasDivided = false;

                for (int i = 0; i < _hullConcaveEdges.Count; i++)
                {
                    Line line = _hullConcaveEdges[i];

                    List<Node> nearbyPoints = HullFunctions.GetNearbyPoints(line, _unusedNodes, scaleFactor);
                    List<Line> dividedLines = HullFunctions.GetDividedLine(line, nearbyPoints, _hullConcaveEdges, concavity);

                    if (dividedLines.Count > 0)
                    {
                        Node insertedNode = dividedLines[0].Nodes[1];
                        _unusedNodes.RemoveAll(n => n.Id == insertedNode.Id);

                        _hullConcaveEdges.RemoveAt(i);
                        _hullConcaveEdges.AddRange(dividedLines);

                        lineWasDivided = true;
                        break;
                    }
                }

                _hullConcaveEdges
                    .Sort((a, b) => Line
                        .GetLength(b.Nodes[0], b.Nodes[1])
                        .CompareTo(Line.GetLength(a.Nodes[0], a.Nodes[1])));

            } while (lineWasDivided);

            return _hullConcaveEdges;
        }

        public static void CleanUp()
        {
            _unusedNodes.Clear();
            _hullEdges.Clear();
            _hullConcaveEdges.Clear();
        }
    }
}
