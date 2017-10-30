using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int BulletDmg;
	public BulletManager BMan;
	public float bulletSpeed;
	public float slowBulletSpeed;
	public float bulletStartSpeed;
	public float prevSpeed;
	public float fastBulletSpeed;
	public float stopBulletSpeed;
	public float inactiveTime = .2f;
	public float lifeTime = 2.0f;
	public float rayDist;
	Rigidbody r;
	Renderer render;
	Vector3 velocity;
	bool isFrozen;
	RigidbodyConstraints freezeVal;
	RigidbodyConstraints normalBehavior;

	public Material playerOneBullet;
	public Material playerTwoBullet;
	public Material normBullet;
	public Material frozenBullet;
	public bool isDestroyedOnHit = true;
//	Vector3 prevVel;
	public AudioSource bulletNoise;


	public bool player1AuraTriggered;
	public bool player2AuraTriggered;

	// Use this for initialization
	void Awake(){
		r = GetComponent<Rigidbody> ();
		render = GetComponent<Renderer> ();

	}
	void Start () {
		
		Physics.IgnoreCollision (this.gameObject.GetComponent<Collider> (), BMan.gameObject.GetComponent<Collider> (), true);

		bulletSpeed = bulletStartSpeed;
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;
		if (BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 0) {
			normBullet = playerOneBullet;
		}
		if (BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 1) {
			normBullet = playerTwoBullet;
		}

		render.material = normBullet;
		freezeVal = RigidbodyConstraints.FreezeRotation;
		normalBehavior = RigidbodyConstraints.FreezePositionY;
		r.constraints = normalBehavior | freezeVal;

	}

	// Update is called once per frame

	void Update(){
		lifeTime -= Time.deltaTime;
		inactiveTime -= Time.deltaTime;
			InactiveBullet ();

	}

	public void InactiveBullet ()
	{
		if (inactiveTime <= 0f) {

			Physics.IgnoreCollision (this.gameObject.GetComponent<Collider> (), BMan.gameObject.GetComponent<Collider> (), false);
		}
	}

	public void SetFreeze(bool b){
		isFrozen = b;

		if (isFrozen) {
//			Debug.Log (freezeVal);
//			freezeVal = RigidbodyConstraints.FreezePosition;
			render.material = frozenBullet;
//			prevVel = r.velocity;
//			prevSpeed = bulletSpeed;
			bulletSpeed = fastBulletSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;
//			Debug.Log (bulletSpeed);
//			GetComponent<Rigidbody> ().velocity = Vector3.zero;//isKinematic = true;

//			Debug.Log (freezeVal+"1");

		} else {
//			freezeVal = RigidbodyConstraints.FreezeRotation;
			render.material = normBullet;
//			r.velocity = prevVel;//.isKinematic = false;
			bulletSpeed = prevSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;
//			Debug.Log (bulletSpeed);
		}
		//r.constraints = freezeVal;
//		Debug.Log (freezeVal+"2");
	}


	void OnCollisionEnter (Collision other)
	{	
		if (isDestroyedOnHit) {
			if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<auraPlayerHealth> ().takeDamage (BulletDmg);
				BMan.DestroyBullet (this);
			}
		}
	}
		
	void OnTriggerStay (Collider other)
	{
		if (player1AuraTriggered && player2AuraTriggered) 
		{
			auraStop ();
			return;
		}
		if (other.gameObject.tag == "player1Aura") 
		{
			player1AuraTriggered = true;
			auraPlayerOneSlow ();
		}

		if (other.gameObject.tag == "player2Aura") 
		{
			player2AuraTriggered = true;
			auraPlayerTwoSlow ();
		}




	}

	void OnTriggerExit (Collider other)
	{
			bulletSpeed = bulletStartSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;

		if (player1AuraTriggered) {
			player1AuraTriggered = false;
		}
		if (player2AuraTriggered) {
			player2AuraTriggered = false;
		}
	}

	void auraPlayerOneSlow ()
	{
			bulletSpeed = slowBulletSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * bulletSpeed;
	}

	void auraPlayerTwoSlow ()
	{
			bulletSpeed = slowBulletSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * bulletSpeed;
	}

	void auraStop ()
	{
		render.material = frozenBullet;
			bulletSpeed = stopBulletSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * bulletSpeed;
	}
}
