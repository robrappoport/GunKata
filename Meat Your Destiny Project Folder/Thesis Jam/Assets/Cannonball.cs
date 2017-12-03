using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {
	public float speed, lifetime;
	public int damage;
	public int ownerNum;
	public Renderer player1BulletMaterial, player2BulletMaterial;

	void Start(){
		Invoke ("SelfDestruct", lifetime);
	}


	void OnCollisionEnter(Collision col){
		if (col.gameObject.GetComponent<auraPlayerHealth> ()) {
			if (col.gameObject.GetComponent<AuraCharacterController> ().playerNum != ownerNum) {
				col.gameObject.GetComponent<auraPlayerHealth> ().takeDamage (damage);
			}
		}
		CancelInvoke ();
		SelfDestruct();
	}

	void SelfDestruct(){
		Destroy (gameObject);
	}
}
