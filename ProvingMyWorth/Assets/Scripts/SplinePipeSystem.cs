using UnityEngine;
using System.Collections;

public class SplinePipeSystem : MonoBehaviour {

    public int pipeCount, radiusSegmentCount, curveSegmentCount, radiusVariationNum;
    public float ringDistance, radiusScale, minRadius, maxRadius;

    public Material material;
    public BezierSpline spline;
    public SplinePipe pipePrefab;
    public AnimationCurve radiusCurve;

    private SplinePipe[] pipes;
    private float position, playerStart;
    private int lastPipe, generatedPipes;

    void Awake ()
    {
        playerStart = (float)pipeCount / 2f * curveSegmentCount * ringDistance;

        GenerateRadiusCurve();

        pipes = new SplinePipe[pipeCount];
        float pipePosition;

        for (int i = 0; i < pipes.Length; i++)
        {
            float offset;

            SplinePipe pipe = pipes[i] = Instantiate<SplinePipe>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            pipePosition = i * (curveSegmentCount - 1) * ringDistance;

            if (pipePosition > spline.SplineLength)
            {
                offset = pipePosition - (int)(pipePosition / spline.SplineLength) * spline.SplineLength;
                pipe.Generate(offset);
                continue;
            }

            pipe.Generate(pipePosition);
        }
        generatedPipes = pipes.Length;
        lastPipe = 0;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public SplinePipe SetupNextPipe()
    {
        float pipePosition;
        pipePosition = generatedPipes * (curveSegmentCount - 1) * ringDistance;

        if (pipePosition > spline.SplineLength)
        {
            pipePosition = pipePosition - (int)(pipePosition / spline.SplineLength) * spline.SplineLength;
            
        }

        pipes[lastPipe].Generate(pipePosition);

        lastPipe++;

        if (lastPipe == pipes.Length)
            lastPipe = 0;

        generatedPipes++;

        return pipes[lastPipe];
    }

    void GenerateRadiusCurve()
    {
        Keyframe[] keyFrames = new Keyframe[radiusVariationNum];

        keyFrames[0].time = 0f;
        keyFrames[0].value = 0.5f;
        keyFrames[keyFrames.Length - 1].time = spline.SplineLength;
        keyFrames[keyFrames.Length - 1].value = 0.5f;

        float delta = spline.SplineLength / keyFrames.Length;

        for (int i = 1; i < keyFrames.Length - 1; i++)
        {
            keyFrames[i].time = i * delta;
            keyFrames[i].value = Random.Range(minRadius, maxRadius);
        }

        radiusCurve.postWrapMode = WrapMode.Clamp;

        radiusCurve.keys = keyFrames;
    }

    public float GetRadius(float progress)
    {
        /*float quarterLength = spline.SplineLength / 4f;
        if (progress < quarterLength) return 10f;
        if (progress < 2f * quarterLength) return Mathf.Lerp(10f, 100f, (progress - quarterLength) / (2f * quarterLength - quarterLength));
        if (progress < 3f * quarterLength) return 100f;
        if (progress < 4f * quarterLength) return Mathf.Lerp(100f, 10f, (progress - 3f * quarterLength) / (4f * quarterLength - 3f * quarterLength));
        return 50f;*/
        //return 100f + Mathf.Sin(progress/100f) * 50f;

        return radiusCurve.Evaluate(progress) * radiusScale;
    }

    public float RadiusScale
    {
        get
        {
            return radiusScale;
        }
    }

    public int RadiusSegmentCount
    {
        get
        {
            return radiusSegmentCount;
        }
    }

    public int CurveSegmentCount
    {
        get
        {
            return curveSegmentCount;
        }
    }

    public float RingDistance
    {
        get
        {
            return ringDistance;
        }
    }

    public float PlayerStart
    {
        get
        {
            return playerStart;
        }
    }

    public SplinePipe FirstPipe()
    {
        return pipes[0];
    }

    void OnDrawGizmos()
    {
        /*for(int i = 0; i < pipes.Length; i++)
        {
            Gizmos.DrawSphere(spline.GetPoint(spline.GetTForPosition(pipes[i].StartPosition)), 10f);
        }*/
    }
}
