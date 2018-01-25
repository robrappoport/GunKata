using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {
	public float speed, lifetime = 10;
	public int damage;
	public int ownerNum;
	public Material player1BulletMaterial, player2BulletMaterial;
	public Material frozenBullet;
    public Turret myTurret;


	public bool player1AuraTriggered;
	public bool player2AuraTriggered;
	public bool prevPlayer1Triggered;
	public bool prevPlayer2Triggered;
	private float timeInAura;
	public float auraSpeedIncrease, slowSpeed, startSpeed, stopSpeed;
    public GameObject impactPrefab;

	Rigidbody r;
	Renderer render;

	void Start(){
		
//		myTurret = GetComponentInParent<Turret> ();
//		ownerNum = myTurret.ownerNum;
//		impactPrefab = myTurret.impactPrefabs [ownerNum];
		render = GetComponent<Renderer> ();
//		if (ownerNum != 2) {
//			render.material.color = myTurret.playerColors [ownerNum];
//		}

        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
		Invoke ("SelfDestruct", lifetime);
		r = GetComponent<Rigidbody> ();
	
		speed = startSpeed;
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = transform.forward * speed;

//		r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * speed;

	}

	void FixedUpdate(){


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


	void OnTriggerEnter(Collider col){
		if (col.gameObject.GetComponent<auraPlayerHealth> ()) {
			if (col.gameObject.GetComponent<AuraCharacterController> ().playerNum != ownerNum) {
				col.gameObject.GetComponent<auraPlayerHealth> ().takeDamage (damage);
			}
		}
		CancelInvoke ();
		if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "CannonBall" && col.gameObject.GetComponent<Turret>() != myTurret && !col.GetComponent<AuraGenerator>())
        {
            Instantiate(impactPrefab, transform.position, Quaternion.identity);
            //myTurret.cannonBallList.Remove(this);
            SelfDestruct();
        }
	}

	void SelfDestruct(){
		Destroy (gameObject);
	}

	void OnTriggerStay (Collider other)
	{
		//		this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
		timeInAura += Time.deltaTime;
		//		Debug.Log ("enter time" + timeInAura);
		//		Debug.Log("enter bullet speed" + bulletSpeed);

		if (other.gameObject.tag == "player1Aura") 
		{	
			r = GetComponent<Rigidbody> ();

			player1AuraTriggered = true;
			auraPlayerOneSlow ();
		}

		if (other.gameObject.tag == "player2Aura") 
		{
			r = GetComponent<Rigidbody> ();

			//			this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
			player2AuraTriggered = true;
			auraPlayerTwoSlow ();
		}

	}

	void OnTriggerExit (Collider other)
	{
		print ("exiting aura");
			//		this.gameObject.GetComponent<Collider> ().material.bounciness = 1f;
			if (prevPlayer1Triggered) {
				player1AuraTriggered = false;
				prevPlayer1Triggered = false;
			}
			if (prevPlayer2Triggered) {
				player2AuraTriggered = false;
				prevPlayer2Triggered = false;
			}
			//tr.time += (time + timeInAura + auraSpeedIncrease);
			//		Debug.Log (tr.time);
			//		Debug.Log ("exit time" + timeInAura);
			speed = startSpeed * (timeInAura + auraSpeedIncrease);
			//		Debug.Log ("exit bullet speed" + bulletSpeed);
			//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = r.velocity.normalized * speed;


	}


	void auraPlayerOneSlow ()
	{
		speed = slowSpeed;
		//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = r.velocity.normalized * speed;

		//Debug.Log ("bullet speed is" + ""+ r.velocity);
	}

	void auraPlayerTwoSlow ()
	{

		speed = slowSpeed;
		//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		//Debug.Log ("bullet speed is" + ""+ r.velocity);
		r.velocity = r.velocity.normalized * speed;

	}

	void auraStop ()
	{
		render.material = frozenBullet;
		speed = stopSpeed;
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * speed;
	}

    public void checkHit()
    {
        Destroy(gameObject);
    }

}
