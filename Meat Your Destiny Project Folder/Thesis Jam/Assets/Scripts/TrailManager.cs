using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour {
	public Color startColorLeft, startColorRight, endColorLeft, endColorRight;//defines the left and right sides of the gradient when the bullet starts and the bullet disappears, respectively
	TrailRenderer trailRenderer;
	Gradient gradient;
	Color currentColorLeft, currentColorRight;
	float alpha = 1.0f, age = 0;

	// Use this for initialization
	void Start () {
		trailRenderer = GetComponent <TrailRenderer> ();
		gradient = new Gradient ();
		AdjustColors ();
 

	}
		
	// Update is called once per frame
	void Update () {
		age += Time.deltaTime;
		AdjustColors ();	
	}
	void AdjustColors(){
		currentColorLeft = Color.Lerp (startColorLeft, endColorLeft, age/GetComponent<Bullet> ().lifeTime);
		currentColorRight = Color.Lerp (endColorRight, endColorRight, age/GetComponent<Bullet> ().lifeTime);
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(currentColorLeft, 0.0f), new GradientColorKey(currentColorRight, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
		);
		trailRenderer.colorGradient = gradient;
	}

}
