﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunBehavior : MonoBehaviour {

	GameObject prefab;
	void Start (){
		prefab = Resources.Load ("Bullet") as GameObject;
	}

	void Update (){
		if (Input.GetButtonDown ("Fire")) {
			Debug.Log ("Firing!");
			GameObject Bullet = Instantiate (prefab) as GameObject;
			Bullet.transform.position = transform.forward * 2;
			Rigidbody rb = Bullet.GetComponent<Rigidbody> ();
			rb.velocity = transform.forward * 50f;
		}
	
	}
}
