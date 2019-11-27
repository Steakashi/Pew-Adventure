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
	
	private const int SPEED = 10;
	private const int PIVOT = 10;
	
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
	
	Vector3 apply_pivot(float distance)
    {
        if (distance < distance_max_radius)
        {
			return ((transform.right * pivot_direction + transform.forward)  * pivot_force * PIVOT);
			/*
            m_rigidbody.AddForce((transform.right * pivot_direction + transform.forward)  * pivot_force * Time.deltaTime * 10);
            Debug.DrawRay(transform.position, (transform.right * pivot_direction + transform.forward) * pivot_force * Time.deltaTime * 10, Color.yellow);
			*/
		}
		else
		{
			return Vector3.zero;
		}
	}
	
    Vector3 get_acceleration_movement(float distance)
    {

        // Calculate max_speed limit factor
        float acceleration_factor = max_speed - m_rigidbody.velocity.magnitude;

        // Calculate distance factor reduction
        float middle_radius = (distance_max_radius + distance_min_radius) / 2;
        float radius_segment = (distance_max_radius - distance_min_radius) / 2;
        float radius_weighting = (distance - middle_radius) / radius_segment;

        if (radius_weighting < 1)
        {
            acceleration_factor *= radius_weighting; 
        }
        return transform.forward * acceleration_factor * SPEED;
		//return (transform.forward * acceleration_factor * Time.deltaTime * SPEED);

        // Apply final force vector
		/*
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
		*/


    }
	
	bool obstacle_detected(Vector3 acceleration_force)
	{
	
		Vector3 next_position = m_rigidbody.transform.position + new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
		Debug.DrawRay(transform.position, next_position - m_rigidbody.transform.position, Color.yellow);
		Debug.DrawRay(transform.position +(next_position - m_rigidbody.transform.position), Vector3.down * 1000, Color.yellow);
		
		RaycastHit hit;
		if (Physics.Raycast(next_position, Vector3.down, out hit, Mathf.Infinity) && hit.collider.tag == "WorldLimit"){ return true;}
		return false;
	
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
		set_chasing_state(distance);
		
        Vector3 acceleration_force = get_acceleration_movement(distance);
        Vector3 pivot_force = apply_pivot(distance);

		//Debug.DrawRay(transform.position, acceleration_force, Color.red);
		//Debug.DrawRay(transform.position, pivot_force, Color.blue);
		//Debug.DrawRay(transform.position, acceleration_force + pivot_force, Color.green);
		
		
		if (obstacle_detected(acceleration_force))
		{
			acceleration_force *= -1;
		}

		m_rigidbody.AddForce(acceleration_force * Time.deltaTime);
		if (distance < distance_max_radius)
        {
            m_rigidbody.velocity = new Vector3(
                m_rigidbody.velocity.x * (1 - (braking / 1000.0f)),
                m_rigidbody.velocity.y,
                m_rigidbody.velocity.z * (1 - (braking / 1000.0f))
            );
        }
		
		m_rigidbody.AddForce(pivot_force * Time.deltaTime);

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
    public void set_chasing_state(float distance)
    {
        if (distance < distance_max_radius){ isChasing = true; }
        else { isChasing = false; }
    }
	
}
