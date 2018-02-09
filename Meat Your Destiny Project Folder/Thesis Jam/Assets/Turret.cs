using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    public Renderer topRenderer, middleRenderer, bottomRenderer;
    public Color p1Color, p2Color, neutralColor, currentColor, uncontestableColor;
    public GameObject CannonballPrefab;
	public int litSegments = 0, ownerNum = 2, timesOwned = 0, maxTimesCanBeOwned, chargeIncrementSign = 1;
    public float startTime, repeatTime, immuneTime, uncontestableTime, spinSpeed;
    public bool amountOwnedIncrease;
    public bool scoreIncrease;
    public GameObject textPrefab;
	public List<Renderer> SegmentsList;

	public float charge, chargeSpeed = 1;
	public bool charging = false, withinTimerLimits = true;

    //public List<Cannonball> cannonBallList = new List<Cannonball>();
	//UbhShotCtrl myShooter;
    TwoDGameManager gm;
    public enum Owner { Player1, Player2, NONE };
    public Owner owner = Owner.NONE;

    GameObject Cannon;
    public bool completelyOwned = false, contestable = true;

    public List<Cannonball> cannonBallList = new List<Cannonball>();

    public GameObject[] Emitter;
    public GameObject[] turretTypes;
    public GameObject[] impactPrefabs;
    public Color[] playerColors;


    //ownerNum will be received from the playerNum variable from AuraCharacterController script, where 2 acts as "none"
    //I know, I know, 0 makes you think "none" more than 2, but that's how the players are determined and I don't wanna fuck with that.
    void Awake(){	


		//set emitters
		Emitter[0] = GameObject.Find(name + "/N_Emitter");
		Emitter[1] = GameObject.Find(name + "/E_Emitter"); 
		charge = 0;
		ownerNum = 2;
        gm = FindObjectOfType<TwoDGameManager>();
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

        p1Color = gm.playerHealth1.normalColor.color;
        p2Color = gm.playerHealth2.normalColor.color;
        currentColor = neutralColor;
        //InvokeRepeating("Fire", startTime, repeatTime);
        //amountOwnedIncrease = false;
    }

    // Update is called once per frame
    void Update()
    {
		if (charging && contestable) {
			charge = Mathf.Clamp (charge + chargeIncrementSign * Time.deltaTime * chargeSpeed, 0, 3);
		}
//        Vector3 curRotation = transform.localRotation.eulerAngles;
//        transform.localRotation = Quaternion.Euler(curRotation.x, curRotation.y + spinSpeed, curRotation.z);
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
			contestable = false;
			//myShooter.StartShotRoutine ();
			if (owner == Owner.Player1) {
				if (ownerNum != 0) {
					ownerNum = 0;
					neutralColor = p1Color;

					//increase the score
			
					TwoDGameManager.player1ScoreNum++;
					TextManager txt = ((GameObject)Instantiate (textPrefab, transform.position + (Vector3.up * 2f), Quaternion.identity)).GetComponent<TextManager> ();
					txt.color = playerColors [ownerNum];
					txt.pointString = "50";
					scoreIncrease = true;
					print("player1 score increase");
				}
			} else if (owner == Owner.Player2) {
				if (ownerNum != 1) {
					ownerNum = 1;
					neutralColor = p2Color;

					TextManager txt = ((GameObject)Instantiate (textPrefab, transform.position + (Vector3.up * 2f), Quaternion.identity)).GetComponent<TextManager> ();
					txt.color = playerColors [ownerNum];
					txt.pointString = "50";
					TwoDGameManager.player2ScoreNum++;
					scoreIncrease = true;
					print ("player2 score increase");
				}
			}
			litSegments = 0;
			charge = 0;

		}


		CleanCannonballList ();
			
    }


	void OnTriggerStay(Collider col){
		if (withinTimerLimits) {
			if (contestable) {
				if (completelyOwned) {
					contestable = false;
				}

				if (col.gameObject.GetComponent<AuraGenerator> ()) {
					//ownerNum = col.gameObject.GetComponentInChildren<AuraGenerator> ().auraPlayerNum;
					if (litSegments < 3) {
						charging = true;

					}

			
					if (col.gameObject.GetComponent<AuraGenerator> ().auraPlayerNum == 0) {
						owner = Owner.Player1;
					} else {
						owner = Owner.Player2;
					}
					litSegments = (int)charge;

					//if the intruding player is hitting the turret , increment the number of lit segments up to a max of 3
					if (ownerNum != col.gameObject.GetComponent<AuraGenerator> ().auraPlayerNum) {

						chargeIncrementSign = 1;
					} else {
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
		} else {
			return false;
		}
	}
	void OnTriggerExit(Collider col){

		if (col.gameObject.GetComponent<AuraGenerator> ()) {
			charging = false;
			contestable = true;
		}
	}

//    void OnTriggerEnter(Collider col)
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
//    }

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
		CleanCannonballList ();
			
        foreach (GameObject Em in Emitter)
        {
			print ("firing");
            GameObject cannonBall = Instantiate(CannonballPrefab, Em.transform.position, Em.transform.rotation, null) as GameObject;
            cannonBallList.Add(cannonBall.GetComponent<Cannonball>());
            Cannonball newBall = cannonBall.GetComponent<Cannonball>();
			print (impactPrefabs.Length);
            newBall.impactPrefab = impactPrefabs[ownerNum];

            if (completelyOwned)
            {
                
                newBall.ownerNum = ownerNum;

            }
            else
            {
                newBall.ownerNum = 2;
            }
            Cannonball cball = cannonBall.GetComponent<Cannonball>();
            //cannonBallList.Add(cball);
//            if (completelyOwned)
//            {

                if (ownerNum == 0)
                {
                    cannonBall.GetComponent<Renderer>().material = cannonBall.GetComponent<Cannonball>().player1BulletMaterial;
                    Physics.IgnoreCollision(gm.player1.GetComponentInChildren<Collider>(), cannonBall.GetComponent<Collider>());
                    cannonBall.layer = LayerMask.NameToLayer("Player1OwnsTurret");
                    if (!scoreIncrease)
                    {
                        TwoDGameManager.player1ScoreNum+=2;
                        TextManager txt = ((GameObject)Instantiate(textPrefab, transform.position + (Vector3.up * 2f), Quaternion.identity)).GetComponent<TextManager>();
                        txt.color = playerColors[ownerNum];
                        txt.pointString = "50";
                        scoreIncrease = true;
                    }

                   

                }
                else if (ownerNum == 1)
                {
                    cannonBall.GetComponent<Renderer>().material = cannonBall.GetComponent<Cannonball>().player2BulletMaterial;
                    Physics.IgnoreCollision(gm.player2.GetComponentInChildren<Collider>(), cannonBall.GetComponent<Collider>());
                    cannonBall.layer = LayerMask.NameToLayer("Player2OwnsTurret");
                    if (!scoreIncrease)
                    {
                        TextManager txt = ((GameObject)Instantiate(textPrefab, transform.position + (Vector3.up * 2f), Quaternion.identity)).GetComponent<TextManager>();
                        txt.color = playerColors[ownerNum];
                        txt.pointString = "50";
                        TwoDGameManager.player2ScoreNum+=2;
                        scoreIncrease = true;
                    }
                }
                else
                {
                    cannonBall.GetComponent<Renderer>().material.color = neutralColor;

                }
           // }
        }

    }

    void Reset()
    {

        ownerNum = 2;
        litSegments = 0;
        completelyOwned = false;
		AdjustOwnership(owner);
        AdjustCannonColor();
        topRenderer.material.color = uncontestableColor;
        middleRenderer.material.color = uncontestableColor;
        bottomRenderer.material.color = uncontestableColor;
        CancelInvoke();
        Invoke("Neutralize", uncontestableTime);
        amountOwnedIncrease = false;
        scoreIncrease = false;
    }

    void Neutralize()
    {
        contestable = true;
        AdjustCannonColor();
        InvokeRepeating("Fire", startTime, repeatTime);

    }

   public void init (int ownerNum_, int timesOwned_, int litSegments_)
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
            if (!completelyOwned)
            {


//                //destroy all unowned cannonballs
//                foreach (Cannonball c in cannonBallList)
//                {
//                    if (c)
//                    {
//                        if (c.ownerNum != ownerNum)
//                        {
//
//                            Destroy(c.gameObject);
//                        }
//                    }
//                    else
//                    {
//                    }
//                }
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
