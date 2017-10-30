using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auraCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerStay (Collider other)
	{
		if (other.gameObject.tag == "Bullet") {
			if (this.gameObject.tag == "player1Aura") {
				other.gameObject.GetComponent<Bullet>().player1AuraTriggered = true;
			}
			if (this.gameObject.tag == "player2Aura") {
				other.gameObject.GetComponent<Bullet>().player2AuraTriggered = true;
			}
		}

	}
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Bullet") {

			if (this.gameObject.tag == "player1Aura") {
				other.gameObject.GetComponent<Bullet>().player1AuraTriggered = false;
			}
			if (this.gameObject.tag == "player2Aura") {
				other.gameObject.GetComponent<Bullet>().player2AuraTriggered = false;
			}
		}
	}
}
