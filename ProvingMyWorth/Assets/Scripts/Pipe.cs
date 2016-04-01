using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour {
    //private GameObject pipe;
    private Mesh mesh;
    //private MeshCollider meshCollider;
    private Vector3[] vertices;
    private int[] triangles;
    private float arcLength, curveAngle, curveRadius, relativeRotation, deltaDistance;
    
    private int curveSegmentCount;

    private PipeSystem pipeSystem;

    public float minCurveRadius, maxCurveRadius,
        ringDistance;

    public int minCurveSegmentCount, maxCurveSegmentCount,
        pipeSegmentCount;

    static private int pipeNumber = 0;
    static private float lastQuadRadius;

    private bool lastQuad = false;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake ()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        
        

    }

    public void Generate ()
    {
        GameObject mainPipeSystem = GameObject.Find("PipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<PipeSystem>();

        GetComponent<MeshRenderer>().material = pipeSystem.pipeMaterial;

        this.name = "pipe" + pipeNumber.ToString();
        pipeNumber++;
        
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
        curveAngle = ringDistance * (360f / (2f * Mathf.PI));
        arcLength = 2 * Mathf.PI * curveRadius * curveAngle / 360;
        deltaDistance = arcLength / curveSegmentCount;

        mesh.Clear();
        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();

        /*meshCollider = this.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;*/
    }

    void SetVertices()
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
        //verticesToPassOn = new Vector3[pipeSegmentCount * curveSegmentCount * 4];

        float uStep = ringDistance / curveSegmentCount;
        // = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
        CreateFirstQuadRing(uStep);
        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
        {
            if (u == curveSegmentCount)
            {
                lastQuad = true;
            }
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
    }

    void SetTriangles ()
    {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 2;
            triangles[t + 2] = triangles[t + 3] = i + 1;
            triangles[t + 5] = i + 3;
        }
        mesh.triangles = triangles;
    }

    void CreateQuadRing (float u, int i)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;
        int ringOffset = pipeSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus(u, 0f, false);
        for (int v = 1, j = 0; v <= pipeSegmentCount; v++, i += 4, j += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep, false);
        }
        pipeSystem.SetPipeDistance(pipeSystem.PipeDistance + deltaDistance);
        pipeSystem.SetPipeRadius(pipeSystem.CalculatePipeRadius(pipeSystem.PipeDistance));
    }

    void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f, true);//-pipeSystem.PipeRadChange);
        Vector3 vertexB = GetPointOnTorus(u, 0f, true);

        //print(this.name + " " + pipeSystem.PipeRadius);

        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep, true);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep, true);
        }
        pipeSystem.SetPipeDistance(pipeSystem.PipeDistance + deltaDistance);
        pipeSystem.SetPipeRadius(pipeSystem.CalculatePipeRadius(pipeSystem.PipeDistance));
    }

    Vector3 GetPointOnTorus(float u, float v, bool firstQuad)
    {
        Vector3 p;
        float pipeRadius;

        if (firstQuad)
        {
            pipeRadius = lastQuadRadius;
            //print("first " + pipeRadius);
        }
        else
            pipeRadius = (pipeSystem.PipeRadius);

        float r = (curveRadius + pipeRadius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);

        if (lastQuad)
        {
            lastQuadRadius = pipeRadius;
            //print("last " + lastQuadRadius);
        }

        return p;
    }

    public void AlignWith (Pipe pipe)
    {
        relativeRotation = Random.Range(0, curveSegmentCount) * 360f / pipeSegmentCount;
        print(relativeRotation);

        transform.SetParent(pipe.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.curveAngle);
        transform.Translate(0f, pipe.curveRadius, 0f);
        transform.Rotate(relativeRotation, 0f, 0f);
        transform.Translate(0f, -curveRadius, 0f);
        transform.localScale = Vector3.one;
        transform.SetParent(pipe.transform.parent);
    }

    //Getters
    public float CurveRadius
    {
        get
        {
            return curveRadius;
        }
    }

    public float CurveAngle
    {
        get
        {
            return curveAngle;
        }
    }

    public float RelativeRotation
    {
        get
        {
            return relativeRotation;
        }
    }

    public float ArcLength
    {
        get
        {
            return arcLength;
        }
    }
}
