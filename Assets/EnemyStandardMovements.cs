using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStandardMovements : MonoBehaviour {

	private NavMeshAgent agent;
	private Rigidbody m_rigidbody;
	private Transform hero;
	
	private bool isChasing = false;
	private int pivot_direction = 1;
	private bool in_visu = false;
	
	private int speed_offset = 10;
	
	public int max_speed;			// Speed limit
	public int acceleration;		// Force vector applied to enemy, ponderate by distance
	public int rotation;			// Time taken to face Hero
	public int pivot_force;			// Force vector applied to enemy side when pivoting
	public int distance_max_radius;
	public int distance_min_radius;
    public int braking;             // Reduction applied to enemy velocity when approaching the Hero

	// Use this for initialization
	void Start () {
		m_rigidbody = GetComponent<Rigidbody> ();
		agent = GetComponent<NavMeshAgent>();
		hero = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		agent.updatePosition = false;
		//agent.updateRotation = false;
	}
	
	void FixedUpdate(){
		
	}
	
	
	void update_follow_state(bool new_visu_state){
		if(in_visu != new_visu_state){
			if (new_visu_state) { agent.isStopped = true; }
			else { agent.isStopped = false; }
			in_visu = new_visu_state;
		}
	}
	
	void apply_agent_movement(){
		m_rigidbody.velocity = new Vector3(
			agent.velocity.x,
			m_rigidbody.velocity.y,
			agent.velocity.z
		);
		agent.nextPosition = m_rigidbody.position;
	}
	
	void apply_orientation(){
		Vector3 direction = hero.position - transform.position;
		Quaternion to_rotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, to_rotation, rotation * Time.deltaTime);
	}
	
	void apply_pivot(float distance)
    {
        if (distance < distance_max_radius)
        {
            m_rigidbody.AddForce((transform.right * pivot_direction + transform.forward)  * pivot_force * Time.deltaTime * 10);
            Debug.DrawRay(transform.position, (transform.right * pivot_direction + transform.forward) * pivot_force * Time.deltaTime * 10, Color.yellow);
        }
	}
	
    void apply_forward_movement(float distance)
    {

        // Calculate max_speed limit factor
        float acceleration_factor = max_speed - m_rigidbody.velocity.magnitude;

        // Calculate distance factor reduction


        float middle_radius = (distance_max_radius + distance_min_radius) / 2;
        float radius_segment = (distance_max_radius - distance_min_radius) / 2;
        float radius_weighting = (distance - middle_radius) / radius_segment;

        if (radius_weighting < 1)
        {
               /*
            // If enemy is coming too fast, apply a little brake
            Debug.Log(m_rigidbody.velocity.magnitude);
            if ((m_rigidbody.velocity.magnitude > (max_speed / 2)) && (acceleration_factor > 0))
            {
                acceleration_factor *= -1;
            }
            else
            {
                acceleration_factor *= radius_weighting * 0.1f;
            }*/
            acceleration_factor *= radius_weighting; 
        }
        


        // Apply final force vector
        m_rigidbody.AddForce(transform.forward * acceleration_factor * Time.deltaTime * 10);
        Debug.DrawRay(transform.position, transform.forward * acceleration_factor * Time.deltaTime * 10, Color.cyan);

 
        if (distance < distance_max_radius)
        {
            m_rigidbody.velocity = new Vector3(
                m_rigidbody.velocity.x * (1 - (braking / 1000.0f)),
                m_rigidbody.velocity.y,
                m_rigidbody.velocity.z * (1 - (braking / 1000.0f))
            );
        }

        /*m_rigidbody.velocity = new Vector3(
            Mathf.Clamp(m_rigidbody.velocity.x, -max_speed, max_speed),
            m_rigidbody.velocity.y,
            Mathf.Clamp(m_rigidbody.velocity.z, -max_speed, max_speed)
        );*/
    }

	void apply_independant_movement(Vector3 direction){
		agent.Warp(transform.position);

		float distance = direction.magnitude;
        //float force_vector_direction = (direction.normalized  * acceleration Time.deltaTime).sqrMagnitude;

        // Enemy is in interaction radius : we ponderate force vector depending of middle radius point.
        // We also allow pivot movement

        /*
		if (distance_max_radius > distance){
			float middle_radius = (distance_max_radius + distance_min_radius) / 2;
			float radius_segment = (distance_max_radius - distance_min_radius) / 2;
			float radius_ponderation = (distance - middle_radius) / radius_segment;
			Debug.Log("--");
			Debug.Log(radius_ponderation);
			apply_turning_movement(); 
		}
		Debug.Log("END");
		
		float force_to_apply = distance;
		
		float distance_to_hero = Vector3.Distance(hero.position, transform.position);//.sqrMagnitude;
		if (distance_to_hero < distance_min_radius){ force_to_apply = -force_to_apply; }
		if (distance_max_radius > distance_to_hero  && distance_to_hero > distance_min_radius){ apply_turning_movement(); }
		
		
		m_rigidbody.AddForce(
			transform.forward * Mathf.Clamp(force_to_apply, -max_speed, max_speed)
		);*/

        apply_orientation();
        apply_forward_movement(distance);
        apply_pivot(distance);
        SetChasingState(distance);


    }
	
	// Update is called once per frame
	void Update () {

		agent.destination = hero.position;
		Vector3 direction = (hero.position  - m_rigidbody.transform.position);

		RaycastHit hit;
		Ray ray = new Ray(transform.position, direction);
		
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.rigidbody != null)
			{
				//Debug.DrawRay(transform.position, direction, Color.red);
				update_follow_state(true);
				apply_independant_movement(direction);

			}
			else
			{
				//Debug.DrawRay(transform.position, direction, Color.green);
				update_follow_state(false);
				apply_agent_movement();
			}
			
		}
	
		else{

		}
		
	}
	
	void LateUpdate(){
		
	}
	
	public void InvertRotation() { pivot_direction *= (-1); }
    public bool GetChasingState() { return isChasing; }
    public void SetChasingState(float distance)
    {
        if (distance < distance_max_radius){ isChasing = true; }
        else { isChasing = false; }
    }
	
}
