using UnityEngine;
using System.Collections;

public class PlayerShip : SplineWalker{

    public float acceleration, slowdownFactor;
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

        spawnpoint = pipeSystem.PlayerStart;
        progress = spawnpoint;
        currentPipe = pipeSystem.FirstPipe();
        traveledPipes = 0;
        pipePathLengthHalf = ((float)pipeSystem.pipeCount / 2f) * pipeSystem.RingDistance * pipeSystem.CurveSegmentCount;
        justLooped = false;
        lookForward = true;
        speed = minSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        float input;

        input = Input.GetAxis("Mouse X");

        UpdateAvatarLocation(input);
        CalculateSpeed();
        UpdatePipes();
	}

    void CalculateSpeed()
    {
        speed = speed + (acceleration - Mathf.Abs(lateralSpeed) * slowdownFactor) * Time.deltaTime;
        if (speed < minSpeed) speed = minSpeed;
    }

    void UpdatePipes()
    {
        if (CheckForLooping())
        {
            justLooped = true;
            print("justlooped " + spline.SplineLength);

        }

        //print(traveledPipes);

        

        if (progress - currentPipe.StartPosition > pipePathLengthHalf || (spline.SplineLength - currentPipe.StartPosition + progress > pipePathLengthHalf && justLooped))
        {

            print(currentPipe.name);
            currentPipe = pipeSystem.SetupNextPipe();
            print(currentPipe.name);
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
