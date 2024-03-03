using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapePostcard
{
    class Landscape
    {
        static Random rnd = new Random();
        public List<lPoint> pArr;

        public Landscape(int X, int Y)
        {
            // Create framing rectangle.
            pArr.Add(new lPoint(0, 0));
            pArr.Add(new lPoint(X, 0));
            pArr.Add(new lPoint(0, Y));
            pArr.Add(new lPoint(X, Y));
            int num = rnd.Next(5, 15);

            // Creating set of lPoint.
            for (int i = 0; i < num; i++)
            {
                double layer = rnd.NextDouble();

            }
        }
    }

    public class lPoint
    {
        /// <summary>
        /// Used only for generating a unique ID for each instance of this class that gets generated
        /// </summary>
        private static int _counter;

        /// <summary>
        /// Used for identifying an instance of a class; can be useful in troubleshooting when geometry goes weird
        /// (e.g. when trying to identify when Triangle objects are being created with the same Point object twice)
        /// </summary>
        private readonly int _instanceId = _counter++;

        public double X { get; set; }
        public double Y { get; set; }
        public int Z;
        public HashSet<Triangle> AdjacentTriangles { get; } = new HashSet<Triangle>();

        public lPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            // Simple way of seeing what's going on in the debugger when investigating weirdness
            return $"{nameof(Point)} {_instanceId} {X:0.##}@{Y:0.##}";
        }
    }

    public class TriangleRef
    {
        public int tr;
        List<int> dependencies;
        List<int> obstacleToThese;
        public bool isOneSide;

        public void CreateDependencies(ref List<Triangle> arr)
        {
            // Sorting vertices so that [0] is the left one
            // [2] is the right one and both of them are the lowest possible
            // lowest == highest in terms of coordinates.


            for (int i = 0; i < tr; i++)
            {

            }
            if (tr < arr.Count() - 1)
                for (int i = tr + 1; i < arr.Count(); i++)
                {

                }
        }

        public TriangleRef(int tr)
        {
            this.tr = tr;
        }
    }

    public class Triangle
    {
        // CHANGED GETTER TO FIELD.
        public lPoint[] Vertices = new lPoint[3];
        public lPoint Circumcenter { get; private set; }
        public double RadiusSquared;
        public Color clr;
        public bool isOneSide;
        public int depInd;
        public int depOrObs;
        public int obsInd;
        public int seq;

        public void SortVertices()
        {
            for (int i = 1; i < 3; i++)
            {
                if (Vertices[0].X > Vertices[i].X ||
                (Vertices[0].X == Vertices[i].X &&
                Vertices[0].Y > Vertices[i].Y))
                {
                    lPoint temp = Vertices[0];
                    Vertices[0] = Vertices[i];
                    Vertices[i] = temp;
                }
                if (Vertices[2].X < Vertices[i - 1].X ||
                (Vertices[2].X == Vertices[i - 1].X &&
                Vertices[2].Y > Vertices[i - 1].Y))
                {
                    lPoint temp = Vertices[2];
                    Vertices[2] = Vertices[i - 1];
                    Vertices[i - 1] = temp;
                }
            }

            if ((Vertices[1].X - Vertices[0].X) *
                (Vertices[2].Y - Vertices[1].Y)
                - (Vertices[1].Y - Vertices[0].Y) *
                (Vertices[2].X - Vertices[1].X) > 0)
                isOneSide = false;
            else isOneSide = true;
        }

        public IEnumerable<Triangle> TrianglesWithSharedEdge
        {
            get
            {
                var neighbors = new HashSet<Triangle>();
                foreach (var vertex in Vertices)
                {
                    var trianglesWithSharedEdge = vertex.AdjacentTriangles.Where(o =>
                    {
                        return o != this && SharesEdgeWith(o);
                    });
                    neighbors.UnionWith(trianglesWithSharedEdge);
                }

                return neighbors;
            }
        }

        public Triangle()
        {
            depInd = -1;
            obsInd = -1;
            depOrObs = -1;
            seq = -1;
        }
        public Triangle(lPoint point1, lPoint point2, lPoint point3)
        {
            depInd = -1;
            obsInd = -1;
            depOrObs = -1;
            seq = -1;
            // In theory this shouldn't happen, but it was at one point so this at least makes sure we're getting a
            // relatively easily-recognised error message, and provides a handy breakpoint for debugging.
            if (point1 == point2 || point1 == point3 || point2 == point3)
            {
                throw new ArgumentException("Must be 3 distinct points");
            }

            if (!IsCounterClockwise(point1, point2, point3))
            {
                Vertices[0] = point1;
                Vertices[1] = point3;
                Vertices[2] = point2;
            }
            else
            {
                Vertices[0] = point1;
                Vertices[1] = point2;
                Vertices[2] = point3;
            }

            Vertices[0].AdjacentTriangles.Add(this);
            Vertices[1].AdjacentTriangles.Add(this);
            Vertices[2].AdjacentTriangles.Add(this);
            UpdateCircumcircle();
        }

        private void UpdateCircumcircle()
        {
            // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
            // https://en.wikipedia.org/wiki/Circumscribed_circle
            var p0 = Vertices[0];
            var p1 = Vertices[1];
            var p2 = Vertices[2];
            var dA = p0.X * p0.X + p0.Y * p0.Y;
            var dB = p1.X * p1.X + p1.Y * p1.Y;
            var dC = p2.X * p2.X + p2.Y * p2.Y;

            var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
            var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
            var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

            if (div == 0)
            {
                throw new DivideByZeroException();
            }

            var center = new lPoint(aux1 / div, aux2 / div);
            Circumcenter = center;
            RadiusSquared = (center.X - p0.X) * (center.X - p0.X) + (center.Y - p0.Y) * (center.Y - p0.Y);
        }

        private bool IsCounterClockwise(lPoint point1, lPoint point2, lPoint point3)
        {
            var result = (point2.X - point1.X) * (point3.Y - point1.Y) -
                (point3.X - point1.X) * (point2.Y - point1.Y);
            return result > 0;
        }

        public bool SharesEdgeWith(Triangle triangle)
        {
            var sharedVertices = Vertices.Where(o => triangle.Vertices.Contains(o)).Count();
            return sharedVertices == 2;
        }

        public bool IsPointInsideCircumcircle(lPoint point)
        {
            var d_squared = (point.X - Circumcenter.X) * (point.X - Circumcenter.X) +
                (point.Y - Circumcenter.Y) * (point.Y - Circumcenter.Y);
            return d_squared < RadiusSquared;
        }
    }

    public class Edge
    {
        public lPoint Point1 { get; }
        public lPoint Point2 { get; }

        public Edge(lPoint point1, lPoint point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var edge = obj as Edge;

            var samePoints = Point1 == edge.Point1 && Point2 == edge.Point2;
            var samePointsReversed = Point1 == edge.Point2 && Point2 == edge.Point1;
            return samePoints || samePointsReversed;
        }

        public override int GetHashCode()
        {
            int hCode = (int)Point1.X ^ (int)Point1.Y ^ (int)Point2.X ^ (int)Point2.Y;
            return hCode.GetHashCode();
        }
    }

    public class DelaunayTriangulator
    {
        private double MaxX { get; set; }
        private double MaxY { get; set; }
        private IEnumerable<Triangle> border;

        public IEnumerable<lPoint> GeneratePoints(int amount, double maxX, double maxY)
        {
            MaxX = maxX;
            MaxY = maxY;

            // TODO make more beautiful
            var point0 = new lPoint(0, 0);
            var point1 = new lPoint(0, MaxY);
            var point2 = new lPoint(MaxX, MaxY);
            var point3 = new lPoint(MaxX, 0);
            var points = new List<lPoint>() { point0, point1, point2, point3 };
            var tri1 = new Triangle(point0, point1, point2);
            var tri2 = new Triangle(point0, point2, point3);
            border = new List<Triangle>() { tri1, tri2 };

            var random = new Random();
            for (int i = 0; i < amount - 4; i++)
            {
                var pointX = random.NextDouble() * MaxX;
                var pointY = random.NextDouble() * MaxY;
                points.Add(new lPoint(pointX, pointY));
            }

            return points;
        }

        public IEnumerable<Triangle> BowyerWatson(IEnumerable<lPoint> points)
        {
            //var supraTriangle = GenerateSupraTriangle();
            var triangulation = new HashSet<Triangle>(border);

            foreach (var point in points)
            {
                var badTriangles = FindBadTriangles(point, triangulation);
                var polygon = FindHoleBoundaries(badTriangles);

                foreach (var triangle in badTriangles)
                {
                    foreach (var vertex in triangle.Vertices)
                    {
                        vertex.AdjacentTriangles.Remove(triangle);
                    }
                }
                triangulation.RemoveWhere(o => badTriangles.Contains(o));

                foreach (var edge in polygon.Where(possibleEdge => possibleEdge.Point1 != point && possibleEdge.Point2 != point))
                {
                    var triangle = new Triangle(point, edge.Point1, edge.Point2);
                    triangulation.Add(triangle);
                }
            }

            //triangulation.RemoveWhere(o => o.Vertices.Any(v => supraTriangle.Vertices.Contains(v)));
            return triangulation;
        }

        private List<Edge> FindHoleBoundaries(ISet<Triangle> badTriangles)
        {
            var edges = new List<Edge>();
            foreach (var triangle in badTriangles)
            {
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
            }
            var grouped = edges.GroupBy(o => o);
            var boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
            return boundaryEdges.ToList();
        }

        private Triangle GenerateSupraTriangle()
        {
            //   1  -> maxX
            //  / \
            // 2---3
            // |
            // v maxY
            var margin = 500;
            var point1 = new lPoint(0.5 * MaxX, -2 * MaxX - margin);
            var point2 = new lPoint(-2 * MaxY - margin, 2 * MaxY + margin);
            var point3 = new lPoint(2 * MaxX + MaxY + margin, 2 * MaxY + margin);
            return new Triangle(point1, point2, point3);
        }

        private ISet<Triangle> FindBadTriangles(lPoint point, HashSet<Triangle> triangles)
        {
            var badTriangles = triangles.Where(o => o.IsPointInsideCircumcircle(point));
            return new HashSet<Triangle>(badTriangles);
        }
    }
}
