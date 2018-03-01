using UnityEngine;
using System.Collections;
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
    private float bulletOffsetNorm = 0f;
    public int MaxBullets;
    public int CurrentBullets;
    public float ReloadTime;
    public float initShootTime;
    public float shootTime;
    public float shootVol;
    private bool isFiring;
    public bool autoReloadEnabled;
    private bool autoReload;
    private bool buttonReload;

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
    bool currentlyLerpingAuraSize = false;
    public float lerpDur;
    float currentLerpTimeElapsed;
    public GameObject sprAura;
    public GameObject AuraObj;
    public float tempAuraScaleMin;
    private float tempAuraScaleCurrent;
    public float tempAuraGrowthRate;
    public float refractoryPeriod;
    public int auraIndex;

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
	[Tooltip("The length of the buffer before the initial charge. Higher value results in a longer time before charge.")]
	public float initialChargeBuffer;
    public float loadedChargeTime;
    public float laserLengthPercent;
    public GameObject[] wings;
    public Color inactiveWingColor;
    public Color activeWingColor;
    public float totalLaserShotTime;
    public int wingMatChangeValue;
    private float laserFiring = 0f;
    private bool laserIsFiring = false;
    private GameObject laserObj;
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

	
        foreach (GameObject g in wings)
        {
            Renderer [] wingArray = g.GetComponentsInChildren<Renderer>();

            foreach (Renderer r in wingArray)
            {
                r.material.color = inactiveWingColor;
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
        refractoryPeriod = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        AuraSys();
        SetStamina(); 

        //AURA CHANGER
        if (myCont.yButton())
        {
            AuraObj = ProjectAuraObj;
        }
        if (myCont.xButton())
        {
            AuraObj = SlowAuraobj;
        }
        //AURA CHANGER

        if (shootTime > 0)
        {
            //          Debug.Log ("test shootime");
            shootTime -= Time.deltaTime;
            isFiring = false;
        }

        Debug.DrawRay(transform.position, transform.forward * 50, Color.red);
        if (myCont.xButtonUp() && CurrentBullets < MaxBullets)
        {
            isReloading = true;
            buttonReload = true;
            Invoke("Reload", .001f);
        }
        else
        {
        }

        if (CurrentBullets <= 0 && !isReloading && autoReloadEnabled == true)
        {
            isReloading = true;
            Invoke("Reload", .001f);
        }
        if (isReloading)
        {
            return;
        }
        //auraProject();


        //if (myCont.secondaryFire () && !isExhausted) {

        //      } else if ((auraInitScale != auraBaseScale) && !isExhausted)
        //{
        ////    auraContract ();
        //}



        if (CurrentBullets > 0)
        {
            if (chargeTime >= loadedChargeTime && !laserIsFiring)
            {
                chargeTime = loadedChargeTime;
                Debug.Log("laser fully charged");
                //play charge sound
            }
            if (myCont.primaryFireDown() == true || myCont.primaryFire() == true)
            {
                if (myCont.primaryFireDown() == true)
                {
                    isFiring = true;
                    shootTime = initShootTime;
                    PrimaryFire();
                    StartCoroutine(ShootSound());
                }
                if (myCont.primaryFire() == true)
                {
                    //Debug.Log(chargeTime);
                    //Debug.Log(loadedChargeTime);
                    //Debug.Log(chargeTime + " " + "chargetime");
					if (wingMatChangeValue == 0) {
						chargeTime += Time.deltaTime / initialChargeBuffer;
					} else {
						chargeTime += Time.deltaTime;
					}
                    wingMatChangeValue = Mathf.FloorToInt((chargeTime / loadedChargeTime) * 3f);
                    myCont.shootSlowDown();
                    if (wingMatChangeValue != tempValue){
                        for (int i = 0; i < wingMatChangeValue; i++)
                        {
                            Renderer [] wingArray = wings[i].GetComponentsInChildren<Renderer>();
                            foreach (Renderer r in wingArray)
                            {
                                r.material.color = activeWingColor;
                            }


                        }
                        tempValue = wingMatChangeValue;
                    }
                   



                }

            }
            else
            {
                myCont.NotShot();
            }
			if (myCont.primaryFireUp ())
			if (wingMatChangeValue== 0) {
				chargeTime = 0;
			} else {
				if (chargeTime >= 1) {
                    StartCoroutine(LaserShotSound());
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
            if (laserIsFiring)
            {
                gameObject.GetComponent<AuraCharacterController>().turnSpeed = .2f;
                gameObject.GetComponent<AuraCharacterController>().prevMoveForce = .2f;
                laserFiring += Time.deltaTime;
               //play laser sound

                if (laserFiring >= totalLaserShotTime)
                {
                    gameObject.GetComponent<AuraCharacterController>().turnSpeed = 20f;
                    gameObject.GetComponent<AuraCharacterController>().prevMoveForce = 4f;
                    foreach (GameObject g in wings)
                    {
                        Renderer [] wingArray = g.GetComponentsInChildren<Renderer>();
                        foreach (Renderer r  in wingArray)
                        {
                            r.material.color = inactiveWingColor;
                        }
                    }
                    laserFiring = 0f;
                    laserIsFiring = false;
                    laserObj.GetComponent<LaserShotScript>().on = false;
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
       
            CurrentBullets -= 1;
            //myCont.OnShot();
            bulletManager.CreateBullet(
                RyuBullet,
                Bullet_Emitter.transform.position,
                Quaternion.Euler(new Vector3(0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
        
      
    }

    int activeAura()
    {

        for (int i = 0; i < auraLevelCharge.Length; i++)
        {
            if (auraLevelCharge[i] >= 100f)
            {
                return i;
            }
           
        }
        return -1;
    }

    void AuraSys()
    {
        Debug.Log(heldCharges);
        //Debug.Log(activeAura() + "active aura");
        //if (!myCont.secondaryFire())
        //{
        //    refractoryPeriod -= Time.deltaTime;
        //    if (refractoryPeriod <= 0f)
        //    {
        //        refractoryPeriod = 0f;
        //    }
        //}
      
        // when we press the left trigger and you have at least 1 energy bar
        if (myCont.secondaryFireDown() && auraIndex > -1)
        {
            auraIndex = activeAura();
            Debug.Log(auraIndex + "aura index" + Time.time + "at time");
            //turn on the aura outline and set its scale to minimum
            tempAuraScaleCurrent = tempAuraScaleMin;
            sprAura.SetActive(true);
        }

        //while we hold down the left trigger and we still have at least 1 energy bar 
        if (myCont.secondaryFire() && auraIndex > -1 && auraIndex < auraLevelCharge.Length)
        {
            // if you have just pressed the button i.e. have no current aura charges, then quickly reduce the value of the current float to 0
            if (heldCharges == 0)
            {
                auraLevelCharge[auraIndex] -= Time.deltaTime * 50f;
            }
            //otherwise, reduce the value by this set slower amount
            else
            {
                auraLevelCharge[auraIndex] -= Time.deltaTime * 20f;
            }
            //if the current float we are on becomes less than or equal to zero, 
            if (auraLevelCharge[auraIndex] <= 0)
            {
                auraLevelCharge[auraIndex] = 0;  //set it to zero for sanity
                heldCharges++;
                if (auraIndex < auraLevelCharge.Length-1)
                {
                    auraIndex++; 
                }
               
               
                //start lerping the aura outline
                currentlyLerpingAuraSize = true;
                currentLerpTimeElapsed = 0f;
            }

            //lerp the aura outline over a period of time by the number of held charges
            if (currentlyLerpingAuraSize)
            {
                currentLerpTimeElapsed += Time.deltaTime;
                sprAura.transform.localScale = Vector3.Lerp(Mathf.Pow(1.1f, heldCharges)
                                                    * Vector3.one, Mathf.Pow(1.1f, heldCharges + 1)
                                                    * Vector3.one, currentLerpTimeElapsed / lerpDur);
                if (currentLerpTimeElapsed >= lerpDur)
                {
                    currentlyLerpingAuraSize = false;
                }
            }

        }
        // when you release the button, if the current aura charge is less than 100 but not zero, lerp it back to 100 very quickly
        if (myCont.secondaryFireUp())
        {
            //refractoryPeriod = 5f;
            heldCharges = 0;
            sprAura.SetActive(false);
            AuraGenerator aura = Instantiate(AuraObj, this.gameObject.transform.position,
                                  Quaternion.Euler(0, 0, 0))
                          .GetComponent<AuraGenerator>();
            aura.Init(playerNum, tempAuraScaleCurrent);
            tempAuraScaleCurrent = tempAuraScaleMin;
            sprAura.transform.localScale = new Vector3(1, 1, 1);
            if (auraLevelCharge[auraIndex] > 0f && auraLevelCharge[auraIndex] < 100f)
            {
                auraLevelCharge[auraIndex] = Mathf.Lerp(auraLevelCharge[auraIndex], 100f, (2 / 1));
            }
        }
    }
    public void SetStamina()
    {
        for (int i = 0; i < auraStamImgArray.Length; i++)
        {
            if (i < activeAura())
            {
                auraStamImgArray[i].fillAmount = 0f;
            }
            else
                auraStamImgArray[i].fillAmount = 1f;
            if (i == activeAura() - 1)
            {
                auraStamImgArray[i].fillAmount = auraLevelCharge[auraIndex] / auraLevelChargeMax;
            }
        }
    }


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
        Sound.me.Play(playerSounds[2], .8f, true);
        yield return null;
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