using UnityEngine;

namespace DataStructures
{
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
            Center = new Vector2(X + width / 2, Y + height / 2);
        }

        public bool Contains(Vector2 point)
        {
            return point.x > X
                && point.x < X + Width
                && point.y > Y
                && point.y < Y + Height;
        }

        public bool Overlaps(Rectangle other)
        {
            return !(X + Width < other.X || other.X + other.Width < X
                || Y + Height < other.Y || other.Y + Height < Y);
        }
    }
}
