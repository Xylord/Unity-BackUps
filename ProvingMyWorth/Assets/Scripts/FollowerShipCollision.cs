using UnityEngine;
using System.Collections;

public class FollowerShipCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            print("gotchu");
            Explode();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //print("Collision!");
        Explode();
    }

    void Explode()
    {
        print("Pretty please");
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
    }
}
