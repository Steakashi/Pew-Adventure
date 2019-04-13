using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHandler : MonoBehaviour {

    private float currentLifePoints;
    private float healthBarMaxWidth = 160;
    private float invincibilityTimeMarker = 0;
    private float explosionFXTime;

    public Image healthBar;
    public float maxLifePoints;
    public float invincibilityTime;
    public List<GameObject> thingsToExplode;
    public GameObject explosionFX;

	// Use this for initialization
	void Start () {

        currentLifePoints = maxLifePoints;
        explosionFXTime = explosionFX.GetComponent<Animation>().clip.length;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy" && Time.time > invincibilityTimeMarker)
        {


        }
    }

    public void ReceiveDamages(float damages)
    {

        if (Time.time > invincibilityTimeMarker)
        {
            currentLifePoints -= damages;

            float value = (currentLifePoints / maxLifePoints) * healthBarMaxWidth;
            healthBar.rectTransform.SetSizeWithCurrentAnchors(new RectTransform.Axis(), value - 1);

            if(currentLifePoints <= 0) { StartCoroutine("DestroyObject"); }

            invincibilityTimeMarker = Time.time + invincibilityTime;
        }

    }

    IEnumerator DestroyObject()
    {
        Debug.Log("EXPLOSION");
        explosionFX.transform.position = transform.position;
        explosionFX.SetActive(true);
        foreach (GameObject th in thingsToExplode)
        {
            Destroy(th);
        }

        yield return new WaitForSeconds(explosionFXTime);

        //Destroy(transform.parent.gameObject);
    }
}
