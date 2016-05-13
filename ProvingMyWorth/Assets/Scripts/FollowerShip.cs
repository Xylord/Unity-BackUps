using UnityEngine;
using System.Collections;

public class FollowerShip : SplineWalker {

    public PlayerShip player;
    public float time;

    // Use this for initialization
    void Start () {
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

    }
	
	// Update is called once per frame
	void Update () {

        UpdateAvatarAttitude();
        CheckForLooping();
        CalculateSpeed();

        UpdateAvatarRotation(SeekTarget(player.AvatarRotation));
        UpdateAvatarLocation();
    }

    /*IEnumerator TryForLaser()
    {
        time = 0f;
        aiming = true;
        while (playerInRange)
        {
            time += Time.deltaTime;

            if(time > aimingTime)
            {
                //gun.FireLaser(chargingDuration, shootingDuration);
                shooting = true;
                break;
            }

            yield return null;
        }
        aiming = false;
    }*/

    

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

    public void CalculateSpeed()
    {
        if (progress < player.Progress - 100f)
            speed += 10f;

        else if (progress > player.Progress - 75f)
            speed -= 10f;

        else if (progress < player.Progress -75f && progress > player.Progress - 100f)
            speed = player.speed + Random.Range(-10f, 10f);
    }

    public float DegreesToTarget(float avatarRot, float targetRot)
    {
        float degreesToTarget = avatarRot - targetRot;

        if ((degreesToTarget > 0f && degreesToTarget < 180f) || (degreesToTarget < -180f))
        {
            if (degreesToTarget < -180f)
                degreesToTarget = -360f - degreesToTarget;
            else
                degreesToTarget = -degreesToTarget;
            
        }

        else if ((degreesToTarget > 180f) || (degreesToTarget < 0f && degreesToTarget > -180f))
        {
            if (degreesToTarget > 180f)
                degreesToTarget = 360f - degreesToTarget;
            else
                degreesToTarget = -degreesToTarget;
        }

        return degreesToTarget;
    }

    virtual public float SeekTarget(float targetRotation)
    {
        float degreesToTarget = DegreesToTarget(avatarRotation, targetRotation), lateralValue = 0f, degToDist = 2f * Mathf.PI * shipRadius / 360f;

        if (Mathf.Abs(degreesToTarget) * degToDist < 0.001f)
            return 0f;

        else
        {
            lateralValue = degreesToTarget * degToDist;
        }

        return lateralValue;
    }
}
