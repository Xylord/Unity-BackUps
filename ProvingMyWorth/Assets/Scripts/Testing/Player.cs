using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;

    public float velocity, rotationVelocity, offsetFromPipe;

    private Pipe currentPipe;

    private float distanceTraveled, deltaToRotation, 
        systemRotation, worldRotation, avatarRotation, radiusFromPlayer;

    int radChangeCountDown;

    private Transform world, rotater, avatar;

	// Use this for initialization
	void Start () {
        print(Random.seed);

        world = pipeSystem.transform.parent;
        distanceTraveled = 0f;
        rotater = transform.GetChild(0);
        avatar = rotater.transform.GetChild(0);
        currentPipe = pipeSystem.SetupFirstPipe();
        SetupCurrentPipe();
        deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
	}
	
	// Update is called once per frame
	void Update () {
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta;
        systemRotation += delta * deltaToRotation;

        if (systemRotation >= currentPipe.CurveAngle)
        {
            delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
            currentPipe = pipeSystem.SetupNextPipe();
            SetupCurrentPipe();

            systemRotation = delta * deltaToRotation;
        }

        pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);

        radiusFromPlayer = pipeSystem.CalculatePipeRadius(distanceTraveled + pipeSystem.FirstPipeLengthOffset);
        UpdateAvatarLocation();
	}

    void SetupCurrentPipe ()
    {
        deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
        worldRotation += currentPipe.RelativeRotation;
        if (worldRotation < 0f)
        {
            worldRotation += 360f;
        }
        else if (worldRotation >= 360f)
        {
            worldRotation -= 360f;
        }
        world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
    }

    void UpdateAvatarLocation()
    {
        avatarRotation += Input.GetAxis("Horizontal") * rotationVelocity *
            Time.deltaTime * pipeSystem.minPipeRadius / radiusFromPlayer;
        print(avatarRotation + " " + pipeSystem.minPipeRadius);
        if (avatarRotation < 0f)
        {
            avatarRotation += 360f;
        }
        else if (avatarRotation >= 360f)
        {
            avatarRotation -= 360f;
        }
        rotater.localRotation = Quaternion.Euler(avatarRotation, 0f, 0f);
        //UpdatePipeRadius();
        avatar.localPosition = new Vector3(0, -radiusFromPlayer, 0);
    }

    void UpdatePipeRadius()
    {
        


    }
}
