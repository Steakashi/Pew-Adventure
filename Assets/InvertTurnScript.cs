using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertTurnScript : MonoBehaviour {

    private EnemyStandardMovements script;

	// Use this for initialization
	void Start () { script = transform.parent.gameObject.GetComponent<EnemyStandardMovements>(); }
    private void OnTriggerEnter (Collider other) { script.InvertRotation(); }

}
