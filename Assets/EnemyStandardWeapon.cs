using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStandardWeapon : MonoBehaviour {

    private EnemyStandardMovements movementsScript;
	private Transform hero;
	private RaycastHit hit;
	private bool allowFire = true;
    private float timeToWait = 0;
    private int numberOfBullets = 1;

	public GameObject bullet;
	public float loadingTime;
	public float shootRate;
	public bool randomFire;
    public float maxTimeToWait;
    public float maxNumberOfBullets;

    public float precision;


	// Use this for initialization
	void Start () {

		hero = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        movementsScript = GetComponent<EnemyStandardMovements>();

	}
	
	// Update is called once per frame
	void Update () {


		if (Physics.Linecast(transform.position, hero.position, out hit) && hit.collider.tag == "Player" & allowFire & movementsScript.GetChasingState())
		{
            if (randomFire)
            {
                timeToWait = Random.Range(shootRate, maxTimeToWait);
                numberOfBullets = (int)Random.Range(1, (maxNumberOfBullets + 1));
            }
			StartCoroutine(Fire());
		}
			
	}

	IEnumerator Fire(){

		allowFire = false;
        for(int i=0; i<numberOfBullets; i++)
        {
          
            GameObject bulletObject = Instantiate(bullet, transform.position, transform.rotation);
            bulletObject.transform.Rotate(Vector3.up * Random.Range(-precision, precision));
            bulletObject.GetComponent<ClassicBullet>().setTarget("Player");
            yield return new WaitForSeconds(shootRate);
            i++;

        }
        yield return new WaitForSeconds(timeToWait);
        allowFire = true;

	}
}
