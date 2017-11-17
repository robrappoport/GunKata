using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraColorManager : MonoBehaviour {

	Renderer r;
	auraGunBehavior player;
	public Color startColor, endColor;
	//to alter transparency, alter the alpha channel in each color


	// Use this for initialization
	void Start () {
		r = GetComponent<Renderer> ();
		player = GetComponentInParent<auraGunBehavior> ();
	}
	
	// Update is called once per frame
	void Update () {
		r.material.color = Color.Lerp (startColor, endColor, Mathf.Abs(1-player.curStamina / player.staminaTotal));
	}
}
