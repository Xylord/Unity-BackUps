using UnityEngine;
using System.Collections;

public class PipeObstacle : PipeObject {

    private Mesh mesh;
    private SplinePipe pipe;
    public Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private float spikeHeight;
    private int spikeIndex;

    // Use this for initialization
    void Start () {
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

    public void Generate(SplinePipe spawningPipe, int index)
    {
        pipe = spawningPipe;
        spikeIndex = index;
        spikeHeight = 20f;

        mesh.Clear();
        SetVertices();
        SetUVs();
        SetTriangles();
        mesh.RecalculateNormals();
    }

    void SetVertices()
    {
        Vector3 norm, center = Vector3.zero;
        vertices = new Vector3[5];

        /*for (int i = 0; i < 4; i++)
        {
            vertices[i] = pipe.spikeVertices[i, spikeIndex];
        }*/

        norm = Vector3.Cross((vertices[1] - vertices[0]), (vertices[2] - vertices[0])).normalized;

        for(int i = 0; i < 4; i++)
        {
            center += vertices[i];
        }

        center /= 4f;

        vertices[4] = center + norm * spikeHeight;
    }

    void SetUVs()
    {

    }

    void SetTriangles()
    {

    }

    public void SetPipe(SplinePipe setPipe)
    {
        pipe = setPipe;
    }

}
