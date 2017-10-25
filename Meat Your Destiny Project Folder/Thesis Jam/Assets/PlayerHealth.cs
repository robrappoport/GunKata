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
	public Renderer render;
	public float damageTime;
	public Material damagedColor;
	public Material normalColor;
	public Material playerColor;
	public int flashNum;

	// Use this for initialization
	void Start () {
		render = GetComponent<Renderer>();
		CurrentHealth = MaxHealth;
		SetHealth ();
		render.material = playerColor;
		playerColor = normalColor;
		takingDamage = false;


		
	}


	public void takeDamage (int amount)
	{
		if (myCont.isDashing == true && myGun.CurrentBullets <= myGun.MaxBullets) {
			dashAbsorb (amount);
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
		
	public void dashAbsorb (int amount)
	{
//		Debug.Log ("Is absorbing");
		CurrentHealth += amount;
		SetHealth ();
	}

	public void SetHealth ()
	{
		HealthBar.fillAmount = CurrentHealth*.01f;

	}

	private IEnumerator colorChange()
	{
		
			playerColor = damagedColor;

			yield return new WaitForSeconds (damageTime);

			playerColor = normalColor;

			yield return new WaitForSeconds (damageTime);


	}


		
}
