using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceLevel : MonoBehaviour {

    public GameObject hero;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = hero.transform.position;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);

    }
}
