using UnityEngine;
using System.Collections;

public class SplineWalker : MonoBehaviour {

    public BezierSpline spline;

    public float speed;

    public bool lookForward;

    private float progress;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos1, pos2, delta;
        pos1 = transform.position;
        
        progress += Time.deltaTime * speed;

        float T = spline.GetTForPosition(progress);

        if (progress > spline.SplineLength)
        {
            progress = 0f;
        }

        Vector3 position = spline.GetPoint(T);
        transform.localPosition = position;

        pos2 = transform.position;

        delta = pos2 - pos1;

        print(delta.magnitude);

        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(T));
        }
	}
}
