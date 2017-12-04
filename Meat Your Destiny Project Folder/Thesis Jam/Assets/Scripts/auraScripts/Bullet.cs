using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int BulletDmg;
	public int ownerNumber = 0;
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
	public float time = 1.0f;
	private TrailRenderer tr;



	public bool player1AuraTriggered;
	public bool player2AuraTriggered;
	public bool prevPlayer1Triggered;
	public bool prevPlayer2Triggered;

	private float timeInAura;
	public float auraBulletSpeedIncrease;
	// Use this for initialization
	void Awake(){
		r = GetComponent<Rigidbody> ();
		tr = GetComponent<TrailRenderer> ();
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
		render.enabled = false;

	}

	// Update is called once per frame

	void Update(){
		tr.time = time;
		lifeTime -= Time.deltaTime;
		inactiveTime -= Time.deltaTime;
//		Debug.Log (inactiveTime);
		if (inactiveTime > 0f) {
			Physics.IgnoreCollision (this.gameObject.GetComponent<Collider> (), BMan.gameObject.GetComponent<Collider> (), true);
			render.enabled = false;
		} else {
			Physics.IgnoreCollision (this.gameObject.GetComponent<Collider> (), BMan.gameObject.GetComponent<Collider> (), false);
			render.enabled = true;
		}
	}
	void FixedUpdate(){
		if ((!player1AuraTriggered && prevPlayer1Triggered) || (!player2AuraTriggered && prevPlayer2Triggered)) {
			if (!(BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 0 && prevPlayer1Triggered) && !(BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 1 && prevPlayer2Triggered)) {
				r.velocity *= -1f;
			}
			r.velocity = r.velocity.normalized * fastBulletSpeed;
		}
	

		if (player1AuraTriggered && player2AuraTriggered) 
		{
			//			this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
			auraStop ();
			player1AuraTriggered = false;
			player2AuraTriggered = false;
			return;
		}
		prevPlayer1Triggered = player1AuraTriggered;
		prevPlayer2Triggered = player2AuraTriggered;
		player1AuraTriggered = false;
		player2AuraTriggered = false;

	}

//	public void SetFreeze(bool b){
//		isFrozen = b;

//		if (isFrozen) {
////			Debug.Log (freezeVal);
////			freezeVal = RigidbodyConstraints.FreezePosition;
//			render.material = frozenBullet;
////			prevVel = r.velocity;
////			prevSpeed = bulletSpeed;
//			bulletSpeed = fastBulletSpeed;
//			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
//			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;
////			Debug.Log (bulletSpeed);
////			GetComponent<Rigidbody> ().velocity = Vector3.zero;//isKinematic = true;

////			Debug.Log (freezeVal+"1");

//		} else {
////			freezeVal = RigidbodyConstraints.FreezeRotation;
//			render.material = normBullet;
////			r.velocity = prevVel;//.isKinematic = false;
//			bulletSpeed = prevSpeed;
//			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
//			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;
////			Debug.Log (bulletSpeed);
//		}
//		//r.constraints = freezeVal;
////		Debug.Log (freezeVal+"2");
	//}
    

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
//		this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
		timeInAura += Time.deltaTime;
//		Debug.Log ("enter time" + timeInAura);
//		Debug.Log("enter bullet speed" + bulletSpeed);

		if (other.gameObject.tag == "player1Aura") 
		{
			
			player1AuraTriggered = true;
			auraPlayerOneSlow ();
		}

		if (other.gameObject.tag == "player2Aura") 
		{
//			this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
			player2AuraTriggered = true;
			auraPlayerTwoSlow ();
		}




	}

	void OnTriggerExit (Collider other)
	{
//		this.gameObject.GetComponent<Collider> ().material.bounciness = 1f;
		if (prevPlayer1Triggered) {
			player1AuraTriggered = false;
			prevPlayer1Triggered = false;
		}
		if (prevPlayer2Triggered) {
			player2AuraTriggered = false;
			prevPlayer2Triggered = false;
		}
        tr.time += (time + timeInAura + auraBulletSpeedIncrease);
//		Debug.Log (tr.time);
//		Debug.Log ("exit time" + timeInAura);
		bulletSpeed = bulletStartSpeed * (timeInAura + auraBulletSpeedIncrease);
//		Debug.Log ("exit bullet speed" + bulletSpeed);
			//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = r.velocity.normalized * bulletSpeed;


	}

	void auraPlayerOneSlow ()
	{
		bulletSpeed = slowBulletSpeed;
		//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = r.velocity.normalized * bulletSpeed;
		Debug.Log ("bullet speed is" + ""+ r.velocity);
	}

	void auraPlayerTwoSlow ()
	{
		bulletSpeed = slowBulletSpeed;
		//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = r.velocity.normalized * bulletSpeed;//new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * bulletSpeed;
		Debug.Log ("bullet speed is" + ""+ r.velocity);
	}

	void auraStop ()
	{
		render.material = frozenBullet;
			bulletSpeed = stopBulletSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * bulletSpeed;
	}
}
