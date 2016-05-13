using UnityEngine;
using System.Collections;
using System;

public class BezierSpline : MonoBehaviour
{
    public int velocityAccuracy, curveNumber;

    public float levelScale, coneAngle, minSphereRadius, maxSphereRadius, 
        levelSize;

    public enum BezierControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }

    private float splineLength;

    [SerializeField]
    private Vector3[] points;

    //[SerializeField]
    private float[,] distanceLUT;

    [SerializeField]
    private BezierControlPointMode[] modes;

    [SerializeField]
    private bool loop;

    float pos = 24000f;

    void Update()
    {
        pos += 10f;
        if (pos > splineLength) pos = 24000f;
        //print(GetTForPosition(pos) + " " + pos + " " + splineLength);
    }

    public void Reset()
    {
        points = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(50f, 0f, 0f),
            new Vector3(100f, 0f, 0f),
            new Vector3(150f, 0f, 0f)
        };

        modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Aligned,
            BezierControlPointMode.Aligned
        };

        distanceLUT = new float[,]
        {
            { 0f, 0f },
            { 1f, 3f }
        };

        splineLength = 3f;
    }

    void Awake()
    {
        points = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(500f, 0f, 0f),
            new Vector3(1000f, 0f, 0f),
            new Vector3(1500f, 0f, 0f)
        };

        modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Aligned,
            BezierControlPointMode.Aligned
        };

        distanceLUT = new float[,]
        {
            { 0f, 0f },
            { 1f, 3f }
        };

        splineLength = 3f;

        Generate();
    }

    void Generate()
    {
        for (int i = 0; i < curveNumber - 1; i++)
        {
            if (i == curveNumber - 2)
            {
                Loop = true;
            }
            AddCurve();
        }
    }

    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    void Start()
    {

    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetPoint(points[i], 
            points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], 
            points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurve()
    {
        Vector3[] thisCurve = new Vector3[4];

        Vector3 point = points[points.Length - 1], velocity, pointMovement;
        thisCurve[0] = point;
        Array.Resize(ref points, points.Length + 3);

        //float tDelta = 1f / (points.Length / 3), tOnCurve = 1f - tDelta, angle, smallestAngle = 90f;


        //do
        //{
            //Vector3 dir;

            for (int i = 6; i > 3; i--)
            {
                velocity = point - points[points.Length - i];
                pointMovement = RandomPointInSphereExceptCone(coneAngle, minSphereRadius, maxSphereRadius, velocity);//coneAngle, minSphereRadius, maxSphereRadius, velocity);
                point += pointMovement;


                //if (point.magnitude > levelSize) point = Vector3.ClampMagnitude(point, levelSize);
                while (point.magnitude > levelSize)
                {
                    point -= pointMovement;
                    pointMovement = RandomPointInSphereExceptCone(coneAngle, minSphereRadius, maxSphereRadius, velocity);
                    point += pointMovement;
                }

                thisCurve[thisCurve.Length - (i - 3)] = point;
                points[points.Length - (i - 3)] = point;
            }

            /*while (tOnCurve < 1f)
            {
                dir = GetDirection(tOnCurve);
                angle = Vector3.Angle(dir, Vector3.up);
                if (angle < 90f && angle < smallestAngle)
                {
                    smallestAngle = angle;
                }
                else if (180f - angle < smallestAngle)
                {
                    smallestAngle = 180f - angle;
                }

                tOnCurve += tDelta / velocityAccuracy;
            }*/
        //} while (smallestAngle < 5f);



        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }

        distanceLUT = new float[2, velocityAccuracy * CurveCount];

        float deltaT = 1f / ((distanceLUT.Length / 2) - 1), t = 0f;
        splineLength = 0f;

        distanceLUT[0, 0] = t;
        distanceLUT[1, 0] = 0f;
        t += deltaT;
        splineLength += (GetPoint(t) - GetPoint(t - deltaT)).magnitude;

        for (int i = 1; i < distanceLUT.Length / 2; i++, t += deltaT, splineLength += (GetPoint(t) - GetPoint(t - deltaT)).magnitude)
        {
            //splineLength += (GetPoint(t) - GetPoint(t - deltaT)).magnitude;

            distanceLUT[0, i] = t;
            distanceLUT[1, i] = splineLength;
        }

        //print(t + "jiewohfewquinjif");
    }

    public float GetTForPosition(float progress)
    {
        float progressDelta = 0f,//progress - distanceLUT[1, 0],
            diffBetweenIndex, T;
        int beforeIndex;
        

        for(int i = 1; i < distanceLUT.Length / 2; i++)
        {
            if (progress - distanceLUT[1, i] < 0f)
            {
                beforeIndex = i - 1;

                progressDelta = progress - distanceLUT[1, beforeIndex];
                diffBetweenIndex = distanceLUT[1, beforeIndex + 1] - distanceLUT[1, beforeIndex];

                T = Mathf.Lerp(distanceLUT[0, beforeIndex],
                    distanceLUT[0, beforeIndex + 1],
                    progressDelta / diffBetweenIndex);
                return T;
            }
        }
        //print("OUPS");
        return 0f;
        
    }

    private void AddPoint(Vector3 point, int index)
    {
        Vector3 velocity, pointMovement;

        velocity = point - points[points.Length - index];
        pointMovement = RandomPointInSphereExceptCone(45f, 5f, 10f, velocity);//coneAngle, minSphereRadius, maxSphereRadius, velocity);
        point += pointMovement;
        points[points.Length - (index - 3)] = point;

    }

    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public float SplineLength
    {
        get
        {
            return splineLength;
        }
    }

    public int ControlPointCount
    {
        get
        {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint (int index)
    {
        return points[index];
    }

    public void SetControlPoint (int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];
            if (loop)
            {
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode (int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode (int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop)
        {
            if (modeIndex == 0)
            {
                modes[modes.Length - 1] = mode;
            }
            else if(modeIndex == modes.Length - 1)
            {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode (int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if(mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if(enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if(fixedIndex >= points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    private float NextGaussianDouble()
    {
        float u, v, D;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            D = u * u + v * v;
        }
        while (D >= 1.0);

        float fac = Mathf.Sqrt(-2.0f * Mathf.Log(D) / D);
        return u * fac;
    }

    private Vector3 RandomPointInSphereExceptCone(float coneAngle, float minSphereRadius, float maxSphereRadius, Vector3 velocity)
    {
        Vector3 point, xAxis = new Vector3 (1f, 0f, 0f);
        float radialAngle, linearAngle, linearAngleRange = 180f - coneAngle,
            sphereRadius;
        Quaternion rotation;
        
        rotation = Quaternion.FromToRotation(xAxis, velocity);

        radialAngle = UnityEngine.Random.Range(0f, 360f);
        linearAngle = linearAngleRange * (float)NextGaussianDouble() / 3f;
        sphereRadius = UnityEngine.Random.Range(minSphereRadius, maxSphereRadius);

        while (linearAngle > linearAngleRange || linearAngle < -linearAngleRange)
        {
            linearAngle = linearAngleRange * (float)NextGaussianDouble() / 3f;
        }

        point.x = sphereRadius * Mathf.Cos(linearAngle * Mathf.PI / 180f);
        point.y = sphereRadius * Mathf.Sin(radialAngle * Mathf.PI / 180f) * Mathf.Sin(linearAngle * Mathf.PI / 180f);
        point.z = sphereRadius * Mathf.Cos(radialAngle * Mathf.PI / 180f) * Mathf.Sin(linearAngle * Mathf.PI / 180f);

        point = rotation * point;

        return point;
    }

    void OnDrawGizmos()
    {
        //float t = 0f;
        //int dots = 0;

        /*Vector3 point = GetPoint(GetTForPosition(splineLength - 10f));
        print(GetTForPosition(splineLength - 10f));
        Gizmos.DrawSphere(point, 5f);*/

        /*while (t < 1f)
        {

            Vector3 point = GetPoint(t);
            //print(GetTForPosition(progress));
            Gizmos.DrawSphere(point, 20f);

            t += 0.05f;
            dots++;
        }*/
    }
}
