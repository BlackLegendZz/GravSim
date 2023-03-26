using System.Collections.Generic;
using UnityEngine;

namespace DataStructures.QuadTree
{
    public sealed class QuadTree<T> : IQuadTree<T> where T : struct
    {
        public uint Capacity { get; private set; }
        public T[] Data;
        public Rectangle Boundary;
        public bool IsSubdivided { get; private set; } = false;
        public uint Depth { get; private set; } = 0;
        public QuadTree<T> Northwest { get; private set; }
        public QuadTree<T> Northeast { get; private set; }
        public QuadTree<T> Southwest { get; private set; }
        public QuadTree<T> Southeast { get; private set; }

        int inserted = 0;
        private readonly Vector2[] points;

        public QuadTree(uint capacity, Rectangle boundary)
        {
            Capacity = capacity;
            Boundary = boundary;
            Data = new T[Capacity];
            points= new Vector2[Capacity];
        }

        public void Insert(Vector2 point, T data)
        {
            if (inserted < Capacity)
            {
                Data[inserted] = data;
                points[inserted] = point;
                inserted++;
            }
            else
            {
                if (!IsSubdivided)
                {
                    Subdivide();
                }

                if (Northwest.Boundary.Contains(point))
                {
                    Northwest.Insert(point, data);
                    return;
                }
                if (Northeast.Boundary.Contains(point))
                {
                    Northeast.Insert(point, data);
                    return;
                }
                if (Southwest.Boundary.Contains(point))
                {
                    Southwest.Insert(point, data);
                    return;
                }
                if (Southeast.Boundary.Contains(point))
                {
                    Southeast.Insert(point, data);
                    return;
                }
            }
        }

        public void Insert(T data)
        {
            throw new System.NotImplementedException();
        }

        public T[] GetPointsInside(Rectangle rect)
        {
            List<T> foundPoints = new List<T>();
            for (int i = 0; i < inserted; i++)
            {
                if (rect.Contains(points[i]))
                {
                    foundPoints.Add(Data[i]);
                }
            }

            if (IsSubdivided)
            {
                foundPoints.AddRange(Southeast.GetPointsInside(rect));
                foundPoints.AddRange(Southwest.GetPointsInside(rect));
                foundPoints.AddRange(Northeast.GetPointsInside(rect));
                foundPoints.AddRange(Northwest.GetPointsInside(rect));
            }

            return foundPoints.ToArray();
        }

        private void Subdivide()
        {
            Rectangle sw = new Rectangle(
                Boundary.X,
                Boundary.Y,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Southwest = new QuadTree<T>(Capacity, sw);
            Southwest.Depth++;

            Rectangle se = new Rectangle(
                Boundary.X + Boundary.Width / 2,
                Boundary.Y,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Southeast = new QuadTree<T>(Capacity, se);
            Southeast.Depth++;

            Rectangle nw = new Rectangle(
                Boundary.X,
                Boundary.Y + Boundary.Height / 2,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Northwest = new QuadTree<T>(Capacity, nw);
            Northwest.Depth++;

            Rectangle ne = new Rectangle(
                Boundary.X + Boundary.Width / 2,
                Boundary.Y + Boundary.Height / 2,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Northeast = new QuadTree<T>(Capacity, ne);
            Northeast.Depth++;

            IsSubdivided = true;
        }
    }
}
