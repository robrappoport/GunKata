using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int BulletDmg;
	public BulletManager BMan;
	public float bulletSpeed;
	public float lifeTime = 2.0f;
	Rigidbody r;
	Renderer render;
	Vector3 velocity;
	bool isFrozen;
	RigidbodyConstraints freezeVal;
	RigidbodyConstraints normalBehavior;

	public Material normBullet;
	public Material frozenBullet;


	// Use this for initialization
	void Start () {
		render = GetComponent<Renderer> ();
		r = GetComponent<Rigidbody> ();
		render.material = normBullet;
		freezeVal = RigidbodyConstraints.FreezeRotation;
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
//			Debug.Log (freezeVal);
			freezeVal = RigidbodyConstraints.FreezeAll;
//			Debug.Log (freezeVal+"1");
			render.material = frozenBullet;
		} else {
			freezeVal = RigidbodyConstraints.FreezeRotation;
			render.material = normBullet;
		}
		r.constraints = freezeVal;
//		Debug.Log (freezeVal+"2");
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
		

		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, Time.deltaTime * bulletSpeed + .3f)) {
			Vector3 reflectDir = Vector3.Reflect (ray.direction, hit.normal);
			float rot = 90 - Mathf.Atan2 (reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3 (0, rot, 0);
		
		}
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;
	
	}
}
