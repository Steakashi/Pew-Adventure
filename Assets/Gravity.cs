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
	
	public float gravity_min_distance;

    // Use this for initialization
    void Start () {
        //m_Rigidbody = GetComponent<Rigidbody>();
        //recordedPositionYAxis = transform.position.y;
        gravityValue = 0;
    }

	
    void OnTriggerEnter(Collider collided_object)
    {
		Debug.Log(collided_object);

		if (collided_object.gameObject.tag == "World"){
			
			Debug.Log(collided_object);
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
			Debug.Log(angle);
			//Debug.Log(transform.rotation.eulerAngles.x - 90);
			Debug.Log("--");
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
			
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            
			distance = gravity_min_distance - hit.distance;
			addForceWithClamp(distance * 5);
			
		}
		
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, gravity_min_distance * 2, 1 << 8))
		{
			
			// Get reflected vector
			Vector3 reflected = Vector3.Reflect(Vector3.down, hit.normal);
			
			// Draw rays and reflected ray
			Debug.DrawRay(hit.point, reflected * hit.distance, Color.yellow);
			Debug.DrawRay(reflected * hit.distance, reflected * distance, Color.yellow);
			
			// Check player orientation from interesected object
			int invert = 0;
			if (Vector3.Dot(transform.forward, reflected) > 0) { invert = -1; }
			else { invert = 1; }
			invert = 1;
			// Update player rotation to face interested object's orientation
			reflected = Quaternion.Euler(invert * 90, 0, 0) * reflected;
			heroRotation.SetLookRotation(reflected, Vector3.up);
	
			// Create wanted rotation
			heroRotation = Quaternion.Euler(
				heroRotation.eulerAngles.x,
				transform.rotation.eulerAngles.y,
				heroRotation.eulerAngles.z
			);

			// Apply rotation with slerp
			//transform.rotation = Quaternion.Slerp(transform.rotation, heroRotation, Time.deltaTime * 5f);
			transform.rotation = heroRotation;
		
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
