using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGravSimulation : MonoBehaviour
{
    int minMass = 100;
    int maxMass = 1000;

    public Transform prefab;
    public bool randomMass = false;
    [Range(100, 1000)]
    public int mass = 400;
    [Range(2, 2000)]
    public int numPoints = 2;
    public bool optimisedCalc = false;
    Point[] points;
    Transform[] pointsTransform;
    const float gravConst = 0.00000000006672041f;
    
    // Start is called before the first frame update
    void Start()
    {
        Point point = new Point();
        point.velocity = Vector3.zero;
        point.mass = mass;

        points = new Point[numPoints];
        pointsTransform = new Transform[numPoints];

        float camVisibleFieldHeight = Camera.main.orthographicSize;
        float scale = Camera.main.pixelWidth / (float)Camera.main.pixelHeight;
        float camVisibleFieldWidth = camVisibleFieldHeight * scale;
        camVisibleFieldWidth -= prefab.localScale.x;
        camVisibleFieldHeight -= prefab.localScale.y;

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < numPoints; i++)
        {
            if (randomMass)
            {
                point.mass = Random.Range(minMass, maxMass);
            }

            Transform t = Instantiate(prefab);
            pos.x = Random.Range(-camVisibleFieldWidth, camVisibleFieldWidth);
            pos.y = Random.Range(-camVisibleFieldHeight, camVisibleFieldHeight);
            t.position = pos;
            t.name = $"ball_{i}";
            t.localScale = Vector3.one * ((float)point.mass / maxMass);
            t.SetParent(transform, false);

            point.position = t.position;
            pointsTransform[i] = t;
            points[i] = point;
        }
    }
    void normal()
    {
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
                float force = gravConst * (p1.mass * p2.mass) / distSqr;

                Vector3 deltaVelocity = deltaDist * force;
                vel += deltaVelocity;
            }

            p1.position -= vel;
            p1.velocity = vel;
            points[i] = p1;
            pointsTransform[i].position = p1.position;
            center += p1.position;
        }
        center.x /= numPoints;
        center.y /= numPoints;
        center.z = -10;
        Camera.main.transform.position = center;
    }
    /*
    void optimised()
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < numPoints; i++)
        {
            Point p1 = points[i];
            Vector3 vel = p1.velocity;
            p1.deltaVelocity = Vector3.zero;
            for (int j = 0; j < numPoints; j++)
            {
                if (j == i) { continue; }
                Point p2 = points[j];

                Vector3 deltaDist = p1.position - p2.position;
                float distSqr = deltaDist.sqrMagnitude;
                float force = (p1.mass * p2.mass) / distSqr;

                p1.deltaVelocity += deltaDist * force;
            }
            vel += gravConst * p1.deltaVelocity;

            p1.position -= vel;
            p1.velocity = vel;
            points[i] = p1;
            pointsTransform[i].position = p1.position;
            center += p1.position;
        }
        center.x /= numPoints;
        center.y /= numPoints;
        center.z = -10;
        Camera.main.transform.position = center;
    }
    */
    // Update is called once per frame
    void Update()
    {
        if (optimisedCalc)
        {
            //optimised();
        }
        else
        {
            normal();
        }
    }
}
