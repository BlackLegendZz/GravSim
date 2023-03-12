using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGravSimulation : MonoBehaviour
{
    private const int minMass = 100;
    private const int maxMass = 1000;
    const float gravConst = 0.00000000006672041f;
    private float mass;
    private int numPoints;
    Point[] points;
    Transform[] pointsTransform;
    private Vector3[] tempVelocities;
    private SpriteRenderer[] spriteRenders;

    public Transform Prefab;
    public bool RandomMass = false;
    public bool InitialRandomVelocity = false;
    [Range(minMass, maxMass)]
    public float Mass = 400;
    [Range(2, 2000)]
    public int NumPoints = 2;

    // Start is called before the first frame update
    void Start()
    {
        numPoints = NumPoints;
        mass = Mass;
        Point point = new Point();
        tempVelocities = new Vector3[numPoints];

        point.velocity = Vector3.zero;
        point.mass = mass;

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
                point.mass = Random.Range(minMass, maxMass);
            }
            if (InitialRandomVelocity)
            {
                point.velocity.x = Random.value * 0.001f;
                point.velocity.y = Random.value * 0.001f;
            }
            Transform t = Instantiate(Prefab);
            pos.x = Random.Range(-camVisibleFieldWidth, camVisibleFieldWidth);
            pos.y = Random.Range(-camVisibleFieldHeight, camVisibleFieldHeight);
            t.position = pos;
            t.name = $"ball_{i}";
            t.localScale = Vector3.one * (point.mass / maxMass);
            t.SetParent(transform, false);

            SpriteRenderer ren = t.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenders[i] = ren;

            point.position = t.position;
            pointsTransform[i] = t;
            points[i] = point;
            tempVelocities[i] = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float maxVelocitySqrMagnitude = 0.0f;
        Vector3 center = Vector3.zero;
        for (int i = 0; i < numPoints; i++)
        {
            Point p1 = points[i];
            Vector3 vel = p1.velocity;
            for (int j = 0; j < numPoints; j++)
            {
                if (j == i) { continue; }
                Point p2 = points[j];

                Vector3 deltaDist = p1.position - p2.position;
                float distSqr = deltaDist.sqrMagnitude;
                float force = gravConst * (p1.mass * p2.mass) / (distSqr + 1e-5f);

                Vector3 deltaVelocity = deltaDist * force;
                vel += deltaVelocity;
            }
            tempVelocities[i] = vel;
            if(vel.sqrMagnitude > maxVelocitySqrMagnitude)
            {
                maxVelocitySqrMagnitude = vel.sqrMagnitude;
            }
        }

        for (int i = 0; i < numPoints; i++)
        {
            points[i].position -= tempVelocities[i];
            points[i].velocity = tempVelocities[i];
            pointsTransform[i].position = points[i].position;
            center += points[i].position;

            float ratio = points[i].velocity.sqrMagnitude / maxVelocitySqrMagnitude;
            spriteRenders[i].color = new Color(ratio, ratio, ratio);
        }

        center.x /= numPoints;
        center.y /= numPoints;
        center.z = -10;
        Camera.main.transform.position = center;
    }
}
