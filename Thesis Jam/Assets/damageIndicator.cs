using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageIndicator : MonoBehaviour {
	public PlayerHealth myHealth;
	public Material normalColor;
	public Material damagedColor;
	public Renderer render;
	public float damageTime;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (myHealth.takingDamage == true) {
			
			StartCoroutine (colorChange ());
		}
	}

	private IEnumerator colorChange()
	{
			myHealth.takingDamage = false;
			render.material = damagedColor;
			yield return new WaitForSeconds (damageTime);
			render.material = normalColor;
	}
}
