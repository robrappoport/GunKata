using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int BulletDmg;
	public float bulletSpeed;
	public float lifeTime = 2.0f;
	Rigidbody r;
	Vector3 velocity;
	bool isFrozen;
	RigidbodyConstraints freezeVal;
	RigidbodyConstraints normalBehavior;


	// Use this for initialization
	void Start () {
		r = GetComponent<Rigidbody> ();

		freezeVal = RigidbodyConstraints.None;
		normalBehavior = RigidbodyConstraints.FreezePositionY;
		r.constraints = freezeVal;
	}

	// Update is called once per frame

	void Update(){
		lifeTime -= Time.deltaTime;
		r.velocity = transform.forward * bulletSpeed;

	}

	public void SetFreeze(bool b){
		isFrozen = b;

		if (isFrozen) {
			freezeVal = RigidbodyConstraints.FreezeAll;
		} else {
			freezeVal = RigidbodyConstraints.None;
		}
		r.constraints = freezeVal;
	}

	void OnCollisionEnter (Collision other)
	{
//		Debug.Log ("is hit");
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerHealth> ().takeDamage (BulletDmg);

			Destroy (this.gameObject);
		}
	}
}
