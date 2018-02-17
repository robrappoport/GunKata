using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {
	public float speed, lifetime = 10, age;
	public int damage;
	public int ownerNum;
	public Material player1BulletMaterial, player2BulletMaterial;
	public Material frozenBullet;
    public Turret myTurret;
    private float bulletHeight;
    private float initDistance;
	public bool player1AuraTriggered;
	public bool player2AuraTriggered;
	public bool prevPlayer1Triggered;
	public bool prevPlayer2Triggered;
	private float timeInAura;
	public float auraSpeedIncrease, slowSpeed, startSpeed, stopSpeed, projectionSpeed, deformingForceModifier;
    public GameObject impactPrefab;
    public float forceMultiplier;
	Rigidbody r;
	Renderer render;

	void Start(){
		age += Time.deltaTime;
//		myTurret = GetComponentInParent<Turret> ();
//		ownerNum = myTurret.ownerNum;
//		impactPrefab = myTurret.impactPrefabs [ownerNum];
		render = GetComponent<Renderer> ();
//		if (ownerNum != 2) {
//			render.material.color = myTurret.playerColors [ownerNum];
//		}

        bulletHeight = transform.position.y;

		r = GetComponent<Rigidbody> ();
	
		speed = startSpeed;
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = transform.forward * speed;

//		r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * speed;

	}
	void OnEnable(){
		age += Time.deltaTime;
		render = GetComponent<Renderer> ();
		bulletHeight = transform.position.y;
		r = GetComponent<Rigidbody> ();

		speed = startSpeed;
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = transform.forward * speed;


	}
	void Update(){
		if (age >= lifetime) {
			SelfDestruct ();
		} else {
			age += Time.deltaTime;
		}
	}

	void FixedUpdate(){
        if ((transform.position.y) > (bulletHeight))
        {
            transform.position = new Vector3 (transform.position.x, bulletHeight, transform.position.z);
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
	void OnTriggerEnter(Collider col){
		deformingForceModifier = deformingForceModifier * 5;
		Deform (col);
		deformingForceModifier = deformingForceModifier / 5;
		if (col.gameObject.GetComponent<auraPlayerHealth> ()) {
            if (col.gameObject.GetComponent<AuraCharacterController>().playerNum != ownerNum)
            {
                col.gameObject.GetComponent<auraPlayerHealth>().takeDamage(damage);
                SelfDestruct();

            }
            if (col.gameObject.tag == "Stage")
            {
                SelfDestruct();
            }
		}


		CancelInvoke ();
		if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "CannonBall" && col.gameObject.GetComponent<Turret>() != myTurret && !col.GetComponent<AuraGenerator>())
        {
            //Instantiate(impactPrefab, transform.position, Quaternion.identity);
            //myTurret.cannonBallList.Remove(this);
            SelfDestruct();
        }

        if (col.gameObject.tag == "PlayerAura")
        {
            initDistance = Vector3.Distance(transform.position, col.gameObject.transform.position);
        }
	}

	public void SelfDestruct(){
		age = 0;

        Instantiate(impactPrefab, transform.position, Quaternion.identity);
		gameObject.SetActive (false);
	}


	void OnTriggerStay (Collider other)
	{
		//		this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
		timeInAura += Time.deltaTime;
		//		Debug.Log ("enter time" + timeInAura);
		//		Debug.Log("enter bullet speed" + bulletSpeed);

        if (other.gameObject.tag == "PlayerAura")
        {

            //player1AuraTriggered = true;

            switch (other.gameObject.GetComponent<AuraGenerator>().auraType)
            {
                case AuraGenerator.AuraType.projection:
                    AuraProject(other.transform);
                    break;
                case AuraGenerator.AuraType.slowdown:
                    auraSlow();
                    break;
            }
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
		Deform (other);


	}

	void Deform(Collider col){
//		if (col.gameObject.GetComponent<MeshDeformer> ()) {
//			RaycastHit hit;
//			if (Physics.Raycast (transform.position, (col.transform.position - transform.position), out hit, GetComponent<SphereCollider> ().radius * 20, 1 << LayerMask.NameToLayer("Aura"), QueryTriggerInteraction.Collide)) {
//				col.gameObject.GetComponent<MeshDeformer> ().AddDeformingForce (hit.point, r.velocity.magnitude * deformingForceModifier);
//				//print (hit.collider.name);
//
//			}
//
//		}

	}

    void auraSlow ()
	{
		speed = slowSpeed;
		//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = r.velocity.normalized * speed;

		//Debug.Log ("bullet speed is" + ""+ r.velocity);
	}

    void AuraProject(Transform t1)
    {

        Transform auraCenter = t1;
        float distanceBtwn = Vector3.Distance(transform.position, auraCenter.position);
        float distancePercent = 1f - (distanceBtwn / initDistance);
        float projectForce = forceMultiplier * distancePercent;
        Vector3 auraVector = transform.position - auraCenter.position;
        r.AddForce(auraVector.normalized * projectForce, ForceMode.VelocityChange);
    }

	//void auraPlayerTwoSlow ()
	//{

	//	speed = slowSpeed;
	//	//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
	//	//Debug.Log ("bullet speed is" + ""+ r.velocity);
	//	r.velocity = r.velocity.normalized * speed;

	//}

	void auraStop ()
	{
		render.material = frozenBullet;
		speed = stopSpeed;
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * speed;
	}

    public void checkHit()
    {
		gameObject.SetActive (false);
    }

}
