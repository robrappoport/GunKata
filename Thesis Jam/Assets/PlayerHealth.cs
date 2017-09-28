using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	public Image HealthBar;
	public float MaxHealth;
	public float CurrentHealth;
	public GameObject playerCanvas;
	public TwoDCharacterController myCont;
	public TwoDGunBehavior myGun;
//	Renderer render;
//	public Material damagedColor;
//	public Material normalColor;

	// Use this for initialization
	void Start () {
		CurrentHealth = MaxHealth;
		SetHealth ();


		
	}
	
	// Update is called once per frame

	public void takeDamage (float amount)
	{
		if (myCont.isDashing == true && myGun.CurrentBullets <= myGun.MaxBullets) {
			dashAbsorb ();
		} else {
			CurrentHealth += amount;
//			render.material = damagedColor;
			SetHealth ();
			if (CurrentHealth <= 0f) {
				gameObject.SetActive (false);
				playerCanvas.gameObject.SetActive (false);
			}
		}
//		render.material = normalColor;
	}
		
	public void dashAbsorb ()
	{
		Debug.Log ("Is absorbing");
		myGun.CurrentBullets += 2;
	}

	public void SetHealth ()
	{
		HealthBar.fillAmount = CurrentHealth*.01f;

	}
		
}
