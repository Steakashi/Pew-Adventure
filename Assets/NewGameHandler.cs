using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameHandler : MonoBehaviour{
	
	private GameObject menu;
	
	void Start(){ menu = GameObject.FindGameObjectWithTag("Menu"); }
	
	public void LoadByIndex(int sceneIndex){
		SceneManager.LoadScene(sceneIndex);
		menu.SetActive(false);
	}
	
}
