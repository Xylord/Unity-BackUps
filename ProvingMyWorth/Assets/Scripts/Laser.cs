using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    public int laserSegments;
    public float maxLength;
    private LineRenderer line;
    private FollowerShip ship;
    private float chargeTime, shootTime;
    private Vector3[] positions;
    private GameObject center, rotater, point;

    // Use this for initialization
    void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;

        positions = new Vector3[laserSegments];

        center = new GameObject();
        rotater = new GameObject();
        point = new GameObject();

        rotater.transform.SetParent(center.transform);
        point.transform.SetParent(rotater.transform);

        ship = transform.parent.parent.parent.parent.GetComponent<FollowerShip>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonDown("Fire1"))
            {

            chargeTime = 2;
            shootTime = 2;
            print("gotchu");
            StopCoroutine("ShootLaser");
            StartCoroutine("ShootLaser");
            }
	}

    public void FireLaser(float chargingDuration, float shootingDuration)
    {
        /*chargeTime = chargingDuration;
        shootTime = shootingDuration;
        StopCoroutine("ShootLaser");
        StartCoroutine("ShootLaser");*/
    }

    IEnumerator ShootLaser()
    {
        float time = 0f, segmentLength = maxLength / laserSegments, progress = 0f;
        //Vector3[] positions = new Vector3[laserSegments];
        ship.SetShooting(true);

        line.enabled = true;

        line.SetVertexCount(laserSegments);

        while (time < chargeTime + shootTime)
        {
            time += Time.deltaTime;

            positions[0] = transform.position;

            progress = ship.Progress;

            for (int i = 1; i < positions.Length; i++, progress += segmentLength)
            {
                center.transform.position = ship.PositionForProgress(progress);
                center.transform.LookAt(center.transform.position + ship.Spline.GetDirection(ship.Spline.GetTForPosition(progress)));

                rotater.transform.localRotation = Quaternion.Euler(0f, 0f, ship.AvatarRotation + 180f);

                point.transform.localPosition = new Vector3(0, ship.RadiusForProgress(progress, ship.AvatarRotation), 0);
                
                positions[i] = point.transform.position;
            }
            line.SetPositions(positions);
            /*Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000))
                line.SetPosition(1, hit.point);

            else
                line.SetPosition(1, ray.GetPoint(10000));*/

            //line.SetPosition(0, transform.position);// ray.origin);

            if (time < chargeTime)
            {
                line.SetWidth(0.25f, 0.25f);
            }

            else
            {
                line.SetWidth(3f, 3f);
            }

            yield return null;
        }

        ship.SetShooting(false);
        line.enabled = false;
        /*line.enabled = true;
        while (Input.GetButton("Fire1"))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000))
                line.SetPosition(1, hit.point);
            
            else
                line.SetPosition(1, ray.GetPoint(10000));

            line.SetPosition(0, ray.origin);
            //line.SetPosition(1, ray.GetPoint(100));

            yield return null;
        }

        line.enabled = false;*/
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i], 1f);
        }
    }
}
