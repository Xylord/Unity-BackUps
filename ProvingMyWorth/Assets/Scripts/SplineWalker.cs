using UnityEngine;
using System.Collections;

public class SplineWalker : PipeObject{

    public float minSpeed, speed;
    public float progress, shipRadius, 
        lateralSpeed, avatarRotation, 
        offsetFromGrid, maxLateralSpeed, spawnpoint;
    public Transform rotater, avatar;
    public bool lookForward;

	// Use this for initialization
	void Start () {
        GameObject mainPipeSystem = GameObject.Find("SplinePipeSystem");
        pipeSystem = mainPipeSystem.GetComponent<SplinePipeSystem>();

        GameObject curve = GameObject.Find("Curve");
        spline = curve.GetComponent<BezierSpline>();

        rotater = transform.GetChild(0);
        avatar = rotater.transform.GetChild(0);

        progress = spawnpoint;
        lookForward = true;
    }
	
	// Update is called once per frame
	void Update () {
        CheckForLooping();
        UpdateAvatarLocation(0f);
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

    public void UpdateAvatarLocation(float lateralMovement)
    {
        progress += Time.deltaTime * speed;
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

        float lerpTemp = avatarRotation / (360f / (2f * pipeSystem.RadiusSegmentCount)),
            inRadius = shipRadius * Mathf.Cos(Mathf.PI / pipeSystem.RadiusSegmentCount);
        bool even = false;

        if ((int)lerpTemp % 2 == 0)
        {
            while(lerpTemp > 1f)
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
        shipRadius = Mathf.Lerp(inRadius, shipRadius, lerpTemp) - offsetFromGrid;

        rotater.localRotation = Quaternion.Euler(0f, 0f, avatarRotation);
        avatar.localPosition = new Vector3(0, -shipRadius, 0);

        float T = spline.GetTForPosition(progress);

        Vector3 position = spline.GetPoint(T);
        transform.localPosition = position;

        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(T));
        }
    }
}
