using UnityEngine;
using System.Collections;

public class SplineWalker : PipeObject{

    public float minSpeed, speed, acceleration;
    public float progress, shipRadius, 
        lateralSpeed, avatarRotation, 
        offsetFromGrid, maxLateralSpeed, spawnpoint;
    public Transform rotater, avatar;
    public Rigidbody rigidBody;
    public bool lookForward;

	// Use this for initialization
	void Start () {
        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GameObject curve = GameObject.Find("Curve");
        spline = curve.GetComponent<BezierSpline>();

        if (gameObject.GetComponent<Rigidbody>())
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
        }

        rotater = transform.GetChild(0);
        avatar = rotater.transform.GetChild(0);

        progress = spawnpoint;
        lookForward = true;
    }
	
	// Update is called once per frame
	void Update () {
        CheckForLooping();
        UpdateAvatarRotation(0f);
        UpdateAvatarLocation();
	}

    public bool CheckForLooping()
    {
        if (progress > spline.SplineLength)
        {
            progress = 0f;
            return true;
        }
        return false;
    }

    virtual public void UpdateAvatarAttitude()
    {
        if (gameObject.GetComponent<Rigidbody>())
        {
            avatar.localRotation = Quaternion.LookRotation(rigidBody.velocity, -avatar.localPosition);
        }
    }

    virtual public void UpdateAvatarRotation(float lateralMovement)
    {
        UpdateAvatarAttitude();

        shipRadius = pipeSystem.GetRadius(progress);
        lateralSpeed += lateralMovement;

        Mathf.Clamp(lateralSpeed, -maxLateralSpeed, maxLateralSpeed);

        avatarRotation += lateralSpeed *
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

    /*public float GetOffsetFromGrid(float progress)
    {
        float offset = 0;

        pipeSystem.GetRadius(progress) - pipeSystem.GetRadius(progress - 0.001f);
    }*/

    public Vector3 PositionForProgress(float instantProgress)
    {
        float T = spline.GetTForPosition(instantProgress);

        Vector3 position = spline.GetPoint(T);

        return position;
    }

    public float RadiusForProgress(float instantProgress, float instantRotation)
    {
        float radius = pipeSystem.GetRadius(instantProgress);

        float lerpTemp = instantRotation / (360f / (2f * pipeSystem.RadiusSegmentCount)),
            inRadius = radius * Mathf.Cos(Mathf.PI / pipeSystem.RadiusSegmentCount);
        bool even = false;

        if ((int)lerpTemp % 2 == 0)
        {
            while (lerpTemp > 1f)
            {
                lerpTemp--;
            }
            even = true;
        }
        else if ((int)lerpTemp % 2 == 1)
        {
            while (lerpTemp > 1f)
            {
                lerpTemp--;
            }
            even = false;
        }

        if (even)
            lerpTemp = 1f - lerpTemp;

        lerpTemp *= lerpTemp;
        radius = Mathf.Lerp(inRadius, radius, lerpTemp) - offsetFromGrid;

        return radius;
    }

    public void UpdateAvatarLocation()
    {
        progress += Time.deltaTime * speed;

        Vector3 position = PositionForProgress(progress);

        avatar.localPosition = new Vector3(0, -RadiusForProgress(progress, avatarRotation), 0);

        transform.localPosition = position;
        float T = spline.GetTForPosition(progress);

        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(T));
        }
    }

    public float AvatarRotation
    {
        get
        {
            return avatarRotation;
        }
    }

    public float Progress
    {
        get
        {
            return progress;
        }
    }
}
