using UnityEngine;
using System.Collections;
using System;

public class Laser : MonoBehaviour {

    public int laserSegments;
    public float maxLength, laserSizeStart, laserSizeEnd;

    //FERWHUFHGWIUE
    public bool hittingStuff;

    private LineRenderer line;
    private LaserEnemy ship;
    private float time;// chargeTime, shootTime, time;
    //private Vector3[] positions;
    private GameObject center, rotater, point;

    // Use this for initialization
    void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;

        //positions = new Vector3[laserSegments];

        center = new GameObject();
        rotater = new GameObject();
        point = new GameObject();
        center.name = "center";
        rotater.name = "rotater";
        point.name = "point";

        rotater.transform.SetParent(center.transform);
        point.transform.SetParent(rotater.transform);

        ship = transform.parent.parent.parent.parent.GetComponent<LaserEnemy>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonDown("Fire1"))
            {
            }
        if(time > ship.ShootingDuration + ship.ChargingTime)
        {
            ship.SetShooting(false);
            line.enabled = false;
            time = 0f;
        }
        else if (ship.Shooting)
        {
            //chargeTime = 2;
            //shootTime = 2;
            //time = 0f;
            ship.SetShooting(true);
            line.enabled = true;
            FireLaser(ship.ChargingTime, ship.ShootingDuration);
        }
	}

    public void FireLaser(float chargingDuration, float shootingDuration)
    {
        float segmentLength = maxLength / laserSegments, progress = 0f;
        
        time += Time.deltaTime;
        //Ray[] rays = new Ray[laserSegments + 1];
        Vector3[] positions = new Vector3[laserSegments + 1], truePositions;
        Vector3 lastPoint;
        int usedSegments = laserSegments;

        

        progress = ship.Progress;
        //positions[0] = ship.PositionForProgress(progress);

        for (int i = 0; i < positions.Length; i++, progress += segmentLength)
        {
            center.transform.position = ship.PositionForProgress(progress);
            center.transform.LookAt(center.transform.position + ship.Spline.GetDirection(ship.Spline.GetTForPosition(progress)));

            rotater.transform.localRotation = Quaternion.Euler(0f, 0f, ship.AvatarRotation + 180f);

            point.transform.localPosition = new Vector3(0, ship.RadiusForProgress(progress, ship.AvatarRotation), 0);

            /*Ray ray = new Ray(positions[i - 1], point.transform.position);
            RaycastHit hit;*/

            positions[i] = point.transform.position;

            /*if (Physics.Raycast(ray, out hit, Vector3.Distance(positions[i - 1], point.transform.position)))
            {
                usedSegments = i + 1;
                hittingStuff = true;
                positions[i] = hit.point;
                print("SWEGSHIT " + hit.collider.name + " " + usedSegments + " " + i);
                break;
            }

            else
            {
                usedSegments = i;
                hittingStuff = false;
                positions[i] = point.transform.position;
            }*/
        }

        for (int i = 0; i < positions.Length - 1; i++)
        {
            Ray ray = new Ray(positions[i], (positions[i + 1] - positions[i]).normalized);
            RaycastHit hit;
            Debug.DrawRay(positions[i], (positions[i + 1] - positions[i]).normalized, Color.white, Time.deltaTime);
            if (Physics.Raycast(ray, out hit, Vector3.Distance(positions[i], positions[i + 1])))
            {
                
                usedSegments = i + 1;
                hittingStuff = true;
                lastPoint = hit.point;
                positions[usedSegments - 1] = lastPoint;
                break;
            }
        }

        truePositions = new Vector3[usedSegments];

        for (int i = 0; i < truePositions.Length; i++)
        {
            truePositions[i] = positions[i];
        }

        line.SetVertexCount(usedSegments);
        //ray.Resize(ref positions, usedSegments);
        line.SetPositions(truePositions);
        /*Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000))
            line.SetPosition(1, hit.point);

        else
            line.SetPosition(1, ray.GetPoint(10000));*/

        //line.SetPosition(0, transform.position);// ray.origin);

        if (time < ship.ChargingTime)
        {
            line.SetWidth(laserSizeStart, laserSizeStart);
        }

        else
        {
            line.SetWidth(laserSizeEnd, laserSizeEnd);
        }
        

        
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

    /*IEnumerator ShootLaser()
    {
        float time = 0f, segmentLength = maxLength / laserSegments, progress = 0f;
        
        ship.SetShooting(true);

        line.enabled = true;

        //line.SetVertexCount(laserSegments);

        while (time < chargeTime + shootTime)
        {
            time += Time.deltaTime;
            //Ray[] rays = new Ray[laserSegments + 1];
            Vector3[] positions = new Vector3[laserSegments + 1], truePositions;
            Vector3 lastPoint;
            int usedSegments = laserSegments;

            positions[0] = transform.position;

            progress = ship.Progress;

            for (int i = 1; i < positions.Length; i++, progress += segmentLength)
            {
                center.transform.position = ship.PositionForProgress(progress);
                center.transform.LookAt(center.transform.position + ship.Spline.GetDirection(ship.Spline.GetTForPosition(progress)));

                rotater.transform.localRotation = Quaternion.Euler(0f, 0f, ship.AvatarRotation + 180f);

                point.transform.localPosition = new Vector3(0, ship.RadiusForProgress(progress, ship.AvatarRotation), 0);

                Ray ray = new Ray(positions[i - 1], point.transform.position);
                RaycastHit hit;

                positions[i] = point.transform.position;

                if (Physics.Raycast(ray, out hit, Vector3.Distance(positions[i - 1], point.transform.position)))
                {
                    usedSegments = i + 1;
                    hittingStuff = true;
                    positions[i] = hit.point;
                    print("SWEGSHIT " + hit.collider.name + " " + usedSegments + " " + i);
                    break;
                }

                else
                {
                    usedSegments = i;
                    hittingStuff = false;
                    positions[i] = point.transform.position;
                }
            }

            for (int i = 0; i < positions.Length - 1; i++)
            {
                Ray ray = new Ray(positions[i], positions[i + 1]);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Vector3.Distance(positions[i], positions[i + 1])))
                {
                    usedSegments = i + 1;
                    hittingStuff = true;
                    lastPoint = hit.point;
                    positions[usedSegments] = lastPoint;
                    break;
                }
            }

            truePositions = new Vector3[usedSegments];

            for (int i = 0; i < truePositions.Length; i++)
            {
                truePositions[i] = positions[i];
            }

            line.SetVertexCount(usedSegments);
            //ray.Resize(ref positions, usedSegments);
            line.SetPositions(truePositions);
            /*Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000))
                line.SetPosition(1, hit.point);

            else
                line.SetPosition(1, ray.GetPoint(10000));*/

            //line.SetPosition(0, transform.position);// ray.origin);

            /*if (time < chargeTime)
            {
                line.SetWidth(2f, 2f);
            }

            else
            {
                line.SetWidth(4f, 4f);
            }

            yield return null;
        }

        ship.SetShooting(false);
        line.enabled = false;
        
    }*/

    void OnDrawGizmos()
    {
        /*for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i], 1f);
        }*/
    }
}
