public sealed class QuadTree
{
    public int Capacity { get; private set; }
    public Point[] Points;
    public Rectangle Boundary;

    QuadTree northwest;
    QuadTree northeast;
    QuadTree southwest;
    QuadTree southeast;
    int inserted = 0;
    bool isSubdivided = false;

    public QuadTree(int capacity, Rectangle boundary)
    {
        Capacity = capacity;
        Boundary = boundary;
        Points = new Point[Capacity];
    }

    public void Insert(Point point)
    {
        if (inserted != Capacity)
        {
            Points[inserted] = point;
            inserted++;
        }
        else
        {
            if(!isSubdivided)
            {
                Subdivide();
            }

            if (northwest.Boundary.Contains(point))
            {
                northwest.Insert(point);
                return;
            }
            if (northeast.Boundary.Contains(point))
            {
                northeast.Insert(point);
                return;
            }
            if (southwest.Boundary.Contains(point))
            {
                southwest.Insert(point);
                return;
            }
            if (southeast.Boundary.Contains(point))
            {
                southeast.Insert(point);
                return;
            }
        }
    }

    private void Subdivide()
    {
        Rectangle ne = new Rectangle(
            Boundary.X + Boundary.Width / 2, 
            Boundary.Y - Boundary.Height / 2,
            Boundary.Width / 2,
            Boundary.Height / 2);
        northeast = new QuadTree(Capacity, ne);

        Rectangle nw = new Rectangle(
            Boundary.X - Boundary.Width / 2,
            Boundary.Y - Boundary.Height / 2,
            Boundary.Width / 2,
            Boundary.Height / 2);
        northwest = new QuadTree(Capacity, nw);

        Rectangle se = new Rectangle(
            Boundary.X + Boundary.Width / 2,
            Boundary.Y + Boundary.Height / 2,
            Boundary.Width / 2,
            Boundary.Height / 2);
        southeast = new QuadTree(Capacity, se);

        Rectangle sw = new Rectangle(
            Boundary.X - Boundary.Width / 2,
            Boundary.Y + Boundary.Height / 2,
            Boundary.Width / 2,
            Boundary.Height / 2);
        southwest = new QuadTree(Capacity, sw);

        isSubdivided= true;
    }
}
