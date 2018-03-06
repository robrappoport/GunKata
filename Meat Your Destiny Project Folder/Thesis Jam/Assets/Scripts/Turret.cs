using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

	public Renderer topRenderer, middleRenderer, bottomRenderer;
	public Color p1Color, p2Color, neutralColor, currentColor, uncontestableColor, unownedColor;
	public GameObject CannonballPrefab;
	public int litSegments = 0, ownerNum = 2, timesOwned = 0, maxTimesCanBeOwned, chargeIncrementSign = 1;
	public float startTime, repeatTime, immuneTime, uncontestableTime, keyUncontestableTime, spinSpeed;
	public List<Renderer> SegmentsList;
	public EZObjectPools.EZObjectPool objectPool;
	public float charge, chargeSpeed = 1;
	public bool charging = false, withinTimerLimits = true, isSpinning, key, hasAddedToBall = false;

	public enum Owner { Player1, Player2, NONE };
	public Owner owner = Owner.NONE;

	GameObject Cannon;
	public bool completelyOwned = false, contestable = true;

	public List<Cannonball> cannonBallList = new List<Cannonball>();

	public GameObject[] Emitter;
	public Transform EmitterRotators;
	public GameObject[] turretTypes;
	public GameObject[] impactPrefabs;
	public Color[] playerColors;
	private Transform curTarget;
	private Vector3 targetDir;
	private Vector3 newDir;
	private Transform target;
	[Header("PARTICLE SYSTEM VARS")]
	ParticleSystem p;
	Collider auraCollider;

	//ownerNum will be received from the playerNum variable from AuraCharacterController script, where 2 acts as "none"
	//I know, I know, 0 makes you think "none" more than 2, but that's how the players are determined and I don't wanna fuck with that.
	void Awake(){	
		p = GetComponentInChildren<ParticleSystem>();
		p.transform.localPosition = new Vector3 (0, 1.5f, 0);
		p.gameObject.SetActive (false);
		//set emitters
		//Emitter[0] = GameObject.Find(name + "/N_Emitter");
		//Emitter[1] = GameObject.Find(name + "/E_Emitter"); 
		charge = 0;
		ownerNum = 2;
		//myShooter = GetComponentInChildren<UbhShotCtrl> ();
		//set up segments here
		topRenderer = transform.Find("Turret Top").GetComponent<Renderer>();
		middleRenderer = transform.Find("Turret Middle").GetComponent<Renderer>();
		bottomRenderer = transform.Find("Turret Bottom").GetComponent<Renderer>();
		SegmentsList.Add (bottomRenderer);
		SegmentsList.Add (middleRenderer);
		SegmentsList.Add (topRenderer);
		//
		Cannon = topRenderer.gameObject;

		p1Color = TwoDGameManager.thisInstance.playerHealth1.normalColor.color;
		p2Color = TwoDGameManager.thisInstance.playerHealth2.normalColor.color;
		currentColor = neutralColor;
		unownedColor = neutralColor;
		//InvokeRepeating("Fire", startTime, repeatTime);
		//amountOwnedIncrease = false;

	}

	void Start(){

		if(key){
			TwoDGameManager.thisInstance.keyTurrets.Add(this);
			uncontestableTime = keyUncontestableTime;

		}
		objectPool = GameObject.Find ("Cannonball pool").GetComponent<EZObjectPools.EZObjectPool>();
	}

	// Update is called once per frame
	void Update()
	{
		AimingTurret();

		if (charging && contestable) {
			charge = Mathf.Clamp (charge + chargeIncrementSign * Time.deltaTime * chargeSpeed, 0, 3);
		}
		if (isSpinning)
		{
			Vector3 curRotation = transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(curRotation.x, curRotation.y + spinSpeed, curRotation.z);
		}
		if(!withinTimerLimits)
		{
			contestable = false;
		}


		//        if (completelyOwned)
		//        {
		//            Debug.Log("checking");
		//            if (!amountOwnedIncrease && timesOwned < maxTimesCanBeOwned)
		//            {
		//                Debug.Log("checking2");
		//                amountOwnedIncrease = true;
		//                timesOwned++;
		//                CreateNewTurret();
		//				print ("creating new turret");
		//            }
		//        }

		litSegments = (int)charge;
		AdjustCannonColor ();
		AdjustOwnership (owner);

		DetermineDegreeOfOwnership();
		if (completelyOwned) {
			CancelInvoke ();
			InvokeRepeating ("Fire", repeatTime - (Time.timeSinceLevelLoad % repeatTime), repeatTime);
			//contestable = false;
			//myShooter.StartShotRoutine ();
			if (owner == Owner.Player1) {
				if (ownerNum != 0) {
					ownerNum = 0;
					neutralColor = p1Color;

					//increase the score

				}
			} else if (owner == Owner.Player2) {
				if (ownerNum != 1) {
					ownerNum = 1;
					neutralColor = p2Color;

				}
			}
			else
			{
				neutralColor = unownedColor;
			}
			litSegments = 0;
			charge = 0;

		}


		if (auraCollider && charging && MismatchedOwners()) {
			print ("sparking");
			var main = p.main;

			p.gameObject.SetActive (true);
			if (owner == Owner.Player1) {
				main.startColor = playerColors [0];
			} else {
				main.startColor= playerColors [1];

			}
		} else {
			p.gameObject.SetActive (false);
		}

		//	CleanCannonballList ();

	}


	void OnTriggerStay(Collider col){
		if (contestable) {


			if (col.gameObject.GetComponent<AuraGenerator>())
			{
				if (!col.gameObject.GetComponent<AuraGenerator>().isSuper)
				{
					//ownerNum = col.gameObject.GetComponentInChildren<AuraGenerator> ().auraPlayerNum;
					if (litSegments < 3)
					{
						charging = true;

					}


					if (col.gameObject.GetComponent<AuraGenerator>().auraPlayerNum == 0)
					{
						owner = Owner.Player1;
					}
					else
					{
						owner = Owner.Player2;
					}
					litSegments = (int)charge;

					//if the intruding player is hitting the turret , increment the number of lit segments up to a max of 3
					if (ownerNum != col.gameObject.GetComponent<AuraGenerator>().auraPlayerNum)
					{

						chargeIncrementSign = 1;
					}
					else
					{
						chargeIncrementSign = -1;
					}
				}
			}

		}
	}

	bool MismatchedOwners(){
		if (ownerNum == 0 && owner != Owner.Player1) {
			return true;
		} else if (ownerNum == 1 && owner != Owner.Player2) {
			return true;
		} else if (ownerNum == 2 && owner != Owner.NONE) {
			return true;
		}else{
			return false;
		}
	}
	void OnTriggerExit(Collider col){
		print("exiting");
		if (col.gameObject.GetComponent<AuraGenerator> ()) {
			charging = false;

			contestable = true;
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.GetComponent<AuraGenerator> ()) {
			auraCollider = col;

		}
		//    {
		//        if (col.gameObject.GetComponent<Bullet>())
		//        {
		//            if (contestable)
		//            {
		//                //resolve the ownerNum
		//                if (owner == Owner.NONE)
		//                {//set the owner to whoever hits the turret when the turret is unowned
		//                    ownerNum = col.gameObject.GetComponent<Bullet>().ownerNumber;
		//                    litSegments = 1;
		//
		//
		//                }
		//                else
		//                {
		//                    if (ownerNum == col.gameObject.GetComponent<Bullet>().ownerNumber)
		//                    {//if the owning player hits the turret, increment the number of lit segments up to a max of 3
		//                        litSegments = (int)(Mathf.Clamp(litSegments + 1, 0, 3));
		//                    }
		//                    else
		//                    {//return the tower to neutral if the last segment is depleted; otherwise decrease the number of lit segments by 1
		//                        if (litSegments == 1)
		//                        {
		//                            litSegments = 0;
		//                            ownerNum = 2;
		//                        }
		//                        else
		//                        {
		//                            litSegments = (int)(Mathf.Clamp(litSegments - 1, 0, 3));
		//                        }
		//                    }
		//                }
		//
		//                AdjustOwnership(ownerNum);
		//                AdjustCannonColor();
		//                DetermineDegreeOfOwnership();
		//
		//               
		//
		//
		//
		//            }
		//            col.gameObject.GetComponent<Bullet>().BMan.DestroyBullet(col.gameObject.GetComponent<Bullet>());
		//        }
	}

	void AdjustOwnership(Owner ownership)
	{
		//adjust ownership and color based on owner number;

		switch (ownership)
		{
		case Owner.Player1: //player 1
			currentColor = p1Color;
			break;
		case Owner.Player2: //player 2
			currentColor = p2Color;
			break;
		default: //neutral
			owner = Owner.NONE;
			currentColor = neutralColor;
			break;
		}
	}

	void AimingTurret()
	{
		// if the turret has an owner
		if (ownerNum != 2)
		{
			//check the owner number
			if (ownerNum == 0)
			{
				//get the other player
				curTarget = TwoDGameManager.thisInstance.players[1].transform;
				Debug.Log(curTarget);
			}

			if (ownerNum == 1)
			{
				//get the other player
				curTarget = TwoDGameManager.thisInstance.players[0].transform;
				Debug.Log(curTarget);
			}

			//face that player over a period of time
			target = curTarget;
			float rotSpeed = 2f;
			targetDir = new Vector3(target.position.x, EmitterRotators.transform.position.y, target.position.z) - EmitterRotators.transform.position;
			float step = rotSpeed * Time.deltaTime;
			newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
			Debug.DrawRay(transform.position, newDir, Color.red, 50f);
			transform.rotation = Quaternion.LookRotation(newDir);
		}


	}
	void AdjustCannonColor()
	{//adjusts turret based on the number of lit segments;
		if (litSegments > 0)
		{
			bottomRenderer.material.color = currentColor;
		}
		else
		{
			bottomRenderer.material.color = neutralColor;
		}

		if (litSegments > 1)
		{
			middleRenderer.material.color = currentColor;
		}
		else
		{
			middleRenderer.material.color = neutralColor;
		}

		if (litSegments > 2)
		{
			topRenderer.material.color = currentColor;
		}
		else
		{
			topRenderer.material.color = neutralColor;
		}

	}


	void CleanCannonballList(){
		//create new, clean list
		List<Cannonball> newCannonBallList = new List<Cannonball>();
		foreach (Cannonball c in cannonBallList)
		{
			if (c)
			{
				if (c.GetComponent<Cannonball> ()) {
					newCannonBallList.Add (c);
				}
			}
		}
		cannonBallList = newCannonBallList;
	}
	void Fire()
	{	 
		print ("firing");
		//CleanCannonballList ();

		foreach (GameObject Em in Emitter)
		{
			print ("firing");
			GameObject cannonBall;

			if (objectPool.TryGetNextObject (Em.transform.position, Em.transform.rotation, out cannonBall)) {
				cannonBallList.Add (cannonBall.GetComponent<Cannonball> ());
				cannonBall.GetComponent<Cannonball> ().myTurret = this;
				Cannonball newBall = cannonBall.GetComponent<Cannonball> ();
				newBall.impactPrefab = impactPrefabs [ownerNum];

				if (owner == Owner.Player1) {

					newBall.ownerNum = 0;

				} else if (owner == Owner.Player2) {
					newBall.ownerNum = 1;
				} else {
					newBall.ownerNum = 2;
				}
				//	Cannonball cball = cannonBall.GetComponent<Cannonball> ();
				//cannonBallList.Add(cball);
				//            if (completelyOwned)
				//            {

				if (ownerNum ==0) {
					cannonBall.GetComponent<Renderer> ().material = cannonBall.GetComponent<Cannonball> ().player1BulletMaterial;
					Physics.IgnoreCollision (TwoDGameManager.thisInstance.player1.GetComponentInChildren<Collider> (), cannonBall.GetComponent<Collider> ());
					cannonBall.layer = LayerMask.NameToLayer ("Player1OwnsTurret");



				} else if (ownerNum == 1) {
					cannonBall.GetComponent<Renderer> ().material = cannonBall.GetComponent<Cannonball> ().player2BulletMaterial;
					Physics.IgnoreCollision (TwoDGameManager.thisInstance.player2.GetComponentInChildren<Collider> (), cannonBall.GetComponent<Collider> ());
					cannonBall.layer = LayerMask.NameToLayer ("Player2OwnsTurret");

				} else {
					cannonBall.GetComponent<Renderer> ().material.color = neutralColor;

				}
			}
		}

	}

	public void Reset()
	{
		Debug.Log("RESET");
		CancelInvoke();
		ownerNum = 2;
		owner = Owner.NONE;
		neutralColor = unownedColor;
		litSegments = 0;
		completelyOwned = false;
		AdjustOwnership(owner);
		AdjustCannonColor();
		topRenderer.material.color = uncontestableColor;
		middleRenderer.material.color = uncontestableColor;
		bottomRenderer.material.color = uncontestableColor;
		contestable = false;
		bool turnOff = GetComponent<BallArrayScript>().on = false;
		hasAddedToBall = false;
		Invoke("Neutralize", uncontestableTime);

	}

	void Neutralize()
	{
		contestable = true;

	}

	public void Init (int ownerNum_, int timesOwned_, int litSegments_)
	{
		Debug.Log(timesOwned_);
		litSegments = litSegments_;
		ownerNum = ownerNum_;
		timesOwned = timesOwned_;
	}

	//    void CreateNewTurret ()
	//    {
	//        if (ownerNum == 0)
	//        {
	//            TwoDGameManager.player1ScoreNum++;
	//        }
	//        if (ownerNum == 1)
	//        {
	//            TwoDGameManager.player2ScoreNum++;
	//        }
	//        Turret newTurret = Instantiate(turretTypes[timesOwned-1], transform.position, Quaternion.identity).GetComponent<Turret>();
	//        newTurret.init(ownerNum, timesOwned+1, litSegments);
	//        newTurret.amountOwnedIncrease = true;
	//		newTurret.AdjustOwnership (newTurret.ownerNum);
	//		newTurret.AdjustCannonColor ();
	//		newTurret.DetermineDegreeOfOwnership ();
	//
	//        Destroy(gameObject);
	//    }

	public void DetermineDegreeOfOwnership ()
	{
		if (litSegments > 2)
		{

			if (key)
			{
				bool turnOn = GetComponent<BallArrayScript>().on = true;
				if (hasAddedToBall == false)
				{
					TwoDGameManager.thisInstance.ballTime += 2f;
					hasAddedToBall = true;
				}

			}
			if (!completelyOwned)
			{

				completelyOwned = true;
				//contestable = false;
				//Invoke("Reset", immuneTime);

			}
		}
		else
		{
			completelyOwned = false;
		}
	}
}
