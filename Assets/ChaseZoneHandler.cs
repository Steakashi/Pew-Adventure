using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseZoneHandler : MonoBehaviour {

    public EnemyStandardMovements script;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //if(other.GetComponent<Collider>().tag == "Player"){ script.beginChasing(); }
    }

    private void OnTriggerExit(Collider other)
    {
        //script.endChasing();
    }
}
