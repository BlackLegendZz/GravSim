using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuadTreeViz : MonoBehaviour
{
    [Range(0,100)]
    public uint Points = 5;
    [Range(1,10)]
    public uint Capacity = 4;
    public uint drawDepth = 1;

    QuadTree qt;
    uint points = 0;
    uint capacity = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    private void OnDrawGizmos()
    {
        if (qt == null || points != Points || capacity != Capacity)
        {
            points = Points;
            capacity= Capacity;
            Random.InitState(0);

            qt = new QuadTree(Capacity, new Rectangle(0, 0, 5, 5));
            for (int i = 0; i < Points; i++)
            {
                Point p = new Point();
                p.Position.x = Random.Range(0.1f, 4.9f);
                p.Position.y = Random.Range(0.1f, 4.9f);
                qt.Insert(p);
            }
        }
        Draw(qt, "");
    }
    
    void Draw(QuadTree qt, string side)
    {
        Color c;
        switch (side)
        {
            case "nw":
                c = Color.white;
                break;
            case "ne":
                c = Color.red;
                break;
            case "sw":
                c = Color.green;
                break;
            case "se":
                c = Color.blue;
                break;
            default:
                c = Color.black;
                break;
        }
        Gizmos.color = c;

        Rectangle rect = qt.Boundary;
        if (qt.Depth == drawDepth)
        {
            foreach (Point p in qt.Points)
            {
                Gizmos.DrawSphere(p.Position, 0.1f / (1 + 0.5f * qt.Depth));
            }
            Gizmos.DrawWireCube(new Vector3(rect.Center.X, rect.Center.Y), new Vector3(rect.Width, rect.Height));
            return;
        }

        if (qt.IsSubdivided)
        {
            Draw(qt.Northeast, "ne");
            Draw(qt.Northwest, "nw");
            Draw(qt.Southeast, "se");
            Draw(qt.Southwest, "sw");
        }
    }
}
