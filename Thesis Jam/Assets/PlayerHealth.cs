using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	public Image HealthBar;
	public int MaxHealth;
	public int CurrentHealth;

	// Use this for initialization
	void Start () {

		CurrentHealth = MaxHealth;
		SetHealth ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void takeDamage (int amount)
	{
		CurrentHealth += amount;
		SetHealth ();
	}

	public void SetHealth ()
	{
		int my_health = CurrentHealth / MaxHealth;
		HealthBar.fillAmount = CurrentHealth;

	}
		
}
