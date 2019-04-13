using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicBullet : MonoBehaviour {

    private Rigidbody m_rigidbody;
    private string target;
    private bool slowMoState;
    private float slowMoValue = 1;

    public int speed;
    public int lifeTime;
    

    // Use this for initialization
    void Start () {
		
        m_rigidbody = GetComponent<Rigidbody>();
        
        Destroy(gameObject, lifeTime);

        if (GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<HeroController>().GetSlowMoState()) slowMoValue = 5;

        m_rigidbody.AddForce(transform.forward * 10 * speed * slowMoValue);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setTarget(string targetReceived)
    {
        target = targetReceived;

    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == target)
        {
            coll.GetComponent<HealthHandler>().ReceiveDamages(10);
            Destroy(gameObject);
        }
    }
}
