public class Rectangle
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool Contains(Point point)
    {
        return point.Position.x > X - Width &&
            point.Position.x < X + Width &&
            point.Position.y > Y - Height &&
            point.Position.y < Y + Height;
    }
}
