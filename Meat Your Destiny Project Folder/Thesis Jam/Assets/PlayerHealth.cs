using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	public Image HealthBar;
	public float MaxHealth;
	public float CurrentHealth;
	public bool takingDamage;
	public GameObject playerCanvas;
	public TwoDCharacterController myCont;
	public TwoDGunBehaviorBigClip myGun;
	public Renderer[] renderers;
	public float damageTime;
	public Material damagedColor;
	public Material normalColor;
	public int flashNum;

	// Use this for initialization
	void Start () {
		CurrentHealth = MaxHealth;
		SetHealth ();
		renderers = GetComponentsInChildren <Renderer> (); 
		takingDamage = false;


		
	}


	public void takeDamage (float amount)
	{
		if (myCont.isDashing == true && myGun.CurrentBullets <= myGun.MaxBullets) {
			dashAbsorb ();
		} else {
			
			takingDamage = true;
			CurrentHealth += amount;
			SetHealth ();

			StartCoroutine (colorChange ());


			if (CurrentHealth <= 0f) {
				gameObject.SetActive (false);
				playerCanvas.gameObject.SetActive (false);
			}
		}
	}
		
	public void dashAbsorb ()
	{
//		Debug.Log ("Is absorbing");
		myGun.CurrentBullets += 2;
	}

	public void SetHealth ()
	{
		HealthBar.fillAmount = CurrentHealth*.01f;

	}

	private IEnumerator colorChange()
	{
		for (int i = 0; i < flashNum; i++) {
			
			foreach (Renderer render in renderers) {
				render.material = damagedColor;
			}
			yield return new WaitForSeconds (damageTime);

			foreach (Renderer render in renderers) {

				render.material = normalColor;
			}
			yield return new WaitForSeconds (damageTime);

		}

	}


		
}
