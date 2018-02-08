﻿using UnityEngine;
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
    public GameObject AuraObj;
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

    [Header ("SPRITE AURA VARS")]
    public GameObject sprAura;
    public float tempAuraScaleMin;
    private float tempAuraScaleCurrent;
    public float tempAuraGrowthRate;

    [Header("Charge Shot Vars")]
    public float buttonDownTime;
    public float initchargeThreshold;
    public float chargeTime;
    public float loadedChargeTime;
    public float laserLengthPercent;
    //Cave Story Gun Behavior Bools//
    bool gunLevel1, gunLevel2, gunLevel3;

    //AUDIO
    private AudioSource myAudio;
    public AudioClip [] playerSounds;

    private TwoDGameManager gameManager;
    private AuraCharacterController myCont;

    void Start()
    {
        //find all its own components and static objects 
        gameManager = FindObjectOfType<TwoDGameManager>();
        myCont = GetComponent<AuraCharacterController>();
        bulletManager = GetComponent<BulletManager>();
        myAudio = GetComponent<AudioSource>();
        curStamina = staminaTotal;
        //auraBaseScale = AuraObj.transform.localScale;
        //auraInitScale = auraBaseScale;
        sprAura.GetComponent<Renderer>().enabled = false;
        sprAura.transform.localScale *= tempAuraScaleMin;
        isFiring = false;
        shootTime = 0;
        //      Debug.Log ("Player Number"+playerNum);
        CurrentBullets = MaxBullets;


    }
    // Update is called once per frame
    void Update()
    {

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

        auraProject();


        //if (myCont.secondaryFire () && !isExhausted) {

        //      } else if ((auraInitScale != auraBaseScale) && !isExhausted)
        //{
        ////    auraContract ();
        //}



        if (CurrentBullets > 0)
        {
            if (myCont.primaryFireDown() == true)
            {
                buttonDownTime += Time.deltaTime;
                //              Debug.Log ("is pressing button");
                if (shootTime <= 0)
                {
                    isFiring = true;
                    shootTime = initShootTime;
                    PrimaryFire();
                    StartCoroutine(ShootSound());
                    if (myCont.primaryFire() == true && buttonDownTime >= initchargeThreshold)
                    {
                        chargeTime += Time.deltaTime;
                        int wingMatChangeValue = Mathf.FloorToInt((chargeTime / loadedChargeTime) * 5f);
                        myCont.shootSlowDown();
                        for (int i = 0; i < wingMatChangeValue; i++)
                        {
                            if (i < wingMatChangeValue)
                            {
                                //change the color of i
                                laserLengthPercent = chargeTime / loadedChargeTime;
                            }
                        }
                        if (chargeTime >= loadedChargeTime)
                        {
                            //play charge sound
                        }
                        if (myCont.primaryFireUp())
                        {
                            bulletManager.CreateBullet(
                                LaserBullet,
                                Bullet_Emitter.transform.position,
                                Quaternion.Euler(new Vector3(0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
                            
                        }

                    }

                }
            }
            else
            {
                myCont.NotShot();
                buttonDownTime = 0f;
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

    void auraProject()
    {
       

        if (myCont.secondaryFireDown() && 100 - curStamina < .00000001/*&& !isExhausted && !isProjecting && !isContracting*/)
        {
            standardHalo.Clear();
            standardHalo.Pause();
            DamagedHalo.Play();
            StartCoroutine(AuraSound());
            //AuraObj.transform.position = transform.position;
            isProjecting = true;
            //AuraObj.SetActive(true);
            timeElapsed = 0f;
            //auraInitScale = AuraObj.transform.localScale;
            pressedWhileExhausted = false;
            tempAuraScaleCurrent = tempAuraScaleMin;
            sprAura.GetComponent<Renderer>().enabled = true;
        }

        if (myCont.secondaryFireUp())
        {
            if (isProjecting)
            {
                sprAura.GetComponent<Renderer>().enabled = false;
                isProjecting = false;
                AuraGenerator aura = Instantiate(AuraObj, this.gameObject.transform.position,
                            Quaternion.Euler(0, 0, 0))
                    .GetComponent<AuraGenerator>();
                aura.Init(playerNum, tempAuraScaleCurrent);
                tempAuraScaleCurrent = tempAuraScaleMin;
                sprAura.transform.localScale = new Vector3(1, 1, 1);
                sprAura.transform.localScale *= tempAuraScaleCurrent;
            }
        }

        if (curStamina > 0)
        {
            

            if (myCont.secondaryFire() && isProjecting/* && !isExhausted && !isContracting*/)
            {
                curStamina -= staminaRate;
                timeElapsed += Time.deltaTime * tempAuraGrowthRate;
                SetStamina();
                if (curStamina <= 0)
                {
                    curStamina = 0;
                    isExhausted = true;
                    isProjecting = false;
                    pressedWhileExhausted = true;
                    isContracting = true;
                    sprAura.GetComponent<Renderer>().enabled = false;
                }
                tempAuraScaleCurrent += tempAuraGrowthRate * Time.deltaTime;
                if (tempAuraScaleCurrent >= 1f)
                {
                    tempAuraScaleCurrent = 1f;
                }
                sprAura.transform.localScale = new Vector3(1, 1, 1);
                sprAura.transform.localScale *= tempAuraScaleCurrent;
               
                //AuraObj.transform.localScale = Vector3.Lerp(auraInitScale,
                //auraBaseScale * auraMultiplier, timeElapsed / duration);
            }

            /*if (isExhausted){
                //recharge
                curStamina += staminaRate;
                SetStamina();
                if (curStamina >= staminaTotal / 2f){
                    isExhausted = false;
                }
            }*/

            //auraInitScale = AuraObj.transform.localScale;
        }
        if (curStamina < staminaTotal && !isProjecting)
        {
            //recharge
            //Gabe changed this
            curStamina += staminaRate * .5f;
            SetStamina();
            //this is amount needed to be able to Aura again
            if (curStamina >= staminaTotal)
            {
                standardHalo.Play();
                isExhausted = false;
            }
            //Contraction happens here
           
        }

        if (isContracting)
        {
            //auraContract();
        }


        //while (curStamina <= 0f || isExhausted)
        //{
        //    //isRecharging = true;
        //    StartCoroutine(auraRecharge());
        //    AuraObj.SetActive(false);
        //    return;
        //}
    }

    void auraContract()
    {
        if (curStamina <= 0f)
        {
            isProjecting = false;
            timeElapsed = 0f;
            //auraInitScale = AuraObj.transform.localScale;
        }
        else
        {
            curStamina += staminaRate * .2f;
            SetStamina();
            timeElapsed += Time.deltaTime;

            //AuraObj.transform.localScale = Vector3.Lerp(auraInitScale, auraBaseScale, timeElapsed / duration);
            if (timeElapsed >= duration)
            {
                Debug.Log("Is");
                isContracting = false;
                //AuraObj.SetActive(false);
            }
        }
    }

    IEnumerator auraRecharge()
    {
        //      Debug.Log ("test");
        while (curStamina < (staminaTotal))
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("In while");
            curStamina += staminaRate;
            SetStamina();
            if (curStamina >= (staminaTotal))
            {
                Debug.Log("while is done");
                isExhausted = false;
                break;
            }
        }

    }

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

    public void SetStamina()
    {
        staminaBar.fillAmount = curStamina * .01f;

        if (curStamina > staminaTotal)
        {
            curStamina = staminaTotal;
        }

    }

    public void turretOwnerCheck ()
    {
        //check how many turrets the player owns//

        //if player owns 1 turret
        //else if player owns 2
        //else if player owns 3
    }
   
}