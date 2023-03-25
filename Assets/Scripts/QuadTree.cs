public sealed class QuadTree
{
    public uint Capacity { get; private set; }
    public Point[] Points;
    public Rectangle Boundary;
    public bool IsSubdivided { get; private set; } = false;
    public uint Depth { get; private set; }
    public QuadTree Northwest { get; private set; }
    public QuadTree Northeast { get; private set; }
    public QuadTree Southwest { get; private set; }
    public QuadTree Southeast { get; private set; }

    int inserted = 0;

    public QuadTree(uint capacity, Rectangle boundary)
    {
        Depth = 0;
        Capacity = capacity;
        Boundary = boundary;
        Points = new Point[Capacity];
    }

    private QuadTree(uint capacity, Rectangle boundary, uint depth)
    {
        Capacity = capacity;
        Boundary = boundary;
        Points = new Point[Capacity];
        Depth = depth;
    }

    public void Insert(Point point)
    {
        if (inserted < Capacity)
        {
            Points[inserted] = point;
            inserted++;
        }
        else
        {
            if(!IsSubdivided)
            {
                Subdivide();
            }

            if (Northwest.Boundary.Contains(point))
            {
                Northwest.Insert(point);
                return;
            }
            if (Northeast.Boundary.Contains(point))
            {
                Northeast.Insert(point);
                return;
            }
            if (Southwest.Boundary.Contains(point))
            {
                Southwest.Insert(point);
                return;
            }
            if (Southeast.Boundary.Contains(point))
            {
                Southeast.Insert(point);
                return;
            }
        }
    }

    private void Subdivide()
    {
        uint newDepth = Depth + 1;
        Rectangle sw = new Rectangle(
            Boundary.X, 
            Boundary.Y,
            Boundary.Width / 2,
            Boundary.Height / 2);
        Southwest = new QuadTree(Capacity, sw, newDepth);

        Rectangle se = new Rectangle(
            Boundary.X + Boundary.Width / 2,
            Boundary.Y,
            Boundary.Width / 2,
            Boundary.Height / 2);
        Southeast = new QuadTree(Capacity, se, newDepth);

        Rectangle nw = new Rectangle(
            Boundary.X,
            Boundary.Y + Boundary.Height / 2,
            Boundary.Width / 2,
            Boundary.Height / 2);
        Northwest = new QuadTree(Capacity, nw, newDepth);

        Rectangle ne = new Rectangle(
            Boundary.X + Boundary.Width / 2,
            Boundary.Y + Boundary.Height / 2,
            Boundary.Width / 2,
            Boundary.Height / 2);
        Northeast = new QuadTree(Capacity, ne, newDepth);

        IsSubdivided = true;
    }
}
