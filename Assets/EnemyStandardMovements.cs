using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStandardMovements : MonoBehaviour {

    private Rigidbody m_rigidbody;
    private Transform hero;
    private bool isChasing = false;
	private bool isTurningAround = false;
	private NavMeshAgent agent;
	private float beginToDecelerate;
	private float speedRatio = 1;
	private float escapeSpeedRatio = 0;
	private NavMeshAgent navMesh;
	private float speedNavStoredValue;
	private float turnSpeedNavStoredValue;
	private int turnDirection = 1;

    public float strength;
    public float speed;
	public float distanceFromHero;
	public float distanceLimit;
	public float distanceMin;

    // Use this for initialization
    void Start () {

        hero = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		m_rigidbody = GetComponent<Rigidbody> ();
		agent = GetComponent<NavMeshAgent>();
		beginToDecelerate = distanceLimit * 2f;
		speedNavStoredValue = agent.speed;
		turnSpeedNavStoredValue = agent.angularSpeed;
		agent.updatePosition = false;
		agent.updateRotation = false;
		//agent.updateUpAxis = false;
	}

	void Update(){

        agent.destination = hero.position;
		m_rigidbody.transform.position = agent.nextPosition;
		transform.LookAt(
			new Vector3(
				hero.transform.position.x, 
				m_rigidbody.transform.position.y,
				hero.transform.position.z 
			)
		);
    
		Debug.Log(agent.transform.position);

        if (agent.remainingDistance < distanceFromHero)
		{
            agent.isStopped = false;
            isChasing = true;

			if (agent.remainingDistance < beginToDecelerate) {

				isTurningAround = true;

			} 
			else 
			{
				isTurningAround = false;
				speedRatio = 1;
			}
				
		} 
		else
		{
			isChasing = false;
            agent.isStopped = true;
		}

	}
	
	// Update is called once per frame
	void LateUpdate () {
/*
        if (isChasing)
        {
			
			if (isTurningAround) {

				if (agent.remainingDistance < distanceLimit) {
					speedRatio = 0;
				} else {
					speedRatio = (agent.remainingDistance - distanceLimit) / (beginToDecelerate - distanceLimit);
				}
					
				m_rigidbody.AddForce (transform.right * Time.deltaTime * 800 * (1 - speedRatio) * turnDirection);

				if (agent.remainingDistance < distanceMin) {

					escapeSpeedRatio = 1 - ((agent.remainingDistance) / (distanceMin));

				
					m_rigidbody.AddForce ((transform.position - hero.position) * 100 * escapeSpeedRatio);

				}

					
			}



            m_rigidbody = GetComponent<Rigidbody>();
			m_rigidbody.AddForce(transform.forward * Time.deltaTime * 80 * speed * speedRatio);

        }

*/

    }


    public void InvertRotation() { turnDirection *= (-1); }
    public bool GetChasingState() { return isChasing; }

    //public void beginChasing(){ isChasing = true; }
    //public void endChasing(){ isChasing = false; }

}
