using UnityEngine;
using System.Collections;

public class PipeObject : MonoBehaviour {

    public BezierSpline spline;
    public SplinePipeSystem pipeSystem;

	// Use this for initialization
	void Start () {
        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GameObject curve = GameObject.Find("Curve");
        spline = curve.GetComponent<BezierSpline>();

        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public BezierSpline Spline
    {
        get
        {
            return spline;
        }
    }
}
