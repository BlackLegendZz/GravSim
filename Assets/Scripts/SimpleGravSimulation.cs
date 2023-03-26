using DataStructures;
using UnityEngine;

public class SimpleGravSimulation : MonoBehaviour
{
    const int minMass = 100;
    const int maxMass = 1000;
    const float gravConst = 0.00000000006672041f;
    float mass;
    int numPoints;
    Point[] points;
    Transform[] pointsTransform;
    Vector3[] tempVelocities;
    SpriteRenderer[] spriteRenders;

    public Transform Prefab;
    [Range(2, 2000)]
    public int NumPoints = 2;
    public bool Optimize = true;

    [Header("Mass")]
    public bool RandomMass = false;
    [Range(minMass, maxMass)]
    public float Mass = 400;

    [Header("Initial Velocity")]
    public bool InitialRandomVelocity = false;
    [Range(0.001f, 0.1f)]
    public float VelocityScale = 0.001f;


    void NaiveNBody()
    {
        // Very basic newtonian gravity formula for all N bodies
        float maxVelocitySqrMagnitude = 0.0f;
        Vector3 center = Vector3.zero;
        for (int i = 0; i < numPoints; i++)
        {
            Point p1 = points[i];
            Vector3 vel = p1.Velocity;
            for (int j = 0; j < numPoints; j++)
            {
                if (j == i) { continue; }
                Point p2 = points[j];

                Vector3 deltaDist = p1.Position - p2.Position;
                float distSqr = deltaDist.sqrMagnitude;
                float force = gravConst * (p1.Mass * p2.Mass) / (distSqr + 1e-5f); //Apply a low value to the distance to combat a near "infinite" force for very close bodies

                Vector3 deltaVelocity = deltaDist * force;
                vel += deltaVelocity;
            }
            tempVelocities[i] = vel;
            if (vel.sqrMagnitude > maxVelocitySqrMagnitude)
            {
                maxVelocitySqrMagnitude = vel.sqrMagnitude;
            }
        }

        // Apply the calculated forces at the end to not screw up the forces of bodies that get calculated later
        for (int i = 0; i < numPoints; i++)
        {
            points[i].Position -= tempVelocities[i];
            points[i].Velocity = tempVelocities[i];
            pointsTransform[i].position = points[i].Position;
            center += points[i].Position;

            float ratio = points[i].Velocity.sqrMagnitude / maxVelocitySqrMagnitude;
            spriteRenders[i].color = new Color(ratio, ratio, ratio);
        }

        center.x /= numPoints;
        center.y /= numPoints;
        center.z = -10;
        Camera.main.transform.position = center;
    }

    void BarnesHut()
    {
        /*
        QuadTree qt = new QuadTree(1, new Rectangle(-15,-15,30,30));
        for (int i = 0; i < numPoints; i++)
        {
            qt.Insert(points[i]);
        }
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        numPoints = NumPoints;
        mass = Mass;
        
        tempVelocities = new Vector3[numPoints];
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
                point.Mass = Random.Range(minMass, maxMass);
            }
            if (InitialRandomVelocity)
            {
                point.Velocity.x = Random.value * VelocityScale;
                point.Velocity.y = Random.value * VelocityScale;
            }
            Transform t = Instantiate(Prefab);
            pos.x = Random.Range(-camVisibleFieldWidth, camVisibleFieldWidth);
            pos.y = Random.Range(-camVisibleFieldHeight, camVisibleFieldHeight);
            t.position = pos;
            t.name = $"ball_{i}";
            t.localScale = Vector3.one * (point.Mass / maxMass);
            t.SetParent(transform, false);

            SpriteRenderer ren = t.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenders[i] = ren;

            point.Position = t.position;
            pointsTransform[i] = t;
            points[i] = point;
            tempVelocities[i] = Vector3.zero;
        }

        Camera.main.orthographicSize = 10; // Adjust the camera so MOST bodies are in view all of the time
    }

    // Update is called once per frame
    void Update()
    {
        if (Optimize)
        {
            BarnesHut();
        }
        else
        {
            NaiveNBody();
        }
    }
}
