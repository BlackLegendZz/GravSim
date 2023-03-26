using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DataStructures.QuadTree
{
    internal interface IQuadTree<T> where T : struct
    {
        public void Insert(Vector2 point, T data);
        public void Insert(T data);
        public T[] GetPointsInside(Rectangle rect);
    }
}
