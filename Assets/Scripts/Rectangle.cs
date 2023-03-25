using System.Numerics;

public sealed class Rectangle
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public Vector2 Center;

    public Rectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Center= new Vector2(X + width / 2, Y + height / 2);
    }

    public bool Contains(Point point)
    {
        return point.Position.x > X 
            && point.Position.x < X + Width 
            && point.Position.y > Y 
            && point.Position.y < Y + Height;
    }
}
