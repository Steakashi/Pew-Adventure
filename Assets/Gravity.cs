using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

    public Rigidbody m_rigidbody;
	public float gravity_min_distance;
	public float rotation_factor;
	public float force_factor;
	public float force_limit;

	private Quaternion hero_rotation;
	private RaycastHit hit;
	
	void OnCollisionEnter(Collision collision)
	{
	
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, gravity_min_distance * 2, 1 << 8))
		{
		
			m_rigidbody.angularVelocity = Vector3.zero;
			/*
			transform.eulerAngles = new Vector3(
				transform.rotation.eulerAngles.x, Mathf.Atan2(m_rigidbody.velocity.x,m_rigidbody.velocity.z) * Mathf.Rad2Deg, transform.rotation.eulerAngles.z
			);*/
			//Mathf.Atan2(m_rigidbody.velocity.x,m_rigidbody.velocity.z) * Mathf.Rad2Deg
		}
	}
	
	void addForceWithClamp(float force_value)
	{
		m_rigidbody.AddForce(Vector3.up * force_value * force_factor);
		m_rigidbody.velocity = new Vector3(
			m_rigidbody.velocity.x,
			Mathf.Clamp(m_rigidbody.velocity.y, -1000, force_value * force_limit),
			m_rigidbody.velocity.z
		);
	}
	
	void rotateShip()
	{
		Quaternion hero_rotation = Quaternion.LookRotation(
			Vector3.Cross(transform.right, hit.normal),
			Vector3.up
		);
		transform.rotation = Quaternion.RotateTowards(
			transform.rotation,
			Quaternion.Euler(
				hero_rotation.eulerAngles.x,
				transform.rotation.eulerAngles.y,
				hero_rotation.eulerAngles.z
			),
			Time.deltaTime * rotation_factor
		);
	}
	
	void Update()
	{
		
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, gravity_min_distance * 2, 1 << 8))
		{
			// Add Force if ship is lower to ground than gravity_min_distance
			if (hit.distance < gravity_min_distance){ addForceWithClamp((gravity_min_distance - hit.distance));}
			rotateShip();
		}
	}

}
