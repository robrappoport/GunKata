using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{


	//	public Renderer topRenderer, middleRenderer, bottomRenderer;

	public Color p1Color, p2Color, neutralColor, currentColor, uncontestableColor, unownedColor;
	//public GameObject CannonballPrefab;
	public int litSegments = 0, ownerNum = 2, timesOwned = 0, maxTimesCanBeOwned, chargeIncrementSign = 1;
	public float startTime, repeatTime, immuneTime, uncontestableTime, keyUncontestableTime, spinSpeed;
	public List<Renderer> SegmentsList;
	public EZObjectPools.EZObjectPool objectPool;
	public float charge, chargeSpeed = 1;
	public bool charging = false, withinTimerLimits = true, isSpinning, key, hasAddedToBall = false;

	public enum Owner { Player1, Player2, NONE };
	public Owner owner = Owner.NONE;

	//GameObject Cannon;
	public bool completelyOwned = false, contestable = true;

	public List<Cannonball> cannonBallList = new List<Cannonball>();

    public List<GameObject> Emitters = new List<GameObject>();
	public Transform EmitterRotator;
	public GameObject[] turretTypes;
	public GameObject[] impactPrefabs;
	public Color[] playerColors;
	private Transform curTarget;
	private Vector3 targetDir;
	private Vector3 newDir;
	private Transform target;
    public LineRenderer lineRenderer;
	public int segmentNum;
	[Header("PARTICLE SYSTEM VARS")]
	public TurretParticles tp;
	public ParticleSystem lightningParticle, circleParticle, captureParticle;
	List<Collider> cols = new List<Collider> ();
	Collider auraCollider;
	ParticleSystem.MainModule lightningMain, circleMain, captureParticleMain;
	Animator anim;
	[Header("CHARGE PROGRESS VARS")]
	public Renderer[] letterRenderers = new Renderer[5];
	public Renderer[] piecesRenderers = new Renderer[6];
	public GameObject UICanvasPrefab;
	public GameObject UICanvas;
    public Vector3 UILocalPos;
	Quaternion UIRot;
	Vector3 UIPos;
	Image progressBar;
	Image outlineBar;
    List<Animator> emitterAnimators = new List<Animator>();
    public AudioClip captureSound;

	Collider myCollider;

	//CameraMultiTargetObjective camTar;
	//ownerNum will be received from the playerNum variable from AuraCharacterController script, where 2 acts as "none"
	//I know, I know, 0 makes you think "none" more than 2, but that's how the players are determined and I don't wanna fuck with that.
	void Awake(){
		
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.enabled = false;

		anim = GetComponent<Animator> ();
		segmentNum = letterRenderers.Length;


		lightningParticle = Instantiate (tp.chargeParticles, transform) as ParticleSystem;
		lightningParticle.transform.localPosition = tp.chargeParticlesLocation;
		lightningMain = lightningParticle.main;

		lightningParticle.gameObject.SetActive (false);
		circleParticle = Instantiate (tp.circleParticles, transform) as ParticleSystem;
		circleParticle.transform.localPosition = tp.circleParticlesLocation;
		circleMain = circleParticle.main;
		circleParticle.gameObject.SetActive (false);
		captureParticle = Instantiate (tp.captureParticles, transform) as ParticleSystem;
		captureParticle.transform.localPosition = tp.captureParticlesLocation;
		captureParticleMain = captureParticle.main;
		captureParticle.gameObject.SetActive (false);

		//set emitters
		//Emitter[0] = GameObject.Find(name + "/N_Emitter");
		//Emitter[1] = GameObject.Find(name + "/E_Emitter"); 
		charge = 0;
		ownerNum = 2;
		//myShooter = GetComponentInChildren<UbhShotCtrl> ();
		//set up segments here
		////		topRenderer = transform.Find("Turret Top").GetComponent<Renderer>();
		////		middleRenderer = transform.Find("Turret Middle").GetComponent<Renderer>();
		////		bottomRenderer = transform.Find("Turret Bottom").GetComponent<Renderer>();
		//		SegmentsList.Add (bottomRenderer);
		//		SegmentsList.Add (middleRenderer);
		//		SegmentsList.Add (topRenderer);
		//
		//Cannon = topRenderer.gameObject;

		p1Color = TwoDGameManager.thisInstance.playerHealth1.normalColor.color;
		p2Color = TwoDGameManager.thisInstance.playerHealth2.normalColor.color;
		currentColor = neutralColor;
		unownedColor = neutralColor;
		//InvokeRepeating("Fire", startTime, repeatTime);
		//amountOwnedIncrease = false;
	}


	void Start(){
		myCollider = GetComponent<Collider> ();
		if(key){
			TwoDGameManager.thisInstance.keyTurrets.Add(this);
			uncontestableTime = keyUncontestableTime;
		}
		RegisterTurret ();

//		objectPool = GameObject.Find ("Cannonball pool").GetComponent<EZObjectPools.EZObjectPool>();

        //get all emitters

        foreach(Transform t in transform){
            if(t.CompareTag("Rotator")){
                EmitterRotator = t;
                break;
            }
        }

        foreach(Transform t in EmitterRotator){
            if(t.GetChild(0).CompareTag("Emitter")){
                Emitters.Add(t.GetChild(0).gameObject);
            }
        }
        foreach (GameObject g in Emitters)
        {
            if (g.GetComponentInChildren<Animator>())
            {
                emitterAnimators.Add(g.GetComponentInChildren<Animator>());
            }
        }
        //find canvas

        UICanvas = Instantiate(UICanvasPrefab, gameObject.transform) as GameObject;
        if (UICanvas)
        {

            //TODO:alter the position of the UI relative to the transform here!
            UICanvas.transform.localPosition = UILocalPos;
            progressBar = UICanvas.transform.Find("TurretFill").GetComponent<Image>();
            outlineBar = UICanvas.transform.Find("TurretFillBack").GetComponent<Image>();
            UIRot = UICanvas.transform.rotation;
            UIPos = UICanvas.transform.position;

        }
		

	}



	// Update is called once per frame
	void Update()
	{
		if (UICanvas) {
			UICanvas.transform.rotation = UIRot;
			UICanvas.transform.position = UIPos;

		}
		AuraCheck();

		AimingTurret();

		if (charging && contestable) {
			charge = Mathf.Clamp (charge + chargeIncrementSign * Time.deltaTime * chargeSpeed, 0, letterRenderers.Length);
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

		DetermineDegreeOfOwnership();
		OnCapture ();
	


		if (auraCollider && charging && MismatchedOwners()) {
			AdjustOwnership (auraCollider.GetComponent<AuraGenerator> ().auraPlayerNum);
			lightningParticle.gameObject.SetActive (true);
			circleParticle.gameObject.SetActive (true);
			if (owner == Owner.Player1) {
				lightningMain.startColor = playerColors [0];
				circleMain.startColor = playerColors [0];
			} else {
				lightningMain.startColor= playerColors [1];
				circleMain.startColor = playerColors [1];
			}
		} else {
			lightningParticle.gameObject.SetActive (false);
			circleParticle.gameObject.SetActive (false);
		}

		//	CleanCannonballList ();

	}



	void OnCapture(){
        objectPool = GameObject.Find("Cannonball Pool " + ownerNum.ToString()).GetComponent<EZObjectPools.EZObjectPool>();
		if (completelyOwned) {
			Sound.me.Play(captureSound);
			StartCoroutine(TimeManipulation.SlowTimeTemporarily(0.7f, 0.1f, .2f, 0.1f));
			CancelInvoke ();
			//InvokeRepeating ("Fire", repeatTime - (Time.timeSinceLevelLoad % repeatTime), repeatTime);
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
			} else {
				neutralColor = unownedColor;
			}

			lineRenderer.enabled = true;
			StartCoroutine(DrawLineToPlayer(ownerNum));
			outlineBar.color = neutralColor;
			litSegments = 0;
			charge = 0;
			AdjustListMembership ();
			captureParticle.gameObject.SetActive (true);
			captureParticleMain.startColor = playerColors [ownerNum];
			captureParticle.Play ();
			foreach (Transform t in EmitterRotator) {
				foreach(Transform c in t.GetChild(0).transform){
					if (c.name == "Emitter_Rotate")
					{
						c.GetComponent<Renderer>().materials[1].color = neutralColor;
					}
				}
			}

		} 
	}
	public void RegisterTurret(){
		if (!TwoDGameManager.thisInstance.turrets [ownerNum].Contains (this)) {
			TwoDGameManager.thisInstance.turrets [ownerNum].Add (this);
		}
		if(!UIManager.thisInstance.turretList.Contains(this)){
			UIManager.thisInstance.turretList.Add (this);

			GetComponentInChildren<MeshRenderer> ().gameObject.AddComponent<CameraMultiTargetObjective> ();
		}
	}

	public IEnumerator FadeUIBar (float t = 1){
        if (UICanvas)
        {
            t = Mathf.Clamp(t, 0.00001f, Mathf.Infinity);
            bool allFaded = false;
            while (!allFaded)
            {
                allFaded = true;
                float step = 0;
                foreach (Image i in UICanvas.GetComponentsInChildren<Image>())
                {
                    step += Time.deltaTime;
                    i.color = Color.Lerp(i.color, Color.clear, step / t);
                    if (i.color.a > 0)
                    {
                        allFaded = false;
                    }
                }
                yield return null;
            }

            foreach (Image i in UICanvas.GetComponentsInChildren<Image>())
            {
                i.gameObject.SetActive(false);
            }
        }
	}

	void AdjustListMembership(){
		foreach(List<Turret> l in TwoDGameManager.thisInstance.turrets){//scan all turret lists and remove itself from the one containing this turret
			if(l.Contains(this)){
				l.Remove(this);
			}
		}
		//add itself to its new owner's list
		TwoDGameManager.thisInstance.turrets[ownerNum].Add(this);


	}


	void AuraCheck (){
		cols = TwoDGameManager.CleanColliderList (cols);
		auraCollider = AuraGenerator.GetCurrentAura (cols, myCollider);
		if (contestable) {

			if (auraCollider)
			{

				if (auraCollider.GetComponent<AuraGenerator>())
				{
					if (!auraCollider.GetComponent<AuraGenerator>().isSuper)
					{
						//ownerNum = col.gameObject.GetComponentInChildren<AuraGenerator> ().auraPlayerNum;
						if (litSegments < letterRenderers.Length)
						{
							charging = true;

						}


						if (auraCollider.GetComponent<AuraGenerator>().auraPlayerNum == 0)
						{
							owner = Owner.Player1;
						}
						else
						{
							owner = Owner.Player2;
						}
						litSegments = (int)charge;

						//if the intruding player is hitting the turret , increment the number of lit segments up to a max of 3
						if (ownerNum != auraCollider.GetComponent<AuraGenerator>().auraPlayerNum)
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
			else
			{
				auraCollider = null;
				charging = false;
			}

		}
		//if (col != col.gameObject.GetComponent<AuraGenerator>())
		//{
		//    charging = false;
		//    col = null;
		//}
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

    IEnumerator DrawLineToPlayer(int playerNum, float drawTime = 0.5f, float recedeTime = 0.25f){
        //retract the line
        if (lineRenderer)
        {
            float elapsedTime = 0;
            while (elapsedTime < recedeTime)
            {

                elapsedTime += Time.deltaTime;
                lineRenderer.SetPosition(1, Vector3.Lerp(lineRenderer.GetPosition(1), transform.position, elapsedTime / recedeTime));
                yield return null;
            }
            //draw the line;

            elapsedTime = 0;
            lineRenderer.material.color = TwoDGameManager.thisInstance.playerVibrantColors[playerNum];
            //lineRenderer.startColor = TwoDGameManager.thisInstance.playerColors[playerNum];
            //lineRenderer.endColor = TwoDGameManager.thisInstance.playerColors[playerNum];
            while (elapsedTime < drawTime)
            {
                elapsedTime += Time.deltaTime;
                lineRenderer.SetPosition(1, Vector3.Lerp(transform.position, TwoDGameManager.thisInstance.players[playerNum].transform.position, elapsedTime / drawTime));
                yield return null;
            }

            //maintain the line at position
            while (playerNum == ownerNum)
            {
                if (TwoDGameManager.thisInstance.GetPlayer(playerNum).activeInHierarchy) 
                {
                    lineRenderer.SetPosition(1, Vector3.Lerp(transform.position, TwoDGameManager.thisInstance.players[playerNum].transform.position, elapsedTime / drawTime));
                }else{
                    lineRenderer.SetPosition(1, transform.position);
                }
                yield return null;
            }
        }


        
    }
	void OnTriggerExit(Collider col){
		if (col.gameObject.GetComponent<AuraGenerator> ()) {
			charging = false;

			contestable = true;
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.GetComponent<AuraGenerator> ()) {
			cols.Add (col);

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

	void AdjustOwnership(int num)
	{
		//adjust ownership and color based on owner number;

		switch (num)
		{
		case 0: //player 1
			currentColor = p1Color;
			break;
		case 1: //player 2
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
		if (ownerNum != 2){
//		{
//			//check the owner number
//			if (ownerNum == 0)
//			{
//				//get the other player
//				curTarget = TwoDGameManager.thisInstance.players[1].transform;
//			}
//
//			if (ownerNum == 1)
//			{
//				//get the other player
//				curTarget = TwoDGameManager.thisInstance.players[0].transform;
//			}
//
			curTarget = TwoDGameManager.thisInstance.players[ownerNum].transform;
			//face that player over a period of time
			target = curTarget;
			float rotSpeed = 2f;
			targetDir = new Vector3(target.position.x, EmitterRotator.transform.position.y, target.position.z) - EmitterRotator.transform.position;
			float step = rotSpeed * Time.deltaTime;
			newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
			//Debug.DrawRay(transform.position, newDir, Color.red, 50f);
			transform.rotation = Quaternion.LookRotation(newDir);
		}


	}
	//	void AdjustCannonColor()
	//	{//adjusts turret based on the number of lit segments;
	//		
	//		if (litSegments > 0)
	//		{
	//			bottomRenderer.material.color = currentColor;
	//		}
	//		else
	//		{
	//			bottomRenderer.material.color = neutralColor;
	//		}
	//
	//		if (litSegments > 1)
	//		{
	//			middleRenderer.material.color = currentColor;
	//		}
	//		else
	//		{
	//			middleRenderer.material.color = neutralColor;
	//		}
	//
	//		if (litSegments > 2)
	//		{
	//			topRenderer.material.color = currentColor;
	//		}
	//		else
	//		{
	//			topRenderer.material.color = neutralColor;
	//		}
	//
	//	}

	void AdjustCannonColor(){
		if (progressBar) {
			progressBar.fillAmount = charge / segmentNum;

			progressBar.color =  Color.Lerp (neutralColor, currentColor, charge / segmentNum);
		}
		for (int i = 0; i < litSegments; i++) {//adjust the lit letters
			letterRenderers [i].material.color = currentColor;
			letterRenderers [i].material.SetColor ("_EmissionColor", currentColor);

		}
		for (int i = litSegments; i < segmentNum; i++) {//adjust the unlit letters
			letterRenderers[i].material.color = neutralColor;
			letterRenderers [i].material.SetColor ("_EmissionColor", neutralColor);

		}

		for (int i = 0; i < litSegments; i++) {
			piecesRenderers[i].materials[2].color = currentColor;
			piecesRenderers[i].materials[2].SetColor ("_EmissionColor", currentColor);

		}
		for (int i = litSegments; i < segmentNum; i++) {
			piecesRenderers[i].materials[2].color = neutralColor;
			piecesRenderers[i].materials[2].SetColor ("_EmissionColor", neutralColor);

		}

		if (litSegments >= segmentNum) {
			piecesRenderers [piecesRenderers.Length - 1].materials [2].color = currentColor;
		} else {
			piecesRenderers [piecesRenderers.Length - 1].materials [2].color = neutralColor;		
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
	public void Fire()
	{	 
		//CleanCannonballList ();

		foreach (GameObject Em in Emitters)
		{
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
				//	cannonBall.GetComponent<Renderer> ().material = cannonBall.GetComponent<Cannonball> ().player1BulletMaterial;
					Physics.IgnoreCollision (TwoDGameManager.thisInstance.player1.GetComponentInChildren<Collider> (), cannonBall.GetComponent<Collider> ());
					cannonBall.layer = LayerMask.NameToLayer ("Player1OwnsTurret");



				} else if (ownerNum == 1) {
					//cannonBall.GetComponent<Renderer> ().material = cannonBall.GetComponent<Cannonball> ().player2BulletMaterial;
					Physics.IgnoreCollision (TwoDGameManager.thisInstance.player2.GetComponentInChildren<Collider> (), cannonBall.GetComponent<Collider> ());
					cannonBall.layer = LayerMask.NameToLayer ("Player2OwnsTurret");

				} else {
					//cannonBall.GetComponent<Renderer> ().material.color = neutralColor;

				}
			}
        }foreach(Animator a in emitterAnimators){
            a.SetTrigger("Fire");
        }

	}

	public void Reset()
	{
        foreach (GameObject Em in Emitters)
        {
            Em.GetComponent<Renderer>().material.color = neutralColor;
        }
        CancelInvoke();
		ownerNum = 2;
		owner = Owner.NONE;
		neutralColor = unownedColor;
		litSegments = 0;
		completelyOwned = false;
		AdjustOwnership(2);
		AdjustCannonColor();
		progressBar.color = Color.white;
		outlineBar.color = Color.white;
		AdjustListMembership ();
		//		topRenderer.material.color = uncontestableColor;
		//		middleRenderer.material.color = uncontestableColor;
		//		bottomRenderer.material.color = uncontestableColor;
		contestable = false;
		bool turnOff = GetComponent<BallArrayScript>().on = false;
		hasAddedToBall = false;
		anim.ResetTrigger ("Open");
		anim.SetTrigger ("Close");
		Invoke("Neutralize", uncontestableTime);


	}

	void ResetAllColors(){
		for (int i = 0; i < letterRenderers.Length; i++) {
			letterRenderers [i].material.color = uncontestableColor;
		}
	}

	void Neutralize()
	{
		contestable = true;

	}

	public void Init (int ownerNum_, int timesOwned_, int litSegments_)
	{
		//Debug.Log(timesOwned_);
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
		if (litSegments > segmentNum - 1)
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
				anim.SetTrigger ("Open");
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
