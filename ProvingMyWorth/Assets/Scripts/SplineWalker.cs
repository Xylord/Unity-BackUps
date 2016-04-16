using UnityEngine;
using System.Collections;

public class SplineWalker : MonoBehaviour{

    public float speed;
    public BezierSpline spline;
    public SplinePipeSystem pipeSystem;
    public float progress, shipRadius, lateralSpeed, avatarRotation;
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

        progress = pipeSystem.PlayerStart;
        lookForward = true;
        speed = 300f;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateAvatarLocation(0f);
	}

    public void UpdateAvatarLocation(float lateralMovement)
    {
        progress += Time.deltaTime * speed;
        shipRadius = pipeSystem.GetRadius(progress);

        avatarRotation += lateralMovement *
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

        print(lerpTemp);

        if ((int)lerpTemp % 2 == 0)
        {
            while(lerpTemp > 1f)
            {
                lerpTemp--;
                even = true;
            }
        }
        else if ((int)lerpTemp % 2 == 1)
        {
            while (lerpTemp > 1f)
            {
                lerpTemp--;
                even = false;
            }
        }

        lerpTemp *= lerpTemp;

        shipRadius = even ? Mathf.Lerp(inRadius, shipRadius, 1f - lerpTemp) : Mathf.Lerp(inRadius, shipRadius, lerpTemp);

        print(even);

        rotater.localRotation = Quaternion.Euler(0f, 0f, avatarRotation);
        avatar.localPosition = new Vector3(0, -shipRadius, 0);

        float T = spline.GetTForPosition(progress);

        if (progress > spline.SplineLength)
        {
            progress = 0f;
        }

        Vector3 position = spline.GetPoint(T);
        transform.localPosition = position;

        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(T));
        }
    }
}
