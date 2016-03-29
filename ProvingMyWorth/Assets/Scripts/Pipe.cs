using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour {

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float curveAngle;//, pipeRadVar;

    private float curveRadius;
    private int curveSegmentCount;

    private PipeSystem pipeSystem;

    //public float pipeRadius, ringDistance;
    public float ringDistance;
    public int pipeSegmentCount;

    public float minCurveRadius, maxCurveRadius,
        pipeRadiusFactor;//, pipeChangeFactor;//, minPipeRadius, maxPipeRadius;

    public int minCurveSegmentCount, maxCurveSegmentCount;

    static private int pipeNumber = 0;
    static private float lastQuadRadius;

    private bool lastQuad = false;

    //static private Vector3[] verticesToPassOn;

    // Use this for initialization
    void Start () {
        //verticesToPassOn = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake ()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        GameObject mainPipeSystem = GameObject.Find("PipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<PipeSystem>();

        //verticesToPassOn = new Vector3[pipeSegmentCount * curveSegmentCount * 4];

        this.name = "pipe" + pipeNumber.ToString();
        pipeNumber++;
        
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);

        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();
    }

    void SetVertices()
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
        //verticesToPassOn = new Vector3[pipeSegmentCount * curveSegmentCount * 4];

        float uStep = ringDistance / curveSegmentCount;
        curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
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

        pipeSystem.ChangePipeRadius();

        //print(this.name + " " + pipeSystem.PipeRadius + " " + pipeSystem.PipeRadChange);
        //pipeRadius -= 0.1f;
        //TRYHAARDALERT

    }

    void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f, true);//-pipeSystem.PipeRadChange);
        Vector3 vertexB = GetPointOnTorus(u, 0f, true);

        print(this.name + " " + pipeSystem.PipeRadius);

        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            

            /*if (pipeSystem.PipeRadius < pipeSystem.maxPipeRadius && pipeSystem.PipeRadius > pipeSystem.minPipeRadius
                && pipeSystem.PipeRadChange != 0)// && pipeSystem.noRadChange)
            {
                int signFactor = 0;

                if (pipeSystem.PipeRadChange > 0)
                {
                    signFactor = 1;
                    print(this.name + " neg");
                }
                else if (pipeSystem.PipeRadChange < 0)
                {
                    signFactor = -1;
                    print(this.name + " pos");
                }

                print("thing" + pipeSystem.noRadChange);
                signFactor = 1;

                vertices[i] = vertexA = GetPointOnTorus(0f, (v - 1) * vStep, -signFactor * pipeSystem.PipeRadChange);
                vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep, -signFactor * pipeSystem.PipeRadChange);
                vertices[i + 2] = vertexB = GetPointOnTorus(u, (v - 1) * vStep, -signFactor * pipeSystem.PipeRadChange);
                vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep, -signFactor * pipeSystem.PipeRadChange);

                vertexA = GetPointOnTorus(0f, v * vStep, 0);
                vertexB = GetPointOnTorus(u, v * vStep, 0);
            }
            else
            {*/

            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep, true);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep, true);
            //}
            //print(this.name + " first " + vertices[i] + " " + vertices[i + 1] + " " + vertices[i + 2] + " " + vertices[i + 3]);

        }

        pipeSystem.ChangePipeRadius();

        //print(this.name + " " + pipeSystem.PipeRadius + " " + pipeSystem.PipeRadChange);
    }

    Vector3 GetPointOnTorus(float u, float v, bool firstQuad)
    {
        Vector3 p;
        float pipeRadius;

        if (firstQuad)
            pipeRadius = lastQuadRadius;
        else
            pipeRadius = (pipeSystem.PipeRadius) * pipeRadiusFactor;

        float r = (curveRadius + pipeRadius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);

        if (lastQuad) lastQuadRadius = pipeRadius;

        return p;
    }

    public void AlignWith (Pipe pipe)
    {
        float relativeRotation = Random.Range(0, curveSegmentCount) * 360f / pipeSegmentCount;

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

    /*public void SetPipeSystem (PipeSystem foo)
    {
        pipeSystem = foo;
    }*/

    /*void OnDrawGizmos()
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f, 0f);//-PipeSystem.pipeRadChange);
        Vector3 vertexB = GetPointOnTorus(ringDistance / curveSegmentCount, 0f, 0f);
    

        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep, 0f);//-PipeSystem.pipeRadChange);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(ringDistance / curveSegmentCount, v * vStep, 0f);

            
        }
    }*/
}
