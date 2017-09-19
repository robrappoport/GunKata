using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	public Image HealthBar;
	public float MaxHealth;
	public float CurrentHealth;
	public GameObject playerCanvas;

	// Use this for initialization
	void Start () {

		CurrentHealth = MaxHealth;
		SetHealth ();
		
	}
	
	// Update is called once per frame

	public void takeDamage (float amount)
	{
		CurrentHealth += amount;
		SetHealth ();
		if (CurrentHealth <= 0f) {
			gameObject.SetActive(false);
			playerCanvas.gameObject.SetActive (false);
		}
	}

	public void SetHealth ()
	{
		HealthBar.fillAmount = CurrentHealth*.01f;

	}
		
}
