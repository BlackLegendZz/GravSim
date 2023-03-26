using DataStructures;
using DataStructures.QuadTree;
using System;
using UnityEngine;

public class SimpleGravSimulation : MonoBehaviour
{
    const int minMass = 500;
    const int maxMass = 2000;
    float mass;
    int numPoints;
    Point[] points;
    QuadTreeBH qt;
    Transform[] pointsTransform;
    SpriteRenderer[] spriteRenders;

    public Transform Prefab;
    [Range(2, 2000)]
    public int NumPoints = 2;
    [Range(0f, 10f)]
    public float TimeStep = 1f;

    [Header("Barnes-Hut Optimization")]
    public bool Optimize = true;
    [Range(0f, 2f)]
    public float Theta = 1f;

    [Header("Mass")]
    public bool RandomMass = false;
    [Range(minMass, maxMass)]
    public float Mass = 1000;

    [Header("Initial Velocity")]
    public bool InitialRandomVelocity = false;
    [Range(0.001f, 0.1f)]
    public float VelocityScale = 0.001f;

    void NaiveNBody()
    {       
        for (int i = 0; i < numPoints; i++)
        {
            Point p1 = points[i];
            Vector3 force = Vector3.zero;
            for (int j = 0; j < numPoints; j++)
            {
                if (j == i) { continue; }
                Point p2 = points[j];
                // Force calculation isn't any different there. I just wanted to remove redundancy
                force += QuadTreeBH.ForceBetweenPoints(p2, p1);
            }
            points[i].Acceleration = force / points[i].Mass;
        }
    }

    void BarnesHut()
    {
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        for (int i = 0; i < numPoints; i++)
        {
            Vector3 pos = points[i].Position;
            if (pos.x < minX)
            {
                minX = pos.x;
            }
            if (pos.y < minY)
            {
                minY = pos.y;
            }
            if (pos.x > maxX)
            {
                maxX = pos.x;
            }
            if (pos.y > maxY)
            {
                maxY = pos.y;
            }
        }
        qt = new QuadTreeBH(new Rectangle(minX - 1, minY - 1, maxX - minX + 2, maxY - minY + 2), Theta);
        for (int i = 0; i < numPoints; i++)
        {
            qt.Insert(points[i]);
        }
        qt.MassDistribution();
        for (int i = 0; i < numPoints; i++)
        {
            points[i].Acceleration = qt.CalculateAcceleration(points[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        numPoints = NumPoints;
        mass = Mass;
        
        Point point = new Point();
        point.Velocity = Vector3.zero;
        point.Mass = mass;

        points = new Point[numPoints];
        pointsTransform = new Transform[numPoints];
        spriteRenders = new SpriteRenderer[numPoints];

        float camVisibleFieldHeight = Camera.main.orthographicSize;
        float scale = Camera.main.pixelWidth / (float)Camera.main.pixelHeight;
        float camVisibleFieldWidth = camVisibleFieldHeight * scale;
        camVisibleFieldWidth -= Prefab.localScale.x;
        camVisibleFieldHeight -= Prefab.localScale.y;

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < numPoints; i++)
        {
            if (RandomMass)
            {
                point.Mass = UnityEngine.Random.Range(minMass, maxMass);
            }
            if (InitialRandomVelocity)
            {
                point.Velocity.x = UnityEngine.Random.value * VelocityScale;
                point.Velocity.y = UnityEngine.Random.value * VelocityScale;
            }
            pos.x = UnityEngine.Random.Range(-camVisibleFieldWidth, camVisibleFieldWidth);
            pos.y = UnityEngine.Random.Range(-camVisibleFieldHeight, camVisibleFieldHeight);

            point.Position = pos;
            points[i] = point;

            
            Transform t = Instantiate(Prefab);
            t.position = pos;
            t.name = $"ball_{i}";
            t.localScale = Vector3.one * (point.Mass / maxMass);
            t.SetParent(transform, false);

            SpriteRenderer ren = t.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenders[i] = ren;
            pointsTransform[i] = t;            
        }

        Camera.main.orthographicSize = 10; // Adjust the camera so MOST bodies are in view all of the time
    }

    // Update is called once per frame
    void Update()
    {
        float maxVel = 0f;
        if (Optimize)
        {
            BarnesHut();
        }
        else
        {
            NaiveNBody();
        }

        for (int i = 0; i < numPoints; i++)
        {
            points[i].Position += points[i].Velocity * TimeStep;
            points[i].Velocity += points[i].Acceleration * TimeStep;

            if (points[i].Velocity.sqrMagnitude > maxVel)
            {
                maxVel = points[i].Velocity.sqrMagnitude;
            }
        }

        Vector3 center = Vector3.zero;
        // Apply the calculated forces at the end to not screw up the forces of bodies that get calculated later
        for (int i = 0; i < numPoints; i++)
        {
            pointsTransform[i].position = points[i].Position;
            center += points[i].Position;

            float ratio = points[i].Velocity.sqrMagnitude / maxVel;
            spriteRenders[i].color = new Color(ratio, ratio, ratio);
        }

        center.x /= numPoints;
        center.y /= numPoints;
        center.z = -10;
        Camera.main.transform.position = center;
        
    }

    private void OnDrawGizmos()
    {
        try
        {
            DrawQT(qt);
        }
        catch (NullReferenceException)
        {
            
        }
    }

    void DrawQT(QuadTreeBH qt)
    {
        Gizmos.color = Color.red;
        if (qt.ContainsData)
        {
            Gizmos.DrawSphere(qt.Body.Position, 0.01f);
        }

        Rectangle rect = qt.Boundary;
        Gizmos.DrawWireCube(new Vector3(rect.Center.x, rect.Center.y), new Vector3(rect.Width, rect.Height));
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(qt.collectiveMass.Position, 0.2f * 1/(1+qt.Depth*2));

        if (qt.IsSubdivided)
        {
            DrawQT(qt.Northeast);
            DrawQT(qt.Northwest);
            DrawQT(qt.Southeast);
            DrawQT(qt.Southwest);
        }
    }
}
