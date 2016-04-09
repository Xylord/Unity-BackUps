using UnityEngine;
using System.Collections;

public class SphereTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        /*float steps = 100;

        for (int v = 0; v < steps; v++)
        {
            Vector3 point = RandomPointInSphereExceptCone(45f, 20f, 20f);
            Gizmos.DrawSphere(point, 0.1f);
        }*/
    }

    public static float NextGaussianDouble()
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

    private Vector3 RandomPointInSphereExceptCone(float coneAngle, float minSphereRadius, float maxSphereRadius)
    {
        Vector3 point;
        float radialAngle, linearAngle, linearAngleRange = 180f - coneAngle,
            sphereRadius;

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


        return point;
    }

    private void OnDrawGizmos()
    {
        float steps = 10000;

        for (int v = 0; v < steps; v++)
        {
            Vector3 point = RandomPointInSphereExceptCone(45f, 20f, 30f);
            Gizmos.color = Color.red * (point.x + 20f) / 40f;
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
