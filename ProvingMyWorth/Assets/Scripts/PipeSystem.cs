using UnityEngine;
using System.Collections;

public class PipeSystem : MonoBehaviour {

    public Pipe pipePrefab;

    public int pipeCount;

    static public float pipeRadius;

    public float minPipeRadius, maxPipeRadius;

    private Pipe[] pipes;

    void Awake ()
    {
        pipeRadius = Random.Range(minPipeRadius, maxPipeRadius);
        pipes = new Pipe[pipeCount];
        for ( int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i] = Instantiate<Pipe>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            if ( i > 0)
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
