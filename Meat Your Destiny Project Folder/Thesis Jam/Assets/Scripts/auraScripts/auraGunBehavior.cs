using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class auraGunBehavior: MonoBehaviour
{
	//Drag in the Bullet Emitter from the Component Inspector.
	public GameObject Bullet_Emitter;
	public BulletManager bulletManager;


	//Drag in the Bullet Prefab from the Component Inspector.
	public GameObject RyuBullet;
	public GameObject AuraObj;
	public int playerNum;
	private float bulletOffsetNorm = 0f;
	public int MaxBullets;
	public int CurrentBullets;
	public float ReloadTime;
	public float initShootTime;
	public float shootTime;
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
	private bool isRecharging;

	private TwoDGameManager gameManager;
	private AuraCharacterController myCont;

	void Start(){
		//find all its own components and static objects 	
		gameManager = FindObjectOfType<TwoDGameManager> ();
		myCont = GetComponent<AuraCharacterController> ();
		bulletManager = GetComponent<BulletManager>();

		curStamina = staminaTotal;
		auraBaseScale = AuraObj.transform.localScale;
		auraInitScale = auraBaseScale;
		isFiring = false;
		shootTime = 0;
//		Debug.Log ("Player Number"+playerNum);
		CurrentBullets = MaxBullets;
	

	}
	// Update is called once per frame
	void Update ()
	{
		if (shootTime > 0) {
//			Debug.Log ("test shootime");
			shootTime -= Time.deltaTime;
			isFiring = false;
		}

		Debug.DrawRay (transform.position, transform.forward * 50, Color.red);
		if (myCont.xButtonUp () && CurrentBullets < MaxBullets) {
			isReloading = true;
			buttonReload = true;
			Invoke ("Reload", .001f);
		} else {
		}

		if (CurrentBullets <= 0 && !isReloading && autoReloadEnabled == true) {
			isReloading = true;
			Invoke ("Reload", .001f);
		}
		if (isReloading) {
			return;
		}
		if (myCont.secondaryFire () && !isRecharging) {
			auraProject ();
		} else if (auraInitScale != auraBaseScale && !isRecharging)
		{
			auraContract ();
		}
			
				
		if (CurrentBullets > 0) {
			if (myCont.primaryFire () == true) {
//				Debug.Log ("is pressing button");
				if (shootTime <= 0) {
//					Debug.Log ("is shooting");
					isFiring = true;
					shootTime = initShootTime;
					PrimaryFire ();
				}
			}
		}

	}

	void Reload(){
		if (CurrentBullets != MaxBullets) {
			myCont.OnShot ();
			if (autoReloadEnabled == true && buttonReload != true) {
				autoReload = true;
			}
			StartCoroutine (NormalReload ());

		}
	}

	void PrimaryFire(){
		CurrentBullets -= 1;
		myCont.OnShot ();
			bulletManager.CreateBullet (
				RyuBullet, 
				Bullet_Emitter.transform.position, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
	}

	void auraProject ()
	{
		if (myCont.secondaryFireDown ()) {
			AuraObj.SetActive (true);
			timeElapsed = 0f;
			auraInitScale = AuraObj.transform.localScale;
		} 
		if (myCont.secondaryFire ()) {
			{
				while (curStamina <= 0f || isRecharging) 
				{	isRecharging = true;
					StartCoroutine (auraRecharge ());
					AuraObj.SetActive (false);
					return;
				}
				curStamina -= staminaRate;
				timeElapsed += Time.deltaTime;
				SetStamina ();
				AuraObj.transform.localScale = Vector3.Lerp (auraInitScale, auraBaseScale * auraMultiplier, timeElapsed / duration);

			}
			auraInitScale = AuraObj.transform.localScale;
		}
<<<<<<< HEAD

        if (myCont.secondaryFireUp()){
            auraInitScale = AuraObj.transform.localScale;
            isProjecting = false;
            timeElapsed = 0f;
        }

        if (myCont.secondaryFire() && !isExhausted && !pressedWhileExhausted)
        {
            
            if (isProjecting && !isExhausted)
            {
                curStamina -= staminaRate;
                timeElapsed += Time.deltaTime * 5;
                SetStamina();
                if (curStamina <=0){
                    curStamina = 0;
                    isExhausted = true;
                    pressedWhileExhausted = true;
                    AuraObj.SetActive(false);

                }
                AuraObj.transform.localScale = Vector3.Lerp(auraInitScale,
                auraBaseScale * auraMultiplier, timeElapsed / duration);
            }

            /*if (isExhausted){
                //recharge
                curStamina += staminaRate;
                SetStamina();
                if (curStamina >= staminaTotal / 2f){
                    isExhausted = false;
                }
            }*/

            auraInitScale = AuraObj.transform.localScale;
        }
        else {
            //recharge
            curStamina += staminaRate;
            SetStamina();
            if (curStamina >= staminaTotal / 2f)
            {
                isExhausted = false;
            }
            //Contraction happens here
            timeElapsed += Time.deltaTime * 1.5f;
            AuraObj.transform.localScale = Vector3.Lerp(auraInitScale, auraBaseScale, timeElapsed / duration);
        }



        //while (curStamina <= 0f || isExhausted)
        //{
        //    //isRecharging = true;
        //    StartCoroutine(auraRecharge());
        //    AuraObj.SetActive(false);
        //    return;
        //}
=======
>>>>>>> master
	}

	void auraContract ()
	{
		if (myCont.secondaryFireUp ()) {
			timeElapsed = 0f;
			auraInitScale = AuraObj.transform.localScale;
		} else {
			timeElapsed += Time.deltaTime;
			curStamina += staminaRate;
			SetStamina ();
			AuraObj.transform.localScale = Vector3.Lerp (auraInitScale, auraBaseScale, timeElapsed / duration);
			if (timeElapsed >= duration) {
				AuraObj.SetActive (false);
			}
		}
	}
		
	IEnumerator auraRecharge ()
	{
//		Debug.Log ("test");
		while (curStamina != staminaTotal) {
			yield return new WaitForSeconds (.0005f);
//			Debug.Log ("In while");
			curStamina += staminaRate;
			SetStamina ();
		}
		if (curStamina == staminaTotal) {
//			Debug.Log ("while is done");
			isRecharging = false;
		}
	}

	IEnumerator NormalReload(){
		yield return new WaitForSeconds (ReloadTime);
		CurrentBullets = MaxBullets;
		isReloading = false;
		bulletManager.Freeze (false);


	}

	public void SetStamina ()
	{
		staminaBar.fillAmount = curStamina*.01f;

		if (curStamina > staminaTotal) 
		{
			curStamina = staminaTotal;
		}

	}
}