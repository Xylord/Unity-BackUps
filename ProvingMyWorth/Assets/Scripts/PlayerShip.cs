using UnityEngine;
using System.Collections;

public class PlayerShip : SplineWalker{

    private float pipePathLengthHalf;
    private SplinePipe currentPipe;
    private int traveledPipes;
    private bool justLooped;

    // Use this for initialization
    void Start () {
        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GameObject curve = GameObject.Find("Curve");
        spline = curve.GetComponent<BezierSpline>();

        rotater = transform.GetChild(0);
        avatar = rotater.transform.GetChild(0);

        progress = pipeSystem.PlayerStart;
        currentPipe = pipeSystem.FirstPipe();
        traveledPipes = 0;
        pipePathLengthHalf = ((float)pipeSystem.pipeCount / 2f) * pipeSystem.RingDistance * pipeSystem.CurveSegmentCount;
        justLooped = false;
        lookForward = true;
        speed = 300f;
    }
	
	// Update is called once per frame
	void Update () {
        float input;

        input = Input.GetAxis("Mouse X");

        UpdateAvatarLocation(input);
        UpdatePipes();
	}

    void UpdatePipes()
    {
        if (progress > spline.SplineLength)
        {
            progress = 0f;
            justLooped = true;
        }

        if (progress - currentPipe.StartPosition > pipePathLengthHalf || (spline.SplineLength - currentPipe.StartPosition + progress > pipePathLengthHalf && justLooped))
        {
            currentPipe = pipeSystem.SetupNextPipe();
            traveledPipes++;

            if (progress > currentPipe.StartPosition)
            {
                justLooped = false;
            }
        }
    }

    /*void UpdateAvatarLocation()
    {
        progress += Time.deltaTime * speed;
        shipRadius = pipeSystem.GetRadius(progress);
        
        float T = spline.GetTForPosition(progress);

        Vector3 position = spline.GetPoint(T);
        transform.localPosition = position;

        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(T));
        }
    }*/
}
