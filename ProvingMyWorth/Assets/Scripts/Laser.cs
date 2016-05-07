using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    LineRenderer line;
    FollowerShip ship;

	// Use this for initialization
	void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;

        
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonDown("Fire1"))
            {
            StopCoroutine("FireLaser");
            StartCoroutine("FireLaser");
            }
	}

    IEnumerator FireLaser()
    {
        line.enabled = true;
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

        line.enabled = false;
    }
}
