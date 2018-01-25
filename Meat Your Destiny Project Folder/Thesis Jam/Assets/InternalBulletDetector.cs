using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalBulletDetector : MonoBehaviour {

	Turret parentTurret;
	// Use this for initialization
	void Awake () {
		parentTurret = GetComponentInParent<Turret> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerExit(Collider col){
		print (col.gameObject.name);
		col.gameObject.transform.parent.gameObject.AddComponent<Cannonball> ();
		col.gameObject.GetComponentInParent<Cannonball> ().ownerNum = parentTurret.ownerNum;
		parentTurret.cannonBallList.Add (col.gameObject.GetComponentInParent<Cannonball> ());

	}
}
