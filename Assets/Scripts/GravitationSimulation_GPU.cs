using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationSimulation_GPU : MonoBehaviour
{
    float minMass = 0.0005f;
    float maxMass = 0.0015f;

    public ComputeShader shader;
    public Transform prefab;
    public bool randomMass = false;
    [Range(0.0005f, 0.0015f)]
    public float mass = 0.1f;
    [Range(2, 20000)]
    public int numPoints = 2;

    Point[] points;
    Transform[] pointsTransform;
    ComputeBuffer buffer;
    int p_size;
    int _numPoints;

    // Start is called before the first frame update
    void Start()
    {
        _numPoints = numPoints;
        p_size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Point));
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

    // Update is called once per frame
    void Update()
    {
        buffer = new ComputeBuffer(_numPoints, p_size);
        buffer.SetData(points);

        shader.SetBuffer(0, "points", buffer);
        shader.SetInt("numPoints", _numPoints);
        shader.Dispatch(0, _numPoints / 100, 1, 1);

        buffer.GetData(points);
        buffer.Dispose();

        Vector3 center = Vector3.zero;
        for (int i = 0; i < _numPoints; i++)
        {
            center += points[i].position;
            pointsTransform[i].position = points[i].position;
        }
        center.x /= numPoints;
        center.y /= numPoints;
        center.z = -10;
        Camera.main.transform.position = center;
    }
}
