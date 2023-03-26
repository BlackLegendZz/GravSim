using DataStructures;
using DataStructures.QuadTree;
using UnityEngine;


public class QuadTreeViz : MonoBehaviour
{
    [Header("Quadtree Zeug")]
    public Vector2 StartPositionQT = Vector2.zero;
    public Vector2 SizeQT = Vector2.one;
    [Range(0,400)]
    public uint Points = 5;
    [Range(1, 10)]
    public uint Capacity = 4;
    [Header("Visualisierung")]
    public bool drawAll = true;
    public uint drawDepth = 1;
    [Header("Überlappendes Rechteck")]
    public Vector2 StartPosition = Vector2.zero;
    public Vector2 Size = Vector2.one;

    QuadTree<Point> qt;
    Rectangle queryRect;
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
        Random.InitState(0);

        qt = new QuadTree<Point>(Capacity, new Rectangle(StartPositionQT.x, StartPositionQT.y, SizeQT.x, SizeQT.y));
        for (int i = 0; i < Points; i++)
        {
            Point p = new Point();
            p.Position.x = Random.Range(StartPositionQT.x, StartPositionQT.x + SizeQT.x);
            p.Position.y = Random.Range(StartPositionQT.y, StartPositionQT.y + SizeQT.y);
            qt.Insert(p.Position, p);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(StartPosition + Size / 2, Size);
        queryRect = new Rectangle(StartPosition.x, StartPosition.y, Size.x, Size.y);
        Point[] foundPoints = qt.GetPointsInside(queryRect);
        foreach (Point fp in foundPoints)
        {
            Gizmos.DrawSphere(fp.Position, 0.1f);
        }
        Debug.Log(foundPoints.Length);

        DrawQT(qt, "");
    }
    
    void DrawQT(QuadTree<Point> qt, string side)
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

        Gizmos.color = Color.black;
        foreach (Point p in qt.Data)
        {
            if (queryRect.Contains(p.Position))
            {
                continue;
            }
            Gizmos.DrawSphere(p.Position, 0.1f / (1 + 0.5f * qt.Depth));
        }

        Rectangle rect = qt.Boundary;
        if (drawAll)
        {
            Gizmos.DrawWireCube(new Vector3(rect.Center.x, rect.Center.y), new Vector3(rect.Width, rect.Height));
        }
        else
        {
            if (qt.Depth == drawDepth)
            {
                Gizmos.color = c;
                Gizmos.DrawWireCube(new Vector3(rect.Center.x, rect.Center.y), new Vector3(rect.Width, rect.Height));
            }
        }

        if (qt.IsSubdivided)
        {
            DrawQT(qt.Northeast, "ne");
            DrawQT(qt.Northwest, "nw");
            DrawQT(qt.Southeast, "se");
            DrawQT(qt.Southwest, "sw");
        }
    }
}
