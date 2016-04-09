using UnityEngine;
using System.Collections;

public class VaryPipeSys : MonoBehaviour {

    public VaryPipeRadius pipePrefab;

    public int pipeCount;

    static public float pipeRadius = 2f;

    public float minPipeRadius, maxPipeRadius;

    private VaryPipeRadius[] pipes;

    void Awake()
    {
        pipes = new VaryPipeRadius[pipeCount];
        for (int i = 0; i < pipes.Length; i++)
        {
            VaryPipeRadius pipe = pipes[i] = Instantiate<VaryPipeRadius>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }
    }


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
