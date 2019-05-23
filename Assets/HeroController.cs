using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class HeroController : MonoBehaviour {

	private Vector3 fingerPositionStored;
    private Vector3 mouseInputStored;
    private float markerTime = 0;
    private float markerTimeForClick = 0;
    private float clickTime = 0.05f;
    private float markerAngle;
	private bool hasDragBegun = false;
    private bool dragParametersInitialized = false;
    private bool isMouseOnObject = false;
    private bool forceApplied = false;
    private bool letCircleGrowing = false;
    private bool cameraZoom = false;
    private int dragLimit = 115;
    private ForceCircle forceCircleScript;
    private ForceLevel forceLevelScript;
    private PostProcessingBehaviour postProcess_DragScript;
    private float zoomValue = 0;
    private Vector3 recordedPosition;
    private float maxSliderValue = 160f;
	private bool isEnabled = true;

    private float gr_TimeRatio = 0.1f;
    private float gr_ScaleLimit = 0.02f;
    private float gr_CurrentScale = 0;
    private float gr_Deceleration = 0.05f;
    private float gr_savedDecelerationValue = 1.1f;
    private float gr_CurrentDeceleration = 0;

    public GameObject bullet;
	public Transform spawn;
	public Rigidbody ship;
    public int speed;
    public Image forceCircle;
    public Image forceLevel;
    public Image dragTime;
    public Camera mainCamera;
    public float zoom = 10;
    public float timeToTurn;
    public float timeToDrag;

    private Ray ray;
    private RaycastHit hit;

    // Use this for initialization
    void Start ()
    {

        // Get the compponent needed
        forceCircleScript = forceCircle.GetComponent<ForceCircle>();
        forceLevelScript = forceLevel.GetComponent<ForceLevel>();
        postProcess_DragScript = Camera.main.GetComponent<PostProcessingBehaviour>();

        // Initialize needed value
        gr_CurrentDeceleration = gr_savedDecelerationValue;
        markerTime = Time.time;

    }
	
	void OnMouseDrag()
    {	
		if (!isEnabled){ return; }
		
        // If this is the first frame dragging, initialize a few values.
        if (hasDragBegun == false)
        {
            // Store finger position, which will be used to calculate the force applied to the hero.
            fingerPositionStored = Input.mousePosition;
            hasDragBegun = true;
        }

        // Only launch the following code if the player is really dragging the hero (and not just clicking)
        if (Time.time > markerTime && Time.time > markerTimeForClick && !dragParametersInitialized)
        {

            // Modify times values to create a slow-motion effect
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;

            // Set boolean to true. Allow specific function to works when drag has begun.
            postProcess_DragScript.enabled = true;
            cameraZoom = true;
            dragParametersInitialized = true;
            letCircleGrowing = true;

        }

        if (dragParametersInitialized)
        {
            // Set vector force, clamp it to a specific value and scale the level of force.
            Vector3 directionLooking = (Input.mousePosition - fingerPositionStored) / 5500;
            float value = Mathf.Clamp(directionLooking.magnitude, 0, 0.02f);
            forceLevel.transform.localScale = new Vector3(0.02f, value, 0.02f);
        }
            
        if(Input.mousePosition != fingerPositionStored)
        {
            // Set the rotation of the hero and the force vector, depending of the mouse's position.
            transform.eulerAngles = new Vector3(0, Mathf.Atan2((fingerPositionStored.x - Input.mousePosition.x), (fingerPositionStored.y - Input.mousePosition.y)) * Mathf.Rad2Deg, 0);
            forceLevel.transform.eulerAngles = new Vector3(90, Mathf.Atan2((fingerPositionStored.x - Input.mousePosition.x), (fingerPositionStored.y - Input.mousePosition.y)) * Mathf.Rad2Deg, 0);

        }
        Vector3 forceVector = new Vector3(fingerPositionStored.x - Input.mousePosition.x, 0, fingerPositionStored.y - Input.mousePosition.y);
        forceVector = Vector3.ClampMagnitude(forceVector, dragLimit) * speed;

    }


    void OnMouseUp()
    {
		if (!isEnabled){ return; }

        if (Time.time > markerTime && Time.time > markerTimeForClick)
        {

            // First, reset the velocity of the hero. It allows to apply a new force to it.
            ship.velocity = Vector3.zero;
			ship.angularVelocity = Vector3.zero;

            // Then create the force vector, clamp it et apply it to the hero.
            Vector3 forceVector = new Vector3(fingerPositionStored.x - Input.mousePosition.x, 0, fingerPositionStored.y - Input.mousePosition.y);
            forceVector = Vector3.ClampMagnitude(forceVector, dragLimit) * speed;
            ship.AddForce(forceVector);

            markerTime = Time.time + timeToDrag;
			
			//transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, Mathf.Atan2(ship.velocity.x, ship.velocity.z) * Mathf.Rad2Deg + 0, transform.rotation.eulerAngles.z);
			
        }

        // Reset time values used for slow-motion.
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;

        // Reset scale of forceCircle and forceLevel.
        forceCircle.transform.localScale = new Vector3(0, 0, 0);
        forceLevel.transform.localScale = new Vector3(0, 0, 0);

        // Store certain values, which will be used later for other functions.
        mouseInputStored = Input.mousePosition;

        // Reset values 
        postProcess_DragScript.enabled = false;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + zoomValue, Camera.main.transform.position.z);
        zoomValue = 0;
        hasDragBegun = false;
        dragParametersInitialized = false;
        forceApplied = true;
        letCircleGrowing = false;
        cameraZoom = false;
	

    }

    private void OnMouseDown()
    {
        markerTimeForClick = Time.time + clickTime;
    }

    void resetValues()
    {

    }

    /**
     * FUNCTION GROWCIRCLE
     * Expand the circle to a given size, depending of a few numbers of variables.
     * When the circle reach the given limit, it reset certains values and set the boolean variable (used as a condition) to false.
     **/
    void growCircle()
    {
        if (letCircleGrowing)
        {
            gr_CurrentDeceleration -= gr_Deceleration;
            gr_CurrentScale += (gr_TimeRatio * gr_ScaleLimit * gr_CurrentDeceleration);
            forceCircle.transform.localScale = new Vector3(gr_CurrentScale, gr_CurrentScale, gr_CurrentScale);

            // This part is launched when the scale limit is reached.
            if (gr_CurrentScale >= gr_ScaleLimit)
            {
                gr_CurrentScale = 0;
                gr_CurrentDeceleration = gr_savedDecelerationValue;
                letCircleGrowing = false;
            }
        }
    }

    /**
    * FUNCTION CAMERAZOOMEFFECT
    * Apply a small zoom effect on camera when called.
    * When the zoom limit is reached, it set the boolean variable (used as a condition) to false.
    **/
    void cameraZoomEffect()
    {
        if (cameraZoom)
        {

            if (zoomValue < zoom)
            {
                zoomValue += 0.1f;
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 0.1f, Camera.main.transform.position.z);

            }
            else { cameraZoom = false; }


        }
    }

    /**
    * FUNCTION DRAGSLIDERHANDLER
    * Called when the hero has been pushed away. Progressively fill the action bar.
    **/
    void dragSliderHandler()
    {
        if(Time.time < markerTime)
        {
            float value = (Time.time - (markerTime - timeToDrag)) / (markerTime - (markerTime - timeToDrag)) * maxSliderValue;
            dragTime.rectTransform.SetSizeWithCurrentAnchors(new RectTransform.Axis(), value);
        }
    }

	void Update()
    {

		Debug.Log(isEnabled);
	
        if(Time.time > markerTimeForClick)
        {
            growCircle();
            cameraZoomEffect();

        }

        dragSliderHandler();

    }

    void FixedUpdate()
    {
        /*if (ship.velocity.magnitude > speed)
        {
            ship.velocity = ship.velocity.normalized * speed;
        }*/
    }

    void LateUpdate()
    {

        

        // Check if the fire button has been pressed and if the hero isn't being dragged. 
        if (Input.GetButtonDown("Fire1") && dragParametersInitialized == false){

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if the raycast hit anything different to the player
            if (Physics.Raycast(ray, out hit, 100) && hit.collider.tag != "Player"){
                
                // Initialize the bullet and rotate it in direction of the point direction.
				GameObject bulletObject = Instantiate (bullet, spawn.position, spawn.rotation);
                bulletObject.transform.LookAt(hit.point);
                bulletObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, Mathf.Atan2((hit.point.x - transform.position.x),(hit.point.z - transform.position.z)) * Mathf.Rad2Deg - 0, 0);
                bulletObject.GetComponent<ClassicBullet>().setTarget("Enemy");

            }
            else if (Physics.Raycast(ray, out hit, 100))
            {   
                
                
            }

		}

        // If the hero isn't being dragged and if the hero has suffisant velocity, we constantly making him face the direction it goes.
        // TODO : make the player following the direction with a specific late.
		//&& ship.velocity.sqrMagnitude > 3
        if (!dragParametersInitialized && ship.velocity.sqrMagnitude > 3)
        {



            //Debug.Log(ship.velocity.magnitude);
            //transform.LookAt(transform.position + ship.velocity);
			
            //transform.eulerAngles = new Vector3(0, Mathf.Atan2(ship.velocity.x, ship.velocity.z) * Mathf.Rad2Deg + 0, 0);
            
			
			/*
			transform.eulerAngles = Vector3.Slerp(transform.rotation.eulerAngles, new Vector3(
				transform.rotation.eulerAngles.x, Mathf.Atan2(ship.velocity.x, ship.velocity.z) * Mathf.Rad2Deg, transform.rotation.eulerAngles.z
			), 100);
			/*
			transform.eulerAngles = new Vector3(
				transform.rotation.eulerAngles.x, Mathf.Atan2(ship.velocity.x, ship.velocity.z) * Mathf.Rad2Deg, transform.rotation.eulerAngles.z
			);*/
			
			//transform.eulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, Mathf.Atan2(ship.velocity.x, ship.velocity.z) * Mathf.Rad2Deg - 0, 0), 10);
            
			//transform.Rotate(90, 90, 90);
			
			
			
            //float beginningAngleValue = Mathf.Atan2((mouseInputStored.x - fingerPositionStored.x), (mouseInputStored.y - fingerPositionStored.y)) * Mathf.Rad2Deg;
            /*float beginningAngleValue = markerAngle;
            float finalAngleValue = Mathf.Atan2(ship.velocity.x, ship.velocity.z) * Mathf.Rad2Deg + 90;
                float angle = Mathf.LerpAngle(beginningAngleValue, finalAngleValue, Time.time - markerTime);
                transform.eulerAngles = new Vector3(90, angle, 90);*/

            

        }
            
            
                

	}

    private void OnCollisionEnter(Collision collision)
    {

    }

    public bool GetSlowMoState() { return dragParametersInitialized; }
	void OnDisable() { isEnabled = false; }
}
