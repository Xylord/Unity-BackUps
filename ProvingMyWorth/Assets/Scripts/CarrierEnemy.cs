using UnityEngine;
using System.Collections;

public class CarrierEnemy : FollowerShip {

    public GameObject[] droneVariants;
    public int droneBaySize;
    private GameObject[] drones;

    // Use this for initialization
    void Start ()
    {
        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GameObject curve = GameObject.Find("Curve");
        spline = curve.GetComponent<BezierSpline>();

        GameObject playerShip = GameObject.Find("PlayerShip");
        player = playerShip.GetComponent<PlayerShip>();

        if (gameObject.GetComponent<Rigidbody>())
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
        }

        rotater = transform.GetChild(0);
        avatar = rotater.transform.GetChild(0);
        spawnpoint = pipeSystem.PlayerStart - 100f;
        progress = spawnpoint;
        lookForward = true;

        InitializeDroneBay();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void InitializeDroneBay()
    {
        drones = new GameObject[droneBaySize];

        for(int i = 0; i < drones.Length; i++)
        {

        }
    }
}
