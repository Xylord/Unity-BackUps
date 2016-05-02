using UnityEngine;
using System.Collections;

public class PipeObstacle : PipeObject {

    private Mesh mesh;
    //private MeshCollider meshCollider;
    private SplinePipe pipe;
    public Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private float spikeHeight;
    private int spikeIndex;

    // Use this for initialization
    void Awake () {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Spike";

        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GameObject curve = GameObject.Find("Curve");
        spline = curve.GetComponent<BezierSpline>();



        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Generate(int index)
    {
        spikeIndex = index;

        mesh.Clear();
        SetVertices();
        SetUVs();
        SetTriangles();
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void SetVertices()
    {
        vertices = new Vector3[12];

        /*for (int i = 0; i < 4; i++)
        {
            vertices[i] = pipe.spikeVertices[i, spikeIndex];
            vertices[i + 1] = pipe.spikeVertices[4, spikeIndex];
            vertices[i + 2] = pipe.spikeVertices[i + 1, spikeIndex];
        }*/

        vertices[0] = pipe.spikeVertices[0, spikeIndex];
        vertices[1] = pipe.spikeVertices[4, spikeIndex];
        vertices[2] = pipe.spikeVertices[1, spikeIndex];

        vertices[3] = pipe.spikeVertices[1, spikeIndex];
        vertices[4] = pipe.spikeVertices[4, spikeIndex];
        vertices[5] = pipe.spikeVertices[3, spikeIndex];

        vertices[6] = pipe.spikeVertices[2, spikeIndex];
        vertices[7] = pipe.spikeVertices[4, spikeIndex];
        vertices[8] = pipe.spikeVertices[0, spikeIndex];

        vertices[9] = pipe.spikeVertices[3, spikeIndex];
        vertices[10] = pipe.spikeVertices[4, spikeIndex];
        vertices[11] = pipe.spikeVertices[2, spikeIndex];

        mesh.vertices = vertices;
    }

    void SetUVs()
    {
        uvs = new Vector2[vertices.Length];
        /* (int i = 0; i < vertices.Length; i += 4)
        {
            uvs[i] = Vector2.zero;
            uvs[i + 1] = Vector2.right;
            uvs[i + 2] = Vector2.up;
            uvs[i + 3] = Vector2.one;
        }*/

        for (int i = 0; i < vertices.Length; i += 3)
        {
            uvs[i] = new Vector2(0f, 0f);
            uvs[i + 1] = new Vector2(1f, 0f);
            uvs[i + 2] = new Vector2(0f, 1f);
        }

        /*uvs[0] = new Vector2(0f, 0f);
        uvs[1] = new Vector2(1f, 0f);
        uvs[2] = new Vector2(0.5f, 1f);*/

        mesh.uv = uvs;
    }

    void SetTriangles()
    {
        triangles = new int[12];

        for(int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }

        mesh.triangles = triangles;
    }

    public void SetPipe(SplinePipe setPipe)
    {
        pipe = setPipe;
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //Gizmos.DrawSphere(vertices[4 * pipeSystem.RadiusSegmentCount + 4], 1f);
        float size = 0.7f;
        for(int i = 0; i < 12; i += 4)
        {
            Gizmos.DrawSphere(vertices[i], size);
            Gizmos.color = Color.yellow;
            size += 0.1f;
            Gizmos.DrawSphere(vertices[i + 1], size);
            Gizmos.color = Color.blue;
            size += 0.1f;
            Gizmos.DrawSphere(vertices[i + 2], size);
            Gizmos.color = Color.green;
            size += 0.1f;
        }
    }*/
}
