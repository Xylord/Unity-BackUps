using UnityEngine;
using System.Collections;

public class PipeSystem : MonoBehaviour {

    public Pipe pipePrefab;

    public int pipeCount, maxRadChangeCount, 
        minPipeRadChange, maxPipeRadChange, 
        minPipeRadius, maxPipeRadius;

    //public bool noRadChange;
    //private float pipeRadius, pipeRadChange = 0f;

    private int pipeRadius, pipeRadChange;

    //public float pipeRadiusFactor, radiusChangeFactor;

    private Pipe[] pipes;

    //public Vector3[] verticesToPassOn;

    void Awake ()
    {
        int radChangeCountDown = maxRadChangeCount;

        //pipeRadius = Random.Range(minPipeRadius, maxPipeRadius) * pipeRadiusFactor;
        pipeRadius = Random.Range(minPipeRadius, maxPipeRadius);
        pipes = new Pipe[pipeCount];

        //verticesToPassOn = new Vector3[pipePrefab.pipeSegmentCount * 4];

        for ( int i = 0; i < pipes.Length; i++, radChangeCountDown--)
        {
            //int temp1 = pipeRadChange, temp2;

            if (radChangeCountDown == 0)
            {
                int ranNum = (int)Random.Range(0f, 99f);

                if (ranNum <= 82)
                    pipeRadChange = Random.Range(minPipeRadChange, maxPipeRadChange + 1);
                else if (ranNum > 32 && ranNum <= 65)
                    pipeRadChange = 0;
                //else print("NoChange");


                radChangeCountDown = maxRadChangeCount;
            }

            /*temp2 = pipeRadChange;

            if (temp1 != temp2 && pipeRadChange == 0)
                noRadChange = true;

            else
                noRadChange = false;*/

            Pipe pipe = pipes[i] = pipes[i] = Instantiate<Pipe>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            
            
            /*if (radChangeCountDown == 1)
            {
                pipeRadChange = maxPipeRadChange;
            }

            if (radChangeCountDown == 0)
            {
                pipeRadChange = minPipeRadChange;
                radChangeCountDown = 2;
            }*/

            if ( i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }
    }

    public Pipe SetupFirstPipe ()
    {
        transform.localPosition = new Vector3(0f, -pipes[0].maxCurveRadius);
        return pipes[0];
    }

    public void ChangePipeRadius ()
    {
        if ((pipeRadius + pipeRadChange >= minPipeRadius
            && pipeRadChange < 0) 
            || (pipeRadius + pipeRadChange <= maxPipeRadius && pipeRadChange > 0))//pipeRadius + pipeRadChange >= minPipeRadius && pipeRadius + pipeRadChange <= maxPipeRadius)
        {
            pipeRadius += pipeRadChange;
        }
        else if (pipeRadius + pipeRadChange <= minPipeRadius)
        {
            pipeRadius = minPipeRadius;
        }
        else if (pipeRadius + pipeRadChange >= maxPipeRadius)
        {
            pipeRadius = maxPipeRadius;
        }
    }

    public int PipeRadius
    {
        get
        {
            return pipeRadius;
        }
    }

    public void SetPipeRadius(int radius)
    {
            pipeRadius = radius;
    }

    public int PipeRadChange
    {
        get
        {
            return pipeRadChange;
        }
    }

    /*public void VerticesToPassOn(int i, Vector3 vertex)
    {
        verticesToPassOn[i] = vertex;
    }

    public Vector3 GetVerticesToPassOn(int i)
    {
        return verticesToPassOn[i];
    }*/

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
