using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int BulletDmg;
	public BulletManager BMan;
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
		r.constraints = freezeVal;
	}

	// Update is called once per frame

	void Update(){
		lifeTime -= Time.deltaTime;
		BulletMomentum ();

	}

	public void SetFreeze(bool b){
		isFrozen = b;

		if (isFrozen) {
			freezeVal = RigidbodyConstraints.FreezeAll;
		} else {
			freezeVal = RigidbodyConstraints.FreezePositionY;
		}
		r.constraints = freezeVal;
	}

	void OnCollisionEnter (Collision other)
	{
//		Debug.Log ("is hit");
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerHealth> ().takeDamage (BulletDmg);
			BMan.DestroyBullet (this);
		}
	}

	void BulletMomentum(){
		transform.Translate (Vector3.forward * Time.deltaTime * bulletSpeed);

		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, Time.deltaTime * bulletSpeed + .1f)) {
			Vector3 reflectDir = Vector3.Reflect (ray.direction, hit.normal);
			float rot = 90 - Mathf.Atan2 (reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3 (0, rot, 0);
		
		}
	
	}
}
