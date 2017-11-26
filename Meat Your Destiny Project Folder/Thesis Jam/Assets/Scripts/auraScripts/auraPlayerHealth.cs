using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class auraPlayerHealth : MonoBehaviour {
	//public Image HealthBar;
	public float MaxHealth;
	public float CurrentHealth;
	public bool takingDamage;
	//public GameObject playerCanvas;
	public float damageTime;
	public Renderer render;
	public bool invincibilityFramesActive;
	public Material playerColor;
	public Material damagedColor;
	public Material normalColor;

	public int flashNum;

	// Use this for initialization
	void Start () {
		render.material = playerColor;
		//Debug.Log (render.material);
		playerColor = normalColor;
		CurrentHealth = MaxHealth;
		//SetHealth ();
		takingDamage = false;



	}

	void Update ()
	{
		render.material = playerColor;
	}


	public void takeDamage (int amount)
	{
		if (!invincibilityFramesActive) {
			takingDamage = true;
			CurrentHealth += amount;
			//SetHealth ();
			invincibilityFramesActive = true;
			StartCoroutine (colorChange ());


			if (CurrentHealth <= 0f) {
				gameObject.SetActive (false);
				//playerCanvas.gameObject.SetActive (false);
			}
		}
	}

	//public void SetHealth ()
	//{
	//	HealthBar.fillAmount = CurrentHealth*.01f;

	//}


	private IEnumerator colorChange()
	{
		for (int i = 0; i < flashNum; i++) {


			playerColor = damagedColor;

			yield return new WaitForSeconds (damageTime);



			playerColor = normalColor;

			yield return new WaitForSeconds (damageTime);

		}
		invincibilityFramesActive = false;
	}



}
