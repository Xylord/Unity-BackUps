using UnityEngine;
using System.Collections;

public class FollowerShip : SplineWalker {

    private PlayerShip player;
    public Laser gun;
    public Transform turret;

    // Use this for initialization
    void Start () {
        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GameObject curve = GameObject.Find("Curve");
        spline = curve.GetComponent<BezierSpline>();

        GameObject playerShip = GameObject.Find("PlayerShip");
        player = playerShip.GetComponent<PlayerShip>();

        rotater = transform.GetChild(0);
        avatar = rotater.transform.GetChild(0);
        gun = avatar.GetChild(1).GetComponent<Laser>();
        turret = avatar.GetChild(1).transform;

        spawnpoint = pipeSystem.PlayerStart - 100f;
        progress = spawnpoint;
        lookForward = true;

    }
	
	// Update is called once per frame
	void Update () {
        CheckForLooping();
        CalculateSpeed();
        UpdateAvatarRotation(SeekPlayer());
        UpdateAvatarLocation();
    }

    override public void UpdateAvatarRotation(float lateralMovement)
    {
        shipRadius = pipeSystem.GetRadius(progress);
        lateralSpeed = 1f;// lateralMovement;

        /*if (lateralSpeed < 0f)
            lateralSpeed += 0.01f;

        else if (lateralSpeed > 0f)
            lateralSpeed -= 0.01f;*/

        //Mathf.Clamp(lateralSpeed, -maxLateralSpeed, maxLateralSpeed);
        avatarRotation += lateralSpeed * lateralMovement *
            Time.deltaTime * pipeSystem.RadiusScale / shipRadius;

        if (avatarRotation < 0f)
        {
            avatarRotation += 360f;
        }
        else if (avatarRotation >= 360f)
        {
            avatarRotation -= 360f;
        }

        rotater.localRotation = Quaternion.Euler(0f, 0f, avatarRotation);
    }

    void CalculateSpeed()
    {
        if (progress < player.Progress - 100f)
            speed += 10f;

        else if (progress > player.Progress - 75f)
            speed -= 10f;

        else if (progress < player.Progress -75f && progress > player.Progress - 100f)
            speed = player.speed + Random.Range(-10f, 10f);
    }

    float SeekPlayer()
    {
        float degreesToPlayer = avatarRotation - player.AvatarRotation, lateralValue = 0f;

        if (Mathf.Abs(degreesToPlayer) < 0.1f)
            return 0f;

        else if ((degreesToPlayer > 0f && degreesToPlayer < 180f) || (degreesToPlayer < -180f))
        {
            if (degreesToPlayer < -180f)
                lateralValue = - 360f - degreesToPlayer;
            else
                lateralValue = -degreesToPlayer;

            lateralValue -= 1f;
        }
        
        else if ((degreesToPlayer > 180f) || (degreesToPlayer < 0f && degreesToPlayer > -180f))
        {
            if (degreesToPlayer > 180f)
                lateralValue = 360f - degreesToPlayer;
            else
                lateralValue = -degreesToPlayer;

            lateralValue += 1f;
        }
            //return Mathf.Clamp(Mathf.Abs(degreesToPlayer), 0f, 90f);//Mathf.Lerp(0f, 0.01f, Mathf.Clamp01(degreesToPlayer / 90f));

        return lateralValue;
    }
}
