using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

    public BoxCollider gravityZone;
    public Rigidbody m_Rigidbody;

    private float distance;
    private float recordedPositionYAxis;
    private float gravityValue;
    private float gravityOffset = 1;
    private float recordedValue;
	private bool isTriggered = false;
	private Collider collidedObject = null;
	private Transform memorized_object_hit;
	private Quaternion heroRotation;
	private Vector3 stored_casted_vector;
	
	public float gravity_min_distance;

    // Use this for initialization
    void Start () {
        //m_Rigidbody = GetComponent<Rigidbody>();
        //recordedPositionYAxis = transform.position.y;
        gravityValue = 0;
    }

	
    void OnTriggerEnter(Collider collided_object)
    {
		if (collided_object.gameObject.tag == "World"){

			collidedObject = collided_object;
			//recordedPositionYAxis = transform.position.y;

		};
		
    }
	
	void setRotation()
	{
		if (collidedObject != null){
			Vector3 targetDir = collidedObject.gameObject.transform.position - transform.position;
			Vector3 forward = transform.up;
			float angle = Vector3.SignedAngle(targetDir, forward, Vector3.forward);
			//Debug.Log(transform.rotation.eulerAngles.x - 90);
			/*
			if (transform.rotation.eulerAngles.x < .1 && transform.rotation.eulerAngles.x > -.1 ){
			
				//transform.rotation = Quaternion.Euler(0, 0, 0);
			
			}
			else
			{
			
				
				targetDir.y = 0;
				var rotation = Quaternion.LookRotation(transform.forward);
				// FROM -110 TO -70
				// Objectif : 0
				float value_to_rotate = (transform.rotation.eulerAngles.x - 180);
				rotation *= Quaternion.Euler(value_to_rotate, 0, 0); // this adds a 90 degrees Y rotation
				//var adjustRotation = transform.rotation.y + rotationAdjust; //<- this is wrong!
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 0.5f);
				
			}*/
			
		};
	}
	
	public void reset_raycast(){
		stored_casted_vector = Vector3.zero;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, gravity_min_distance * 2, 1 << 8))
		{
			Debug.Log("Collision !");
			m_Rigidbody.angularVelocity = Vector3.zero;
			
			
			transform.eulerAngles = new Vector3(
				transform.rotation.eulerAngles.x, Mathf.Atan2(m_Rigidbody.velocity.x,m_Rigidbody.velocity.z) * Mathf.Rad2Deg, transform.rotation.eulerAngles.z
			);

			
			/*
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation,
				Quaternion.Euler(
					transform.rotation.eulerAngles.x,
					Mathf.Atan2(m_Rigidbody.velocity.x,m_Rigidbody.velocity.z) * Mathf.Rad2Deg,
					transform.rotation.eulerAngles.z
				), 
				Time.deltaTime * 200f
			);*/
		}
	}
	
	void addForceWithClamp(float distance_augmented){
		//m_Rigidbody.AddForce(m_Rigidbody.transform.up * distance_augmented * 10);
		m_Rigidbody.AddForce(Vector3.up * distance_augmented * 10);
		m_Rigidbody.velocity = new Vector3(
			m_Rigidbody.velocity.x,
			Mathf.Clamp(m_Rigidbody.velocity.y, -1000, distance_augmented),
			//m_Rigidbody.velocity.y + distance,
			m_Rigidbody.velocity.z
		);
		
	}
	
	void Update()
	{
		
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, gravity_min_distance, 1 << 8))
		{
			
			//Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            
			distance = gravity_min_distance - hit.distance;
			addForceWithClamp(distance * 10);
			
		}
		
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, gravity_min_distance * 2, 1 << 8))
		{
			
			// Get reflected vector
			Vector3 vector_ship_down = Vector3.down * hit.distance;
			Vector3 reflected = Vector3.Reflect(vector_ship_down, hit.normal);
			//Vector3 ship_direction = Quaternion.Euler(90, 0, 0) * reflected;
			
			Vector3 ship_direction = Vector3.Cross(transform.right, hit.normal);
			Debug.Log(ship_direction);
			/*if (ship_direction.y > .5){
				Debug.Log("ABANDON");
				return;
			}*/
			Debug.Log("GOOD");
			Quaternion heroRotation = Quaternion.LookRotation(ship_direction, Vector3.up);
			heroRotation = Quaternion.Euler(
				heroRotation.eulerAngles.x,
				transform.rotation.eulerAngles.y,
				heroRotation.eulerAngles.z
			);
			
			
			transform.rotation = Quaternion.RotateTowards(transform.rotation, heroRotation, Time.deltaTime * 100f);
			//transform.rotation = Quaternion.Slerp(transform.rotation, heroRotation, Time.deltaTime * 1f);
			/*Debug.DrawRay(transform.position, vector_ship_down, Color.blue);
			Debug.DrawRay(hit.point, reflected, Color.yellow);
			Debug.DrawRay(transform.position, ship_direction, Color.red);
			Debug.DrawRay(transform.position, -ship_direction, Color.cyan);
			
			Quaternion heroRotation = Quaternion.LookRotation(ship_direction, Vector3.up);
			
			stored_casted_vector = hit.normal;
			

			float y_rotation = 0;
			if (m_Rigidbody.velocity.sqrMagnitude > 10){ y_rotation = Mathf.Atan2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z) * Mathf.Rad2Deg; }
			else { y_rotation = transform.rotation.eulerAngles.y; }
			
			heroRotation = Quaternion.Euler(
				heroRotation.eulerAngles.x,
				transform.rotation.eulerAngles.y,
				heroRotation.eulerAngles.z
			);
			//transform.rotation = heroRotation;//Quaternion.Slerp(transform.rotation, heroRotation, Time.deltaTime * 5F);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, heroRotation, Time.deltaTime * 200f);
		*/
			// Draw rays and reflected ray
		
			// Check player orientation from interesected object
			//int invert = 0;
			//if (Vector3.Dot(transform.forward, Vector3.forward) > 0) { invert = -1; }
			//else { invert = 1; }


			
			//Vector3 final_vector = reflected + vector_ship_down;
			//float invert = Vector3.Dot(transform.forward, final_vector);
			//final_vector = final_vector * invert;
	


			// Update player rotation to face interested object's orientation
			//reflected = Quaternion.Euler(invert * 90, 0, 0) * reflected;
			/*
			if(final_vector == Vector3.zero){
				final_vector = Vector3.forward;
			}
			Debug.Log(final_vector);
			heroRotation.SetLookRotation(final_vector * -1, Vector3.up);
			*/
			
			//Vector3 final_vector = (reflected) + transform.position;
			//final_vector = reflected;

			//heroRotation.SetLookRotation(final_vector, Vector3.up);
			
			//float angle = Vector3.SignedAngle(final_vector, transform.forward, Vector3.up);
			
			
			// Create wanted rotation

			//transform.eulerAngles = new Vector3(0, Mathf.Atan2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z) * Mathf.Rad2Deg + 0, 0);
			/*
			heroRotation = Quaternion.Euler(
				heroRotation.eulerAngles.x,
				transform.rotation.eulerAngles.y,
				heroRotation.eulerAngles.z
			);*/
		
			
			// Apply rotation with slerp
			//transform.eulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, Mathf.Atan2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z) * Mathf.Rad2Deg - 0, 0), 10);


			/*transform.eulerAngles = Vector3.Lerp(
				transform.position,
				new Vector3(
					transform.rotation.eulerAngles.x,
					Mathf.Atan2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z) * Mathf.Rad2Deg,
					transform.rotation.eulerAngles.z
				),
				10
			);*/
				//transform.rotation = heroRotation;
				
				//transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, Mathf.Atan2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z) * Mathf.Rad2Deg - 0, transform.eulerAngles.z), 10);
				//transform.eulerAngles = new Vector3(0, Mathf.Atan2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z) * Mathf.Rad2Deg + 0, 0);
	
		}
		
	}
			
    void OnTriggerStay(Collider col)
    {
		collidedObject =  col;
        //distance = (recordedPositionYAxis - transform.position.y);
		//Debug.Log(collidedObject.gameObject.transform.position);
		distance = collidedObject.gameObject.transform.position.y - transform.position.y;
		//Debug.Log(distance);
		

		//if (coll.Raycast(ray, out hit, 100.0f))
		//{
		//	transform.position = ray.GetPoint(100.0f);
		//}
		
		if (distance > -0.1f)
		{

			//distance = Mathf.Clamp(gravityOffset / distance, -5f, .5f);
			//gravityValue = gravityOffset * distance;
			/*
			Vector3 gravityVector;
			gravityVector = new Vector3(0, gravityValue, 0);
			
			Debug.Log(gravityValue);
			
			m_Rigidbody.AddForce(Vector3.ClampMagnitude(gravityVector, 10));*/
			float distance_augmented = distance * 5;
			
			/*m_Rigidbody.velocity = new Vector3(
				m_Rigidbody.velocity.x,
				Mathf.Clamp(m_Rigidbody.velocity.y + distance, -1000, distance_augmented),
				//m_Rigidbody.velocity.y + distance,
				m_Rigidbody.velocity.z
			);*/
			//m_Rigidbody.AddForce(new Vector3(0, distance * 20, 0))
			//m_Rigidbody.AddForce(m_Rigidbody.transform.up * distance_augmented * 10);
			//setRotation();
			//addForceWithClamp(distance_augmented);
			/*
			m_Rigidbody.AddForce(new Vector3(0, distance * 20, 0));
			m_Rigidbody.velocity = new Vector3(
				m_Rigidbody.velocity.x,
				Mathf.Clamp(m_Rigidbody.velocity.y, -1000, distance),
				m_Rigidbody.velocity.z
			);*/
			
		}

        //recordedPositionYAxis = transform.position.y;

    }

    void OnTriggerExit()
    {

    }
}
