using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class auraGunBehavior : MonoBehaviour
{
    //Drag in the Bullet Emitter from the Component Inspector.
    public GameObject Bullet_Emitter;
    public BulletManager bulletManager;
    public Turret turretMan;
    //Drag in the Bullet Prefab from the Component Inspector.
    public GameObject RyuBullet;
    public GameObject LaserBullet;
    //public GameObject AuraObj;
    public GameObject ProjectAuraObj;
    public GameObject SlowAuraobj;

    public int playerNum;

	private auraPlayerHealth health;
    private float bulletOffsetNorm = 0f;
	[Header("SHOOTING VARS")]
	public float shootingStaminaDrainRate;
	public int MaxBullets;
    public int CurrentBullets;
    public float ReloadTime;
    public float initShootTime;
    public float shootTime;
    public float shootVol;
	public float shootAnimationDelay;
	private bool isFiring;
    public bool autoReloadEnabled;
    private bool autoReload;
    private bool buttonReload;
	private float shootingStaminaCost;


    private bool isReloading;
    public Vector3 auraInitScale;
    public Vector3 auraBaseScale;
    public float auraMultiplier;
    public float timeElapsed = 0f;
    public float duration = 3f;
    public float staminaRate = 1f;
    public float staminaTotal;
    public float curStamina;
    public Image staminaBar;
    //DEPCRECATED//
    public bool isExhausted;
    //DEPCRECATED//
    private bool isProjecting;
    private bool isContracting;
    public bool pressedWhileExhausted;
    public ParticleSystem standardHalo, DamagedHalo;

    [Header("NEW AURA VERSION VARS")]
    public float[] auraLevelCharge;
    public float auraLevelChargeMax;
    int heldCharges;
    public Image[] auraStamImgArray;
	public int staminaSegmentNum; 
    bool currentlyLerpingAuraSize = false;
    public float lerpDur;
    float currentLerpTimeElapsed;
    public GameObject sprAura;
    public GameObject AuraObj;
    public float tempAuraScaleMin;
    private float tempAuraScaleCurrent;
    public float tempAuraGrowthRate;
    public float curCoolDownAmt;
    public float coolDownTotal;
    public bool coolingDown = false;
    public int auraIndex;
    private float[] auraScales = new float[6] {.5f,.8f,1f,1.2f,1.6f, 2f};
    public float auraDrainRate;
    private float displayDrainRate = 100f;
    private float staminaToDisplay;
    private int startingAuraIndex;
	int chargeIndex;
    Animator wingAnim;

	//Manual Turret Fire vars
	List<Turret> myTurrets = new List<Turret>();


	public float auraChargeRate = 1f, auraRechargeRate = 1f, coolDownDuration;
	public float currentAuraCharge, remainingStamina, currentAuraChargeLimit;


    //[Header ("SPRITE AURA VARS")]
    //public GameObject sprAura;
    //public float tempAuraScaleMin;
    //private float tempAuraScaleCurrent;
    //public float tempAuraGrowthRate;

    [Header("Super Vars")]
    public bool superReady;
    public float tempSuperAuraGrowthRate;

    [Header("Charge Shot Vars")]
    public float buttonDownTime;
	public float chargeTime; 
	public float laserStaminaDrainRate = 1;
	[Tooltip("The length of the buffer before the initial charge. Higher value results in a longer time before charge.")]
	public float initialChargeBuffer;
    public float loadedChargeTime;
    public float laserLengthPercent;
    public GameObject[] wings;
    public Material wingMat;
    public Color inactiveWingColor;
    public Color activeWingColor;
    public float totalLaserShotTime;
    public int wingMatChangeValue;
    private float laserFiring = 0f;
    private bool laserIsFiring = false;
    private GameObject laserObj;
    public ParticleSystem laserChargeSys, laserShotSys;
    //Cave Story Gun Behavior Bools//
    bool gunLevel1, gunLevel2, gunLevel3;

    //AUDIO
    private AudioSource myAudio;
    public AudioClip [] playerSounds;

    private TwoDGameManager gameManager;
    private AuraCharacterController myCont;

    private int tempValue;

    void Start()
    {
        
		currentAuraChargeLimit = staminaSegmentNum;
		remainingStamina = currentAuraChargeLimit;
        foreach (GameObject g in wings)
        {
            Renderer [] wingArray = g.GetComponentsInChildren<Renderer>();

            foreach (Renderer r in wingArray)
            {
                r.material.SetColor("_EmissionColor", inactiveWingColor);
            }
        }
        //find all its own components and static objects 
        gameManager = FindObjectOfType<TwoDGameManager>();
        myCont = GetComponent<AuraCharacterController>();
        bulletManager = GetComponent<BulletManager>();
        myAudio = GetComponent<AudioSource>();
        curStamina = staminaTotal;
        //auraBaseScale = AuraObj.transform.localScale;
        //auraInitScale = auraBaseScale;
        sprAura.SetActive(false);
        sprAura.transform.localScale *= tempAuraScaleMin;
        isFiring = false;
        shootTime = 0;
        //      Debug.Log ("Player Number"+playerNum);
        CurrentBullets = MaxBullets;
        for (int i = 0; i < auraLevelCharge.Length; i++)
        {
            auraLevelCharge[i] = auraLevelChargeMax;
        }
		health = GetComponent<auraPlayerHealth> ();
		myTurrets = TwoDGameManager.thisInstance.turrets [playerNum];
		shootingStaminaCost = (float)staminaSegmentNum / MaxBullets;   
        foreach(Animator a in GetComponentsInChildren<Animator>()){
            if(a.name == "Wings"){
                wingAnim = a;
                break;
            }
        }

    }

    // Update is called once per frame
    void Update()
	{		
		drawStamina ();
		if (!health.dying) {
			AuraCharge ();
			ChangeAura ();
			Shoot ();
		}

        //AuraSys();
       // drawStamina(); 

        //AURA CHANGER
        //AURA CHANGER


        //auraProject();


        //if (myCont.secondaryFire () && !isExhausted) {

        //      } else if ((auraInitScale != auraBaseScale) && !isExhausted)
        //{
        ////    auraContract ();
        //}


    }

	void ChangeAura(){
		if (myCont.yButton())
		{
			AuraObj = ProjectAuraObj;
		}
		if (myCont.xButton ()) {
			AuraObj = SlowAuraobj;
		}

	}
	void Shoot(){
		if (shootTime > 0)
		{
			//          Debug.Log ("test shootime");
			shootTime -= Time.deltaTime;
			isFiring = false;
		}

		//Debug.DrawRay(transform.position, transform.forward * 50, Color.red);

		if (CurrentBullets <= 0 && !isReloading && autoReloadEnabled == true)
		{
			isReloading = true;
			Invoke("Reload", .001f);
		}
		if (isReloading) {
			return;
		}

		if (CurrentBullets > 0) {
			if (chargeTime >= loadedChargeTime && !laserIsFiring) {
				chargeTime = loadedChargeTime;
				//Debug.Log("laser fully charged");
				//play charge sound
			}
			if (myCont.primaryFireDown () == true || myCont.primaryFire () == true) {
				if (myCont.primaryFireDown () == true && remainingStamina > shootingStaminaCost) {
					isFiring = true;
					shootTime = initShootTime;
					PrimaryFire ();
					StartCoroutine (ShootSound ());
					StartCoroutine (DrainAuraOverTime (shootingStaminaCost, shootingStaminaDrainRate));
				}
				if (myCont.primaryFire () == true && remainingStamina > shootingStaminaCost) {
					//Debug.Log(chargeTime);
					//Debug.Log(loadedChargeTime);
					//Debug.Log(chargeTime + " " + "chargetime");

					if (wingMatChangeValue == 0) {
						chargeTime += Time.deltaTime / initialChargeBuffer;
					} else {
						if (!laserChargeSys.isPlaying) {
							LaserChargeSound ();
						}

					
							chargeTime += Time.deltaTime;

						if (chargeTime < loadedChargeTime) {
							DrainAura (Time.deltaTime * laserStaminaDrainRate);
						}
					}
					myCont.shootSlowDown ();

					wingMatChangeValue = Mathf.FloorToInt ((chargeTime / loadedChargeTime) * 3f);
					// DEPRECATED: wings changing color on laser charge
					//                    if (wingMatChangeValue != tempValue){
					//                        for (int i = 0; i < wingMatChangeValue; i++)
					//                        {
					//                            Renderer [] wingArray = wings[i].GetComponentsInChildren<Renderer>();
					//                            foreach (Renderer r in wingArray)
					//                            {
					//                                //r.material.c = activeWingColor;
					//                                r.material.SetColor("_EmissionColor", activeWingColor);
					//                            }
					//
					//
					//                        }
					//                        tempValue = wingMatChangeValue;
					//                    }




				}

			} else {
				myCont.NotShot ();
			}
			if (myCont.primaryFireUp ())
			if (wingMatChangeValue == 0) {
				chargeTime = 0;
			} else {
				if (chargeTime >= 1) {
					StartCoroutine (LaserShotSound ());
					laserChargeSys.Stop ();
					myCont.GetComponent<Animator> ().SetBool ("Laser Firing", true);
					laserIsFiring = true;
					chargeTime = 0f;

					laserObj = Instantiate (LaserBullet, 
						Bullet_Emitter.transform.position, 
						gameObject.transform.rotation) 
						as GameObject;
					laserObj.transform.parent = gameObject.transform;
					laserObj.GetComponent<LaserShotScript> ().on = true;
					laserObj.GetComponent<LaserShotScript> ().owner = myCont;
				}
			}
			if (laserIsFiring) {
				gameObject.GetComponent<AuraCharacterController> ().turnSpeed = .5f;
				gameObject.GetComponent<AuraCharacterController> ().prevMoveForce = .2f;
				laserFiring += Time.deltaTime;
				//play laser sound

				if (laserFiring >= totalLaserShotTime) {
					myCont.GetComponent<Animator> ().SetBool ("Laser Firing", false);
					laserShotSys.Stop ();
					gameObject.GetComponent<AuraCharacterController> ().turnSpeed = 20f;
					gameObject.GetComponent<AuraCharacterController> ().prevMoveForce = 4f;
					//                    foreach (GameObject g in wings)
					//                    {
					//                        Renderer [] wingArray = g.GetComponentsInChildren<Renderer>();
					//                        foreach (Renderer r  in wingArray)
					//                        {
					//                            //r.material.color = inactiveWingColor;
					//                            r.material.SetColor("_EmissionColor", inactiveWingColor);
					//                        }
					//                    }
					laserFiring = 0f;
					laserIsFiring = false;
					laserObj.GetComponent<LaserShotScript> ().on = false;
				}
			}
		}

		
	}
    void Reload()
    {
        if (CurrentBullets != MaxBullets)
        {
            //myCont.OnShot();
            if (autoReloadEnabled == true && buttonReload != true)
            {
                autoReload = true;
            }
            StartCoroutine(NormalReload());

        }
    }

    void PrimaryFire()
    {
		CancelInvoke ("StopAttacking");
		myCont.GetComponent<Animator> ().SetBool ("Attacking", true);
		Invoke ("StopAttacking", shootAnimationDelay);
        CurrentBullets -= 1;
        //myCont.OnShot();
        bulletManager.CreateBullet(
            RyuBullet,
            Bullet_Emitter.transform.position,
            Quaternion.Euler(new Vector3(0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
		//fire all turrets
		foreach(Turret t in myTurrets){
			t.Fire ();
		}
    
      
    }
	IEnumerator DrainAuraOverTime(float cost = 1, float drainRate = 1){
		float totalDrainAmount = 0;
		while (totalDrainAmount < cost) {
			DrainAura (Time.deltaTime * drainRate);
			totalDrainAmount += Time.deltaTime * drainRate;
			yield return null;
		}

	}

	public void StopAttacking(){
		myCont.GetComponent<Animator> ().SetBool ("Attacking", false);
	}
    int activeAura()
    {

        for (int i = 0; i < auraLevelCharge.Length; i++)
        {
            if (auraLevelCharge[i] > 0)
            {
                return i;
            }
           
        }
        return -1;
    }
		
	public void DrainAura(float difference){
		remainingStamina = Mathf.Clamp (remainingStamina - difference, 0, staminaSegmentNum);
		CancelInvoke ("ResetAuraCooldown");
		coolingDown = true;
		Invoke ("ResetAuraCooldown", coolDownDuration);

	
	}
	void AuraCharge(){
		if (myCont.secondaryFireDown () == true) {
			currentAuraChargeLimit = remainingStamina;
			CancelInvoke ("ResetAuraCooldown");
		}
		if (myCont.secondaryFire () == true) {
			
			//calculate how much charge remains and how much is to be used
			sprAura.SetActive (true);
			DrainAura (Time.deltaTime * auraChargeRate);
			if (remainingStamina > 0) {
				currentAuraCharge = Mathf.Clamp (currentAuraCharge + Time.deltaTime * auraChargeRate, 0, currentAuraChargeLimit);
			}
			if (staminaSegmentNum - currentAuraCharge < 0.02f) {
				chargeIndex = staminaSegmentNum;
			} else {
				chargeIndex = (int)currentAuraCharge;
			}

	//		remainingAuraCharge = Mathf.Clamp (remainingAuraCharge - Time.deltaTime * auraChargeRate, 0, auraStamImgArray.Length);
	//		currentAuraCharge = Mathf.Clamp (currentAuraCharge + Time.deltaTime * auraChargeRate, 0, currentAuraChargeLimit);
			//change the wing materials
			if (currentAuraCharge != tempValue) {
				//adjust the charge loop to account for the first and last turns
				for (int i = 0; i <= chargeIndex; i++) {
					Renderer[] wingArray = wings [Mathf.Clamp (i, 0, staminaSegmentNum - 1)].GetComponentsInChildren<Renderer> ();
					foreach (Renderer r in wingArray) {
						//r.material.c = activeWingColor;
						if (chargeIndex != staminaSegmentNum) {
							r.material.SetColor ("_EmissionColor", activeWingColor);
						} else {
							r.material.SetColor ("_EmissionColor", Color.white);
							r.material.color = Color.white; 
						}
					}
			
				}
				tempValue = wingMatChangeValue;
			}
			

	
			//lerp the outline to the target scale
			Vector3 targetScale = auraScales [Mathf.Clamp (chargeIndex, 0, auraScales.Length - 1)] * Vector3.one;
			sprAura.transform.localScale = Vector3.Lerp (sprAura.transform.localScale, targetScale, 0.7f);
		} else {
			if (!coolingDown) {
				//recharge the aura over time
				remainingStamina = Mathf.Clamp (remainingStamina + Time.deltaTime * auraRechargeRate, 0, staminaSegmentNum);
			}
			//restore the wings to their neutral color or make them "dead" depending on remaining charge

			foreach (GameObject g in wings) {
				Renderer[] wingArray = g.GetComponentsInChildren<Renderer> ();
				foreach (Renderer r  in wingArray) {
					if (remainingStamina > 0.02f) {

						//anim.settrigger("open wings")
						r.material.SetColor ("_EmissionColor", inactiveWingColor);
						r.material.color = inactiveWingColor;
                        wingAnim.SetBool("Out Of Stamina", false);
					} else {
						r.material.SetColor ("_EmissionColor", new Color(0, 0, 0, 0));
						r.material.color = Color.black;
                        wingAnim.SetBool("Out Of Stamina", true);

					}
				}

			}

		}

		if (myCont.secondaryFireUp () == true) {
            StartCoroutine(AuraSound());
            coolingDown = true;
			if(currentAuraCharge > 0){//only instantiate an aura if one "charge" has been used
				AuraGenerator aura = Instantiate(AuraObj, this.gameObject.transform.position,
					Quaternion.Euler(0, 0, 0))
					.GetComponent<AuraGenerator>();
				aura.Init(playerNum, auraScales[Mathf.Clamp(chargeIndex, 0, auraScales.Length - 1)]);
			}
			sprAura.transform.localScale = new Vector3(1, 1, 1);
			sprAura.SetActive(false);
			if (currentAuraCharge < 1) {
				remainingStamina = (int)remainingStamina;
			}

			currentAuraCharge = 0f;
			Invoke ("ResetAuraCooldown", coolDownDuration);
		}
	}
	void ResetAuraCooldown(){
		coolingDown = false;
	}

	public void drawStamina()
	{
        UIManager.thisInstance.UpdatePlayerCanvas(playerNum, remainingStamina);
		////draw all full bars
		//for (int i = 0; i < staminaSegmentNum; i++) {
		//	if ((int)remainingStamina > i ) {
		//		auraStamImgArray [i].fillAmount = 1;
		//	}

		//}
		////draw the remainder
		//if (remainingStamina < staminaSegmentNum) {
		//	auraStamImgArray [(int)remainingStamina].fillAmount = remainingStamina - (int)remainingStamina;
		//	//drain the empties
		//	for(int i = (int)remainingStamina + 1; i < staminaSegmentNum; i++){
		//		auraStamImgArray [i].fillAmount = 0;
		//	}
		//}
		//        for (int i = 0; i < auraStamImgArray.Length; i++)
		//        {
		//            //float targetStamina = 
		//            //staminaToDisplay = Mathf.MoveTowards(staminaToDisplay, targetStamina, displayChangeSpeed * Time.deltaTime);
		//		
		//            auraStamImgArray[i].fillAmount = auraLevelCharge[i] / auraLevelChargeMax;
		//
		//        }
	}
    /*void AuraSys()
    {
        
        if (coolingDown)
        {
            curCoolDownAmt -= Time.deltaTime;
        }
      
        // when we press the left trigger and you have at least 1 energy bar
        if (myCont.secondaryFireDown() == true && activeAura() > -1)
        {
            
            coolingDown = false;
            curCoolDownAmt = 0f;
            auraIndex = activeAura();
            startingAuraIndex = activeAura();
            //Debug.Log(auraIndex + "aura index" + Time.time + "at time");
            //turn on the aura outline and set its scale to minimum
            tempAuraScaleCurrent = tempAuraScaleMin;
            sprAura.SetActive(true);
        }

        //not holding button
        if (!myCont.secondaryFire() == true && curCoolDownAmt <= 0f){
            //recharge anything that's not full
            for (int i = auraLevelCharge.Length-1; i >=0; i--){
                //iterate over auras until we find one that isn't fully charged, then charge it a bit
                if (auraLevelCharge[i] < auraLevelChargeMax){
                    auraLevelCharge[i] += Time.deltaTime * auraRechargeRate;
                    auraLevelCharge[i] = Mathf.Clamp(auraLevelCharge[i], 0, auraLevelChargeMax);
                    break;
                }
            }

        }

        //while we hold down the left trigger and we still have at least 1 energy bar 
        if (myCont.secondaryFire() == true && auraIndex > -1 && auraIndex < auraLevelCharge.Length)
        {
            // if you have just pressed the button i.e. have no current aura charges, then quickly reduce the value of the current float to 0
            if (heldCharges == 0)
            {
                auraLevelCharge[auraIndex] -= Time.deltaTime * auraDrainRate;
            }
            //otherwise, reduce the value by this set slower amount
            else
            {
                auraLevelCharge[auraIndex] -= Time.deltaTime * auraDrainRate;
            }
            //if the current float we are on becomes less than or equal to zero, 
            if (auraLevelCharge[auraIndex] <= 0)
            {
               // auraLevelCharge[auraIndex] = 0;  //set it to zero for sanity
               
                //if it's not the last bar 
                if (auraIndex < auraLevelCharge.Length){
                    if (auraIndex != startingAuraIndex)
                    {
                        heldCharges++;
                    }
                   
                    if (auraIndex < auraLevelCharge.Length)
                    {
                        auraIndex++; 
                    }
                }
               
               
                //start lerping the aura outline
    
            }

            //lerp the aura outline over a period of time by the number of held charges
            //if (currentlyLerpingAuraSize)
            //{
            //    currentLerpTimeElapsed += Time.deltaTime;
            //    sprAura.transform.localScale = Vector3.Lerp(Mathf.Pow(1.1f, heldCharges-1)
            //                                        * Vector3.one, Mathf.Pow(1.1f, heldCharges)
            //                                        * Vector3.one, currentLerpTimeElapsed / lerpDur);
            //    if (currentLerpTimeElapsed >= lerpDur)
            //    {
            //        currentlyLerpingAuraSize = false;
            //    }
            //}
            Vector3 targetScale = auraScales[Mathf.Min(heldCharges,auraLevelCharge.Length-1)] * Vector3.one;
            sprAura.transform.localScale = Vector3.Lerp(sprAura.transform.localScale, targetScale,0.7f);

        }

        if (myCont.secondaryFireUp() == true && activeAura() > -1)
        {
            StartCoroutine(AuraSound());
            curCoolDownAmt = coolDownTotal;
            coolingDown = true;

            if (auraIndex == auraLevelCharge.Length) {
                auraIndex--;
            }

            //only make an aura if we made at least one charge



            //currentLerpTimeElapsed = 0f;
            //auraLevelCharge[auraIndex] = 0f;
            if (heldCharges == 0)
            { 
                StartCoroutine(drainToZero(auraIndex));
            }
           //immediately drain
            sprAura.SetActive(false);
            AuraGenerator aura = Instantiate(AuraObj, this.gameObject.transform.position,
                                  Quaternion.Euler(0, 0, 0))
                          .GetComponent<AuraGenerator>();
            aura.Init(playerNum, auraScales[Mathf.Min(heldCharges, auraLevelCharge.Length - 1)]);
            tempAuraScaleCurrent = tempAuraScaleMin;
            sprAura.transform.localScale = new Vector3(1, 1, 1);
            //if (auraLevelCharge[auraIndex] > 0f && auraLevelCharge[auraIndex] < 100f)
            //{
            //    auraLevelCharge[auraIndex] = Mathf.Lerp(auraLevelCharge[auraIndex], 100f, (2 / 1));
            //}


           
            heldCharges = 0;

        }
    
    }
    */
//    IEnumerator drainToZero (int auraIndexToDrain)
//
//    {
//        float currentLevel = auraLevelCharge[auraIndexToDrain];
//        while (currentLevel > 0f)
//        {
//            currentLevel = Mathf.MoveTowards(currentLevel, 0f, displayDrainRate * Time.deltaTime);
//            auraLevelCharge[auraIndexToDrain] = currentLevel;
//            yield return 0f;
//        }
//
//    }


    //void auraProject()
    //{
       

    //    if (myCont.secondaryFireDown() && 100 - curStamina < .00000001/*&& !isExhausted && !isProjecting && !isContracting*/)
    //    {

			
    //        standardHalo.Clear();
    //        standardHalo.Pause();
    //        DamagedHalo.Play();
    //        StartCoroutine(AuraSound());
    //        //AuraObj.transform.position = transform.position;
    //        isProjecting = true;
    //        //AuraObj.SetActive(true);
    //        timeElapsed = 0f;
    //        //auraInitScale = AuraObj.transform.localScale;
    //        pressedWhileExhausted = false;
    //        tempAuraScaleCurrent = tempAuraScaleMin;
    //        sprAura.SetActive(true);

    //    }

    //    if (myCont.secondaryFireUp())
    //       {
    //        if (isProjecting)
    //        {
               

    //                sprAura.SetActive(false);
    //                isProjecting = false;
    //                AuraGenerator aura = Instantiate(AuraObj, this.gameObject.transform.position,
    //                            Quaternion.Euler(0, 0, 0))
    //                    .GetComponent<AuraGenerator>();
    //                aura.Init(playerNum, tempAuraScaleCurrent);
    //            if (superReady)
    //            {
    //                aura.isSuper = true;
    //                superReady = false;
    //            }
    //                tempAuraScaleCurrent = tempAuraScaleMin;
    //                sprAura.transform.localScale = new Vector3(1, 1, 1);
    //                sprAura.transform.localScale *= tempAuraScaleCurrent;

              

    //        }
    //    }
    
    //    if (curStamina > 0)
    //    {
            
    //        if (!superReady){
    //            if (myCont.secondaryFire() && isProjecting)
    //            {
    //                curStamina -= staminaRate;
    //                timeElapsed += Time.deltaTime * tempAuraGrowthRate;
    //                SetStamina();
    //                if (curStamina <= 0)
    //                {
    //                    curStamina = 0;
    //                    isExhausted = true;
    //                    isProjecting = false;
    //                    pressedWhileExhausted = true;
    //                    isContracting = true;
    //                    sprAura.SetActive(false);
    //                }
    //                tempAuraScaleCurrent += tempAuraGrowthRate * Time.deltaTime;
    //                if (tempAuraScaleCurrent >= 1f)
    //                {
    //                    tempAuraScaleCurrent = 1f;
    //                }
    //                sprAura.transform.localScale = new Vector3(1, 1, 1);
    //                sprAura.transform.localScale *= tempAuraScaleCurrent;

    //                //AuraObj.transform.localScale = Vector3.Lerp(auraInitScale,
    //                //auraBaseScale * auraMultiplier, timeElapsed / duration);
    //            }

    //            /*if (isExhausted){
    //                //recharge
    //                curStamina += staminaRate;
    //                SetStamina();
    //                if (curStamina >= staminaTotal / 2f){
    //                    isExhausted = false;
    //                }
    //            }*/

    //            //auraInitScale = AuraObj.transform.localScale;
    //        }
    //        else
    //        {
    //            if (myCont.secondaryFire() && isProjecting)
    //            {
    //                curStamina = staminaTotal;
    //                timeElapsed += Time.deltaTime * tempSuperAuraGrowthRate;
    //                SetStamina();

    //                tempAuraScaleCurrent += tempSuperAuraGrowthRate * Time.deltaTime;
    //                if (tempAuraScaleCurrent >= 1000f)
    //                {
    //                    tempAuraScaleCurrent = 1000f;
    //                }
    //                sprAura.transform.localScale = new Vector3(1, 1, 1);
    //                sprAura.transform.localScale *= tempAuraScaleCurrent;

    //                //AuraObj.transform.localScale = Vector3.Lerp(auraInitScale,
    //                //auraBaseScale * auraMultiplier, timeElapsed / duration);
    //            }
    //        }
    //        if (curStamina < staminaTotal && !isProjecting)
    //        {
    //            //recharge
    //            //Gabe changed this
    //            curStamina += staminaRate * .5f;
    //            SetStamina();
    //            //this is amount needed to be able to Aura again
    //            if (curStamina >= staminaTotal)
    //            {
    //                standardHalo.Play();
    //                isExhausted = false;
    //            }
    //            //Contraction happens here

    //        }
    //        }
           
    //}

    //void auraContract()
    //{
    //    if (curStamina <= 0f)
    //    {
    //        isProjecting = false;
    //        timeElapsed = 0f;
    //        //auraInitScale = AuraObj.transform.localScale;
    //    }
    //    else
    //    {
    //        curStamina += staminaRate * .2f;
    //        SetStamina();
    //        timeElapsed += Time.deltaTime;

    //        //AuraObj.transform.localScale = Vector3.Lerp(auraInitScale, auraBaseScale, timeElapsed / duration);
    //        if (timeElapsed >= duration)
    //        {
    //            Debug.Log("Is");
    //            isContracting = false;
    //            //AuraObj.SetActive(false);
    //        }
    //    }
    //}

    //IEnumerator auraRecharge()
    //{
    //    //      Debug.Log ("test");
    //    while (curStamina < (staminaTotal))
    //    {
    //        yield return new WaitForSeconds(1f);
    //        Debug.Log("In while");
    //        curStamina += staminaRate;
    //        SetStamina();
    //        if (curStamina >= (staminaTotal))
    //        {
    //            Debug.Log("while is done");
    //            isExhausted = false;
    //            break;
    //        }
    //    }

    //}

    IEnumerator NormalReload()
    {
        yield return new WaitForSeconds(ReloadTime);
        CurrentBullets = MaxBullets;
        isReloading = false;
        //bulletManager.Freeze(false);


    }

    IEnumerator ShootSound()
    {
        Sound.me.Play(playerSounds[0], shootVol, true);
            //myAudio.clip = playerSounds[0];
            //myAudio.Play();
            yield return null;
        //yield return new WaitForSeconds(myAudio.clip.length * .2f);
        //myAudio.Stop();
    
    }

    IEnumerator AuraSound()
    {
        Sound.me.Play(playerSounds[1], 1f, true);
        yield return null;
        /*if (!myAudio.isPlaying)
        {
            myAudio.clip = playerSounds[1];
            myAudio.Play();
            yield return new WaitForSeconds(myAudio.clip.length);
            myAudio.Stop();
        }*/
    }

    IEnumerator LaserShotSound()
    {
        laserShotSys.Play();
		Sound.me.Stop (playerSounds [3]);
		Sound.me.Play (playerSounds [2], .8f, false);
        yield return null;
    }

	void LaserChargeSound()
    {

        //THIS DOESN'T WORK RIGHT NOW
        laserChargeSys.Play();
		Sound.me.Play (playerSounds [3]);
//		if (myCont.primaryFireUp ()) {
//			myAudio.Stop ();
//		} else {
//			yield return new WaitForSeconds (myAudio.clip.length);
//			myAudio.Stop ();
//		}
        //THIS DOESN'T WORK RIGHT NOW


    }

    //public void SetStamina()
    //{
    //    for (int i = 0; i < auraStamArray.Length; i++)
    //    {
    //        if (i < currentAuraCharge)
    //        {
    //            auraStamArray[i].fillAmount = 1f;
    //        }
    //        else
    //            auraStamArray[i].fillAmount = 0f;
    //        if (i == currentAuraCharge -1)
    //        {
    //            //auraStamArray[i].fillAmount = some float / how long it takes to completely charge 1 aura charge
    //        }
    //    }

    //    staminaBar.fillAmount = curStamina * .01f;

    //    if (curStamina > staminaTotal)
    //    {
    //        curStamina = staminaTotal;
    //    }

    //}

}