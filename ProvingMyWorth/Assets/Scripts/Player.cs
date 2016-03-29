using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;

    public float velocity;

    private Pipe currentPipe;

	// Use this for initialization
	void Start () {
        currentPipe = pipeSystem.SetupFirstPipe();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
