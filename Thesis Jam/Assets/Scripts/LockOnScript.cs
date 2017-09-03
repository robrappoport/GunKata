using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnScript : MonoBehaviour {
	public GameObject newTarget;
	public GameObject currentTarget;
	Vector3 sphereLoc = new Vector3 (0, 0, 0);
	public float sphereRad = 10.0f;
	// Use this for initialization
	void Update () {
		
		transform.localPosition = sphereLoc;
		LockOnCheck ();
		lockOn ();
	}
	
	void LockOnCheck (){
		Collider[] checkColliders = Physics.OverlapSphere (sphereLoc, sphereRad);
		if (currentTarget == null && Input.GetButtonDown ("R3Down")) {
			float distance = Mathf.Infinity;
			foreach (Collider c in checkColliders) {
				if (c.gameObject.tag == "Enemy") {
					if (Vector3.Distance (c.gameObject.transform.position, transform.position) < distance)
						currentTarget = c.gameObject;
					distance = Vector3.Distance (c.gameObject.transform.position, transform.position);
					}
				}
			}
		if (newTarget != null && Input.GetButtonDown ("R3Down")) {
			currentTarget = null;
		}

		}

	void lockOn (){
		if (currentTarget != null) {
			Transform target = currentTarget.transform;
			transform.LookAt (target);

		}
	}

}


