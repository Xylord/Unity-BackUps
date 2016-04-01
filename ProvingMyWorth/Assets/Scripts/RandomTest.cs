using UnityEngine;
using System.Collections;

public class RandomTest : MonoBehaviour {

    public int seed, min1, max1, min2, max2, result1, result2, result3;

	// Use this for initialization
	void Start () {
        Random.seed = seed;

        result1 = Random.Range(min1, max1);
        result2 = Random.Range(min2, max2);
        result3 = Random.Range(0, 100);
        print(this.name + result1 + " " + result2 + " " + result3);

        result1 = Random.Range(min1, max1);
        result2 = Random.Range(min2, max2);
        result3 = Random.Range(0, 100);
        print(this.name + result1 + " " + result2 + " " + result3);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
