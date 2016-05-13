using UnityEngine;
using System.Collections;

public class LaserEnemy : FollowerShip
{
    public Laser gun;
    public Transform turret;
    public float aimingTime, chargingDuration, shootingDuration, shootingRange;
    private bool playerInRange, aiming, shooting;

    // Use this for initialization
    void Start()
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
    void Update()
    {

        UpdateAvatarAttitude();
        CheckForLooping();
        CalculateSpeed();

        if (playerInRange && !aiming && !shooting)
        {
            print("In Range");
            StopCoroutine("TryForLaser");
            StartCoroutine("TryForLaser");
        }
        if (shooting)
            UpdateAvatarRotation(Mathf.Lerp(SeekTarget(player.AvatarRotation), 0f, Mathf.Clamp01(2f * time / chargingDuration)));
        else
            UpdateAvatarRotation(SeekTarget(player.AvatarRotation));
        UpdateAvatarLocation();
    }

    IEnumerator TryForLaser()
    {
        time = 0f;
        aiming = true;
        while (playerInRange)
        {
            time += Time.deltaTime;

            if (time > aimingTime)
            {
                //gun.FireLaser(chargingDuration, shootingDuration);
                shooting = true;
                break;
            }

            yield return null;
        }
        aiming = false;
    }

    override public float SeekTarget(float targetRotation)
    {
        float degreesToTarget = DegreesToTarget(avatarRotation, targetRotation), lateralValue = 0f, degToDist = 2f * Mathf.PI * shipRadius / 360f;

        if (Mathf.Abs(degreesToTarget) * degToDist < shootingRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (Mathf.Abs(degreesToTarget) * degToDist < 0.001f)
            return 0f;

        else
        {
            lateralValue = degreesToTarget * degToDist;
        }

        return lateralValue;
    }

    public void SetShooting(bool state)
    {
        shooting = state;
    }

    public bool Shooting
    {
        get
        {
            return shooting;
        }
    }

    public float ChargingTime
    {
        get
        {
            return chargingDuration;
        }
    }

    public float ShootingDuration
    {
        get
        {
            return shootingDuration;
        }
    }
}
