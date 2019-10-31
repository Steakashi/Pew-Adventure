using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
     
    public static float Map (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
       
}


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
		float hero_angle = Vector3.Angle(Vector3.up, transform.TransformDirection(Vector3.up)).Map(
			0.0f, 90f, 1f, 0.01f
		);
		
		m_rigidbody.AddForce(Vector3.up * force_value * force_factor * hero_angle);
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
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.green);
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, gravity_min_distance * 2, 1 << 8))
		{
			// Add Force if ship is lower to ground than gravity_min_distance
			if (hit.distance < gravity_min_distance){ addForceWithClamp((gravity_min_distance - hit.distance));}
			rotateShip();
		}
	}

}
