using UnityEngine;
using System.Collections;

public class SplinePipe : MonoBehaviour {

    
    //public int radiusSegmentCount;

    private SplinePipeSystem pipeSystem;

    private Mesh mesh;

    public int spikeDensity;
    public float minSpikeHeight, maxSpikeHeight;
    public PipeObstacle spikePrefab;
    public Vector3[,] spikeVertices;
    private PipeObstacle[] spikes;
    private Quaternion goodToUp;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private float startPosition;

    //public int curveSegmentCount;
    //public float ringDistance;

	// Use this for initialization
	void Start () {
	
	}

    void Awake ()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";

        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GetComponent<MeshRenderer>().material = pipeSystem.material;

        spikes = new PipeObstacle[spikeDensity];
        for (int i = 0; i < spikes.Length; i++)
        {
            PipeObstacle spike = spikes[i] = Instantiate<PipeObstacle>(spikePrefab);
            spike.SetPipe(this);
            spike.transform.SetParent(transform, false);
        }
    }

    public void Generate(float initialPosition)
    {
        startPosition = initialPosition;

        mesh.Clear();
        SetVertices();
        SetUVs();
        SetTriangles();
        mesh.RecalculateNormals();

        GenerateObstacles();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void GenerateObstacles()
    {
        int quadNumber = pipeSystem.RadiusSegmentCount * pipeSystem.CurveSegmentCount;
        spikeVertices = new Vector3[5, spikes.Length];
        for (int i = 0; i < spikes.Length; i++)
        {
            int spikedQuad = Random.Range(0, quadNumber);
            spikeVertices[0, i] = vertices[spikedQuad * 4];
            spikeVertices[1, i] = vertices[spikedQuad * 4 + 1];
            spikeVertices[2, i] = vertices[spikedQuad * 4 + 2];
            spikeVertices[3, i] = vertices[spikedQuad * 4 + 3];

            Vector3 norm, center = Vector3.zero;

            norm = Vector3.Cross((spikeVertices[1, i] - spikeVertices[0, i]), (spikeVertices[2, i] - spikeVertices[0, i])).normalized;

            for (int j = 0; j < 4; j++)
            {
                center += spikeVertices[j, i];
            }

            center /= 4f;

            spikeVertices[4, i] = center - norm * Random.Range(minSpikeHeight, maxSpikeHeight);
        }

        for(int i = 0; i < spikes.Length; i++)
        {
            spikes[i].Generate(i);
        }
        
    }

    void SetVertices()
    {
        vertices = new Vector3[pipeSystem.RadiusSegmentCount * pipeSystem.CurveSegmentCount * 4];


        float vStep = 360f / pipeSystem.RadiusSegmentCount;
        float position = startPosition;

        CreateFirstQuadRing(position, vStep);

        int iDelta = pipeSystem.RadiusSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= pipeSystem.CurveSegmentCount; u++, i += iDelta, position += pipeSystem.RingDistance)
        {
            if (position > pipeSystem.spline.SplineLength)
            {
                position = position - pipeSystem.spline.SplineLength;
            }
            CreateQuadRing(position, vStep, i);
        }
        mesh.vertices = vertices;
    }

    void SetTriangles()
    {
        //int spikeIndex = 0;
        //spikeVertices = new Vector3[4, spikes.Length];
        triangles = new int[pipeSystem.RadiusSegmentCount * pipeSystem.CurveSegmentCount * 6];

        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 2;
            triangles[t + 2] = triangles[t + 3] = i + 1;
            triangles[t + 5] = i + 3;
        }
        
        mesh.triangles = triangles;
    }

    void SetUVs()
    {
        uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i += 4)
        {
            uvs[i] = Vector2.zero;
            uvs[i + 1] = Vector2.right;
            uvs[i + 2] = Vector2.up;
            uvs[i + 3] = Vector2.one;
        }
        mesh.uv = uvs;
    }

    void CreateFirstQuadRing (float position, float vStep)
    {
        vertices = new Vector3[pipeSystem.RadiusSegmentCount * pipeSystem.CurveSegmentCount * 4];

        float radiusA = pipeSystem.GetRadius(position - pipeSystem.RingDistance), radiusB = pipeSystem.GetRadius(position);

        float t1 = pipeSystem.spline.GetTForPosition(position),
            t2 = pipeSystem.spline.GetTForPosition(position + pipeSystem.RingDistance);

        Vector3 firstCircle = pipeSystem.spline.GetDirection(t1),
            secondCircle = pipeSystem.spline.GetDirection(t2), firstUp, secondUp,
            pointA = pipeSystem.spline.GetPoint(t1),
            pointB = pipeSystem.spline.GetPoint(t2);

        Quaternion toUp1 = Quaternion.AngleAxis(90f, Vector3.Cross(firstCircle, Vector3.up)),
            toUp2 = Quaternion.AngleAxis(90f, Vector3.Cross(secondCircle, Vector3.up)),
            aRot = Quaternion.AngleAxis(vStep, firstCircle),
            bRot = Quaternion.AngleAxis(vStep, secondCircle);

        firstUp = toUp1 * firstCircle;
        secondUp = toUp2 * secondCircle;

        Vector3 vertexA = pointA + firstUp * radiusA,
            vertexB = pointB + secondUp * radiusB;

        for (int v = 1, i = 0; v <= pipeSystem.RadiusSegmentCount; v++, i += 4)
        {
            firstUp = aRot * firstUp;
            secondUp = bRot * secondUp;

            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = pointA + firstUp * radiusA;
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = pointB + secondUp * radiusB;
        }
        goodToUp = toUp2;
        position += pipeSystem.RingDistance;
    }

    void CreateQuadRing (float position, float vStep, int i)
    {
        int ringOffset = pipeSystem.RadiusSegmentCount * 4;

        float radius = pipeSystem.GetRadius(position),
        t2 = pipeSystem.spline.GetTForPosition(position + pipeSystem.RingDistance);

        Vector3 secondCircle = pipeSystem.spline.GetDirection(t2);

        //toUp1 = Quaternion.AngleAxis(90f, Vector3.Cross(firstCircle, Vector3.up));
        Quaternion toUp2, bRot = Quaternion.AngleAxis(vStep, secondCircle);

        if (Vector3.Angle(secondCircle, Vector3.up) < 0.01f)
            toUp2 = goodToUp;
        else
            toUp2 = Quaternion.AngleAxis(90f, Vector3.Cross(secondCircle, Vector3.up));

        // firstUp = toUp1 * firstCircle;
        Vector3 secondUp = toUp2 * secondCircle,
        pointB = pipeSystem.spline.GetPoint(t2);

        secondUp.Normalize();

        Vector3 vertex = pointB + secondUp * radius;

        for (int v = 1; v <= pipeSystem.RadiusSegmentCount; v++, i += 4)
        {
            secondUp = bRot * secondUp;

            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = pointB + secondUp * radius;// = bRot * vertex; ;
        }
    }

    public float StartPosition
    {
        get
        {
            return startPosition;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //Gizmos.DrawSphere(vertices[4 * pipeSystem.RadiusSegmentCount + 4], 1f);

        /*for(int i = 0; i < spikeVertices.Length / 4; i++)
        {
            Gizmos.DrawSphere(spikeVertices[0, i], 2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(spikeVertices[1, i], 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spikeVertices[2, i], 2f);
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(spikeVertices[3, i], 2f);
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(spikeVertices[4, i], 2f);
            Gizmos.color = Color.green;
        }*/

        /*Gizmos.color = Color.red;

        vertices = new Vector3[radiusSegmentCount * curveSegmentCount * 4];

        float vStep = (360f) / radiusSegmentCount, position = 0f, radius = pipeSystem.GetRadius(position);

        float t1 = pipeSystem.spline.GetTForPosition(position), 
            t2 = pipeSystem.spline.GetTForPosition(position + ringDistance);

        Vector3 firstCircle = pipeSystem.spline.GetDirection(t1),
            secondCircle = pipeSystem.spline.GetDirection(t2), firstUp, secondUp,
            pointA = pipeSystem.spline.GetPoint(t1),
            pointB = pipeSystem.spline.GetPoint(t2);

        Quaternion toUp1 = Quaternion.AngleAxis(90f, Vector3.Cross(firstCircle, Vector3.up)),
            toUp2 = Quaternion.AngleAxis(90f, Vector3.Cross(secondCircle, Vector3.up)),
            aRot = Quaternion.AngleAxis(vStep, firstCircle),
            bRot = Quaternion.AngleAxis(vStep, secondCircle);

        firstUp = toUp1 * firstCircle;
        secondUp = toUp2 * secondCircle;

        Vector3 vertexA = pointA + firstUp * radius,
            vertexB = pointB + secondUp * radius;

        for (int v = 1, i = 0; v < radiusSegmentCount; v++, i += 4)
        {
            firstUp = aRot * firstUp;
            secondUp = bRot * secondUp;

            vertices[i] = vertexA;
            Gizmos.DrawSphere(vertexA, 1f);
            vertices[i + 1] = vertexA = pointA + firstUp * radius;
            Gizmos.DrawSphere(vertexA, 1f);
            vertices[i + 2] = vertexB;
            Gizmos.DrawSphere(vertexB, 1f);
            vertices[i + 3] = vertexB = pointB + secondUp * radius;
            Gizmos.DrawSphere(vertexB, 1f);
        }
        position += ringDistance;



        int iDelta = radiusSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta, position += ringDistance)
        {
            int ringOffset = radiusSegmentCount * 4;

            radius = pipeSystem.GetRadius(position);

            t1 = pipeSystem.spline.GetTForPosition(position);
            t2 = pipeSystem.spline.GetTForPosition(position + ringDistance);

            firstCircle = pipeSystem.spline.GetDirection(t1);
            secondCircle = pipeSystem.spline.GetDirection(t2);

            //toUp1 = Quaternion.AngleAxis(90f, Vector3.Cross(firstCircle, Vector3.up));
            toUp2 = Quaternion.AngleAxis(90f, Vector3.Cross(secondCircle, Vector3.up));
            //aRot = Quaternion.AngleAxis(vStep, firstCircle);
            bRot = Quaternion.AngleAxis(vStep, secondCircle);

           // firstUp = toUp1 * firstCircle;
            secondUp = toUp2 * secondCircle;

            secondUp.Normalize();

            pointB = pipeSystem.spline.GetPoint(t2);

            Vector3 vertex = pointB + secondUp * radius;

            for (int v = 1; v <= radiusSegmentCount; v++, i += 4)
            {
                secondUp = bRot * secondUp;

                vertices[i] = vertices[i - ringOffset + 2];
                Gizmos.DrawSphere(vertices[i], 0.5f);
                vertices[i + 1] = vertices[i - ringOffset + 3];
                Gizmos.DrawSphere(vertices[i + 1], 0.5f);
                vertices[i + 2] = vertex;
                Gizmos.DrawSphere(vertex, 0.5f);
                vertices[i + 3] = vertex = pointB + secondUp * radius;// = bRot * vertex; ;
                Gizmos.DrawSphere(vertex, 0.5f);
            }

            
        }

    */
    }
}
