using UnityEngine;
using System.Collections;

public class PipeSystem : MonoBehaviour {

    public Pipe pipePrefab;

    public Material pipeMaterial;

    public int pipeCount, seed;

    public float minPipeRadius, maxPipeRadius, 
        minPipeSteepness, maxPipeSteepness, 
        minSectionLength, maxSectionLength;

    //public bool noRadChange;
    //private float pipeRadius, pipeRadChange = 0f;

    //private int pipeRadius, pipeRadChange;

    //public float pipeRadiusFactor, radiusChangeFactor;

    private float pipeRadius, pipeDistance, firstPipeLengthOffset,
        minRadSigmoid, maxRadSigmoid, sigmoidSteepness, offset, sectionLength;

    private int sigmoidSign;

    private Pipe[] pipes;
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //print(pipeRadius);
	}

    void Awake ()
    {
        //Random.seed = seed;

        pipeRadius = Random.Range(minPipeRadius, maxPipeRadius);
        pipes = new Pipe[pipeCount];

        for ( int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i] = pipes[i] = Instantiate<Pipe>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            pipe.Generate();

            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }

        firstPipeLengthOffset = pipes[0].ArcLength;

        AlignNextPipeWithOrigin();
    }

    void ShiftPipes ()
    {
        Pipe temp = pipes[0];
        for (int i = 1; i < pipes.Length; i++)
        {
            pipes[i - 1] = pipes[i];
        }
        pipes[pipes.Length - 1] = temp;
    }

    void AlignNextPipeWithOrigin ()
    {
        Transform transformToAlign = pipes[1].transform;
        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transformToAlign);
            }
        }

        transformToAlign.localPosition = Vector3.zero;
        transformToAlign.localRotation = Quaternion.identity;

        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transform);
            }
        }
    }

    public Pipe SetupFirstPipe ()
    {
        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }

    public Pipe SetupNextPipe()
    {
        ShiftPipes();
        AlignNextPipeWithOrigin();
        pipes[pipes.Length - 1].Generate();
        pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);
        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);

        return pipes[1];
    }

    public float CalculatePipeRadius (float distance)
    {
        float radius;
        radius = 10f + 7.5f * Mathf.Sin(distance / 500f);

        return radius;
    }

    public float PipeRadius
    {
        get
        {
            return pipeRadius;
        }
    }

    public void SetPipeRadius (float value)
    {
        pipeRadius = value;
    }

    public float PipeDistance
    {
        get
        {
            return pipeDistance;
        }
    }

    public float FirstPipeLengthOffset
    {
        get
        {
            return firstPipeLengthOffset;
        }
    }

    public void SetPipeDistance(float value)
    {
        pipeDistance = value;
    }

    /*public void SetPipeRadChange (int value)
    {
        pipeRadChange = value;
    } 

    public int PipeRadChange
    {
        get
        {
            return pipeRadChange;
        }
    }*/
}
