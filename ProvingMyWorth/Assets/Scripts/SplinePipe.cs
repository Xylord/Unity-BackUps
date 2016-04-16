using UnityEngine;
using System.Collections;

public class SplinePipe : MonoBehaviour {

    
    //public int radiusSegmentCount;

    private SplinePipeSystem pipeSystem;

    private Mesh mesh;

    private Vector3[] vertices;
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
    }

    public void Generate(float initialPosition)
    {
        startPosition = initialPosition;

        mesh.Clear();
        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();

    }
	
	// Update is called once per frame
	void Update () {
	
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
        position += pipeSystem.RingDistance;
    }

    void CreateQuadRing (float position, float vStep, int i)
    {
        int ringOffset = pipeSystem.RadiusSegmentCount * 4;

        float radius = pipeSystem.GetRadius(position),
        t2 = pipeSystem.spline.GetTForPosition(position + pipeSystem.RingDistance);

        Vector3 secondCircle = pipeSystem.spline.GetDirection(t2);

        //toUp1 = Quaternion.AngleAxis(90f, Vector3.Cross(firstCircle, Vector3.up));
        Quaternion toUp2 = Quaternion.AngleAxis(90f, Vector3.Cross(secondCircle, Vector3.up)),
        bRot = Quaternion.AngleAxis(vStep, secondCircle);

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
        Gizmos.color = Color.red;

        //Gizmos.DrawSphere(vertices[4 * pipeSystem.RadiusSegmentCount + 4], 1f);

        /*for(int i = 0; i < testGIZMOS.Length; i++)
        {
        Gizmos.DrawSphere(testGIZMOS[i], 1f);
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
