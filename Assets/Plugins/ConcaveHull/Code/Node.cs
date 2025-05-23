namespace Plugins.ConcaveHull.Code
{
    public class Node
    {
        public int Id;
        public double X;
        public double Y;

        public Node(double x, double y, int id)
        {
            X = x;
            Y = y;
            Id = id;
        }
    }
}