﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingScript : MonoBehaviour {

    public GameObject objectToFollow;
	
	// Update is called once per frame
	void Update () {
        transform.position = objectToFollow.transform.position + new Vector3(0,1,.5f);
    }
}
