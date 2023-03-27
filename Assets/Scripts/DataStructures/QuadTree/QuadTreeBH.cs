using System;
using UnityEngine;

namespace DataStructures.QuadTree
{
    public sealed class QuadTreeBH : IQuadTree<Point>
    {
        public Point Body { get; private set; }
        public Rectangle Boundary { get; private set; }
        public bool IsSubdivided { get; private set; } = false;
        public uint Depth { get; private set; } = 0;
        public bool ContainsData { get; private set; } = false;
        public QuadTreeBH Northwest { get; private set; }
        public QuadTreeBH Northeast { get; private set; }
        public QuadTreeBH Southwest { get; private set; }
        public QuadTreeBH Southeast { get; private set; }
        readonly float theta = 1f;
        const float gravConst = 6.674e-11f;
        public Point collectiveMass = new Point();
        QuadTreeBH[] quadrants;

        public QuadTreeBH(Rectangle boundary, float theta)
        {
            Boundary = boundary;
            this.theta = theta;
        }
        public void Insert(Vector2 point, Point data)
        {
            throw new NotImplementedException();
        }

        public void Insert(Point body)
        {
            if(!ContainsData)
            {
                Body = body;
                ContainsData = true;
                return;
            }

            if (!IsSubdivided)
            {
                Subdivide();
            }

            for (int i = 0; i < 4; i++)
            {
                if (quadrants[i].Boundary.Contains(body.Position))
                {
                    quadrants[i].Insert(body);
                    return;
                }
            }
        }

        public void MassDistribution()
        {
            if (ContainsData)
            {
                collectiveMass.Mass = Body.Mass;
                collectiveMass.Position = Body.Position;
            }

            if (IsSubdivided)
            {
                float mass = 0;
                Vector3 com = Vector3.zero;
                // Calculate weighted (haha) distribution of mass
                for (int i = 0; i < 4; i++)
                {
                    quadrants[i].MassDistribution();
                    mass += quadrants[i].collectiveMass.Mass;
                    com += quadrants[i].collectiveMass.Mass * quadrants[i].collectiveMass.Position;
                }
                mass += Body.Mass;
                com += Body.Position;
                com /= mass;

                collectiveMass.Position = com;
                collectiveMass.Mass = mass;
            }
        }

        public Vector3 CalculateAcceleration(Point otherBody)
        {
            Vector3 force = CalculateForceVector(otherBody);
            // a = F/m
            Vector3 acceleration = force / otherBody.Mass;
            return acceleration;
        }

        private Vector3 CalculateForceVector(Point otherBody)
        {
            Vector3 force = Vector3.zero;
            if (!IsSubdivided)
            {
                force = ForceBetweenPoints(Body, otherBody);
            }
            else
            {
                float dist = Vector3.Distance(otherBody.Position, collectiveMass.Position);
                float ratio = Mathf.Max(Boundary.Height, Boundary.Width) / dist;
                if (ratio < theta)
                {
                    force = ForceBetweenPoints(collectiveMass, otherBody);
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        force += quadrants[i].CalculateForceVector(otherBody);
                    }
                }
            }
            
            return force;
        }

        public static Vector3 ForceBetweenPoints(Point point1, Point point2)
        {
            Vector3 delta = point1.Position - point2.Position;
            float distance = delta.magnitude;
            //Apply a low value to the distance to combat a near "infinite" force for very close bodies
            Vector3 force = gravConst * (point1.Mass * point2.Mass) * delta.normalized / (distance + 1e-5f);
            return force;
        }

        private void Subdivide()
        {
            Rectangle sw = new Rectangle(
                Boundary.X,
                Boundary.Y,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Southwest = new QuadTreeBH(sw, theta);
            Southwest.Depth = Depth + 1;

            Rectangle se = new Rectangle(
                Boundary.X + Boundary.Width / 2,
                Boundary.Y,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Southeast = new QuadTreeBH(se, theta);
            Southeast.Depth = Depth + 1;

            Rectangle nw = new Rectangle(
                Boundary.X,
                Boundary.Y + Boundary.Height / 2,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Northwest = new QuadTreeBH(nw, theta);
            Northwest.Depth = Depth + 1;

            Rectangle ne = new Rectangle(
                Boundary.X + Boundary.Width / 2,
                Boundary.Y + Boundary.Height / 2,
                Boundary.Width / 2,
                Boundary.Height / 2);
            Northeast = new QuadTreeBH(ne, theta);
            Northeast.Depth = Depth + 1;

            IsSubdivided = true;
            quadrants = new QuadTreeBH[4] { Northeast, Northwest, Southeast, Southwest };
        }

        public Point[] GetPointsInside(Rectangle rect)
        {
            throw new NotImplementedException();
        }
    }
}
