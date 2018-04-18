using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateThis : MonoBehaviour{

	public bool x, y, z;
	public float speed = 20;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (x) {
			transform.Rotate (Vector3.right, Time.deltaTime * speed);
		}
		if (y) {
			transform.Rotate (Vector3.up, Time.deltaTime * speed);
		}
		if (z) {
			transform.Rotate (Vector3.forward, Time.deltaTime * speed);
		}

	}
}
