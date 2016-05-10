using UnityEngine;
using System.Collections;

public class FollowerShip : SplineWalker {

    private PlayerShip player;
    public Laser gun;
    public Transform turret;
    public float aimingTime, chargingDuration, shootingDuration, shootingRange;
    private bool playerInRange, aiming, shooting;

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
        turret = avatar.GetChild(1).transform;
        gun = turret.GetChild(1).GetComponent<Laser>();

        shooting = false;
        aiming = false;
        playerInRange = false;
        spawnpoint = pipeSystem.PlayerStart - 100f;
        progress = spawnpoint;
        lookForward = true;

    }
	
	// Update is called once per frame
	void Update () {

        UpdateAvatarAttitude();
        CheckForLooping();
        CalculateSpeed();

        if (playerInRange && !aiming && !shooting)
        {
            //print("In Range");
            StopCoroutine("TryForLaser");
            StartCoroutine("TryForLaser");
        }

        UpdateAvatarRotation(SeekPlayer());
        UpdateAvatarLocation();
    }

    IEnumerator TryForLaser()
    {
        float time = 0f;
        aiming = true;
        while (playerInRange)
        {
            time += Time.deltaTime;

            if(time > aimingTime)
            {
                gun.FireLaser(chargingDuration, shootingDuration);
                shooting = true;
                break;
            }

            yield return null;
        }
        aiming = false;
    }

    /*IEnumerator ShootLaser()
    {
        float time = 0f;
        shooting = true;

        while (time < chargingDuration + shootingDuration)
        {
            time += Time.deltaTime;

            if (time < chargingDuration)
            {

            }

            else
            {

            }

            yield return null;
        }

        shooting = false;
    }*/

    override public void UpdateAvatarRotation(float lateralMovement)
    {
        shipRadius = pipeSystem.GetRadius(progress);
        lateralSpeed = 1f;// lateralMovement;
        if (shooting)
        {
            lateralSpeed = 0f;
        }
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

    float DegreesToPlayer(float avatarRot, float playerRot)
    {
        float degreesToPlayer = avatarRot - playerRot;

        if ((degreesToPlayer > 0f && degreesToPlayer < 180f) || (degreesToPlayer < -180f))
        {
            if (degreesToPlayer < -180f)
                degreesToPlayer = -360f - degreesToPlayer;
            else
                degreesToPlayer = -degreesToPlayer;
            
        }

        else if ((degreesToPlayer > 180f) || (degreesToPlayer < 0f && degreesToPlayer > -180f))
        {
            if (degreesToPlayer > 180f)
                degreesToPlayer = 360f - degreesToPlayer;
            else
                degreesToPlayer = -degreesToPlayer;
        }

        return degreesToPlayer;
    }

    private float SeekPlayer()
    {
        float degreesToPlayer = DegreesToPlayer(avatarRotation, player.AvatarRotation), lateralValue = 0f, degToDist = 2f * Mathf.PI * shipRadius / 360f;

        if (Mathf.Abs(degreesToPlayer) * degToDist < shootingRange)
        {
            print("In Range" + Mathf.Abs(degreesToPlayer) * degToDist);
            playerInRange = true;
        }
        else
        {
            print("Not In Range" + Mathf.Abs(degreesToPlayer) * degToDist);
            playerInRange = false;
        }

        if (Mathf.Abs(degreesToPlayer) * degToDist < 0.001f)
            return 0f;

        else
        {
            lateralValue = degreesToPlayer * degToDist;
        }

        return lateralValue;
    }

    public void SetShooting(bool state)
    {
        shooting = state;
    }
}
