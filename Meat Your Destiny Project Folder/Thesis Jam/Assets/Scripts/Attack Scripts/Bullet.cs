using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int BulletDmg;
	public int ownerNumber = 0;
	public BulletManager BMan;
	public float bulletSpeed;
	public float slowBulletSpeed;
    public float projectionBulletSpeed;
	public float bulletStartSpeed;
    public float forceMultiplier, deformingForceModifier;
	public float prevSpeed;
	public float fastBulletSpeed;
	public float stopBulletSpeed;
	public float inactiveTime = .2f;
	public float lifeTime = 2.0f;
	public float rayDist;
    private float bulletHeight;
    private float initDistance;
    private float tempOwnerNum;
	Rigidbody r;
	Renderer render;
	Vector3 velocity;
	bool isFrozen;
	RigidbodyConstraints freezeVal;
	RigidbodyConstraints normalBehavior;

	public EZObjectPools.EZObjectPool objectPool;

	public Material playerOneBullet;
	public Material playerTwoBullet;
	public Material bulletTrailMaterial;
	public Material normBullet;
	public Material frozenBullet;
	public bool isDestroyedOnHit = true;
	public float time = 1.0f;
	private TrailRenderer tr;

    public List<Collider> colliders = new List<Collider>();
    public Collider currentCollider;
    public bool auraEntered = false;



	public bool player1AuraTriggered;
	public bool player2AuraTriggered;
	public bool prevPlayer1Triggered;
	public bool prevPlayer2Triggered;

	private float timeInAura;
	public float auraBulletSpeedIncrease;
    public GameObject impactPrefab;
	// Use this for initialization
	void Awake(){
		r = GetComponent<Rigidbody> ();
		tr = GetComponent<TrailRenderer> ();
		//render = GetComponent<Renderer> ();

	}
	void Start () {
		if (ownerNumber==0) {
			objectPool = GameObject.Find ("Impact pool p1").GetComponent<EZObjectPools.EZObjectPool>();
		} else {
			objectPool = GameObject.Find ("Impact pool p2").GetComponent<EZObjectPools.EZObjectPool>();
		}
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        bulletHeight = transform.position.y;
        
		bulletSpeed = bulletStartSpeed;
		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos(angle)) * bulletSpeed;
		if (BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 0) {
			normBullet = playerOneBullet;
		}
		if (BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 1) {
			normBullet = playerTwoBullet;
		}

		//render.material = normBullet;
		freezeVal = RigidbodyConstraints.FreezeRotation;
		normalBehavior = RigidbodyConstraints.FreezePositionY;
		r.constraints = normalBehavior | freezeVal;
		//render.enabled = false;
        
	}

	// Update is called once per frame

	void Update(){
        if ((transform.position.y) > (bulletHeight))
        {
            transform.position = new Vector3(transform.position.x, bulletHeight, transform.position.z);
        }
		tr.time = time;
		lifeTime -= Time.deltaTime;
		inactiveTime -= Time.deltaTime;
//		Debug.Log (inactiveTime);
		if (inactiveTime > 0f) {
            if (BMan != null)
            {
                Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), BMan.gameObject.GetComponent<Collider>(), true);
            }
			//render.enabled = false;
		} else {
            if (BMan != null)
            {
				if (GetComponent<Collider> () && BMan.GetComponent<Collider> ()) {
					Physics.IgnoreCollision (gameObject.GetComponent<Collider> (), BMan.gameObject.GetComponent<Collider> (), false);
				}
            }
			//render.enabled = true;
		}
	}
	void FixedUpdate(){
		//if ((!player1AuraTriggered && prevPlayer1Triggered) || (!player2AuraTriggered && prevPlayer2Triggered)) {
		//	//if (!(BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 0 && prevPlayer1Triggered) && !(BMan.gameObject.GetComponent<auraGunBehavior> ().playerNum == 1 && prevPlayer2Triggered)) {
		//	//	r.velocity *= -1f;
		//	//}
		//	r.velocity = r.velocity.normalized * fastBulletSpeed;
		//}
	

		//if (player1AuraTriggered && player2AuraTriggered) 
		//{
		//	//			this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
		//	auraStop ();
		//	player1AuraTriggered = false;
		//	player2AuraTriggered = false;
		//	return;
		//}
		//prevPlayer1Triggered = player1AuraTriggered;
		//prevPlayer2Triggered = player2AuraTriggered;
		//player1AuraTriggered = false;
		//player2AuraTriggered = false;

        AuraCheck();

	}

    void AuraCheck()
    {
        colliders = TwoDGameManager.CleanColliderList(colliders);
        currentCollider = AuraGenerator.GetCurrentAura(colliders, GetComponent<Collider>());
        if (currentCollider)
        {
           
            //Debug.Log(currentCollider.GetComponent<AuraGenerator>().isSuper + "supercheck");
            if (!currentCollider.GetComponent<AuraGenerator>().isSuper)
            {
                if (currentCollider.bounds.Contains(transform.position))
                {
                    timeInAura += Time.deltaTime;

                    switch (currentCollider.gameObject.GetComponent<AuraGenerator>().auraType)
                    {
                        case AuraGenerator.AuraType.projection:
                            AuraProject(currentCollider.transform);
                            break;
                        case AuraGenerator.AuraType.slowdown:
                            auraSlow();
                            break;
                    }
                }
                else
                {
                    currentCollider = null;
                }



            }
            else
            {
               
                if (currentCollider.GetComponent<AuraGenerator>().auraPlayerNum != ownerNumber)
                {
					//Debug.Log(currentCollider.GetComponent<AuraGenerator>().auraPlayerNum + "owner of aura    " + ownerNumber + "owner of bullet");
                    if (currentCollider.bounds.Contains(transform.position))
                    {
                        timeInAura += Time.deltaTime;

                        switch (currentCollider.gameObject.GetComponent<AuraGenerator>().auraType)
                        {
                            case AuraGenerator.AuraType.projection:
                                AuraProject(currentCollider.transform);
                                break;
                            case AuraGenerator.AuraType.slowdown:
                                auraSlow();
                                break;
                        }
                    }
                    else
                    {
                        currentCollider = null;
                    }


                }
                else
                {
                    currentCollider = null;
                }

            }

        }
        else
        {
           
            if (auraEntered)
            {
                    tr.time += (time + timeInAura + auraBulletSpeedIncrease);

                    bulletSpeed = bulletStartSpeed * (timeInAura + auraBulletSpeedIncrease);

                    r.velocity = r.velocity.normalized * bulletSpeed;

            }
        }
            

        //if (currentCollider)
        //{
        //    if (currentCollider.bounds.Contains(transform.position))
        //    {
        //        timeInAura += Time.deltaTime;

        //        switch (currentCollider.gameObject.GetComponent<AuraGenerator>().auraType)
        //        {
        //            case AuraGenerator.AuraType.projection:
        //                AuraProject(currentCollider.transform);
        //                break;
        //            case AuraGenerator.AuraType.slowdown:
        //                auraSlow();
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        currentCollider = null;
        //    }

        //}
        //else
        //{
        //    if (auraEntered)
        //    {
                
                //tr.time += (time + timeInAura + auraBulletSpeedIncrease);

                //bulletSpeed = bulletStartSpeed * (timeInAura + auraBulletSpeedIncrease);

                //r.velocity = r.velocity.normalized * bulletSpeed;

        //    }
        //}

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
    private void OnDestroy()
    {
		objectPool.TryGetNextObject(transform.position, Quaternion.identity);

    }

    void OnCollisionEnter (Collision other)
	{	
		if (isDestroyedOnHit) {
            if (other.gameObject.tag == "CannonBall")
            {
                if (other.gameObject.GetComponent<Cannonball>().ownerNum != this.ownerNumber)
                {
                    BMan.DestroyBullet(this);
                }
                else
                {
                    //ignore collision
                }
            }
			
            else
            {
                BMan.DestroyBullet(this);
            }
		}
	}

    private void OnTriggerEnter(Collider other)
    {
		switch (other.gameObject.tag)
		{
			case "Player":
				other.gameObject.GetComponent<auraPlayerHealth> ().takeDamage (BulletDmg);
                BMan.DestroyBullet (this);
			    break;
            
			case "PlayerAura":
				colliders.Add(other);
			    auraEntered = true;
			    break;
			default:
				BMan.DestroyBullet(this);
				break;
		}
        //if (other.gameObject.tag == "Player") {
        //        other.gameObject.GetComponent<auraPlayerHealth> ().takeDamage (BulletDmg);
        //        BMan.DestroyBullet (this);
        //    }

        //if (other.gameObject.tag == "PlayerAura")
        //{
        //    colliders.Add(other);
        //    auraEntered = true;

        //    initDistance = Vector3.Distance(transform.position, other.gameObject.transform.position);
        //}


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
                    AuraProject(other.gameObject.transform);
                    break; 
                case AuraGenerator.AuraType.slowdown: 
                    auraSlow(); 
                    break; 
            }
		}

//		if (other.gameObject.tag == "player2Aura") 
//		{
////			this.gameObject.GetComponent<Collider> ().material.bounciness = 0f;
		//	player2AuraTriggered = true;
		//	auraPlayerTwoSlow ();
		//}




	}

	void OnTriggerExit (Collider other)
	{
//		Debug.Log (tr.time);
//		Debug.Log ("exit time" + timeInAura);
//		Debug.Log ("exit bullet speed" + bulletSpeed);
			//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;


	}

    void auraSlow ()
	{
		bulletSpeed = slowBulletSpeed;
		//float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		r.velocity = r.velocity.normalized * bulletSpeed;
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

	void auraStop ()
	{
		//render.material = frozenBullet;
			bulletSpeed = stopBulletSpeed;
			float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			r.velocity = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * bulletSpeed;
	}
}
