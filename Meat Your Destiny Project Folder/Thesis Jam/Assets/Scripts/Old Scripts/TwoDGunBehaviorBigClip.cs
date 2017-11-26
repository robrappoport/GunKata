using UnityEngine;
using System.Collections;

public class TwoDGunBehaviorBigClip: MonoBehaviour
{
	//Drag in the Bullet Emitter from the Component Inspector.
	public GameObject Bullet_Emitter;
	public GameObject left_Side_Emitter;
	public GameObject right_Side_Emitter;
	[HideInInspector]
	public BulletManager bulletManager;
	public TwoDGameManager gameManager;
	public TwoDCharacterController myCont;
	//Drag in the Bullet Prefab from the Component Inspector.
	public GameObject RyuBullet, HeavyBulletPrimary, HeavyBulletSecondary, FastBullet;
//	public GameObject BigBullet;
    public ParticleSystem pSys;
//	[HideInInspector]
	public int playerNum;
    //Enter the Speed of the Bullet from the Component Inspector.
    public float Bullet_Forward_Force;
	private float bulletOffsetNorm = 0f;
//	private float bulletOffsetRight = -180f;
//	private float bulletOffsetLeft = 0f;
//	public float Bullet_Exist_Time;
	public int MaxBullets;
	public int CurrentBullets;
	public int specialBulletPeriod = 2;
	public float ReloadTime;
	public float initShootTime;
	public float shootTime;
	public string weaponLabel;
	private bool isFiring;
	public bool autoReloadEnabled;
	private bool autoReload;
	private bool buttonReload;

	public enum Loadout {spray, shield};
	public Loadout[] loadout;
	public Loadout currentWeapon;

	private bool isReloading;
	private int weaponSwitchCounter = 0;


	public AudioSource playerAudio;
	public AudioClip [] playerNoises;
    // Use this for initialization
    void Start()
    {
		isFiring = false;
		shootTime = 0;
		Debug.Log ("Player Number"+playerNum);
        CurrentBullets = MaxBullets;
        bulletManager = GetComponent<BulletManager>();
        pSys.Stop();
		currentWeapon = loadout [0];
		weaponLabel = loadout [0].ToString ();

    }
	// Update is called once per frame
	void Update ()
	{
		if (shootTime > 0) {
			shootTime -= Time.deltaTime;
			isFiring = false;
		}
		if ((MaxBullets - CurrentBullets) % specialBulletPeriod == specialBulletPeriod - 1) {
//			Debug.Log ("playing");
			pSys.Play ();
		} else if (pSys.isPlaying){
//			Debug.Log ("stopping");
			pSys.Stop ();
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
			
		if (CurrentBullets > 0) {
			if (myCont.secondaryFire () == true) {
				SecondaryFire ();
				playerAudio.clip = playerNoises [0];
				playerAudio.Play ();
			}
			if (myCont.primaryFire () == true) {
				if (shootTime <= 0) {
					isFiring = true;
					shootTime = initShootTime;
					PrimaryFire ();
					playerAudio.clip = playerNoises [0];
					playerAudio.Play ();

				}
			}
		}
		if (myCont.rightBumperPressed()) {
			SwitchWeapons ();
		}
			
	}

	void Reload(){
		if (CurrentBullets != MaxBullets) {
			myCont.OnShot ();
			if (autoReloadEnabled == true && buttonReload != true) {
				autoReload = true;
			}
			if ((MaxBullets - CurrentBullets) % specialBulletPeriod == specialBulletPeriod - 1 || autoReload == true) {
				autoReload = false;
				buttonReload = false;
				StartCoroutine (Special ());
			}
			StartCoroutine (NormalReload ());
		
		}
	}
	void SwitchWeapons(){
		weaponSwitchCounter += 1;
		currentWeapon = loadout [weaponSwitchCounter  % 2];

		weaponLabel = currentWeapon.ToString ();
	}
	void PrimaryFire(){
		CurrentBullets -= 1;
		myCont.OnShot ();
//		if (CurrentBullets <= 0){
//			StartCoroutine (NormalReload ());
//			return;
//		}
		switch(currentWeapon)
		{
		case Loadout.spray:
			bulletManager.CreateBullet (
				RyuBullet, 
				Bullet_Emitter.transform.position, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
//			bulletManager.CreateBullet (
//				RyuBullet, 
//				Bullet_Emitter.transform.position, 
//				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm - 5f, 0))
//			);

			break;
		case Loadout.shield:
			bulletManager.CreateBullet (
				HeavyBulletPrimary, 
				Bullet_Emitter.transform.position + transform.forward,
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
//			bulletManager.CreateBullet (
//				HeavyBulletPrimary, 
//				Bullet_Emitter.transform.position + transform.forward,
//				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
//			);

			break;
		}


	}

	void SecondaryFire (){
		

		switch (currentWeapon) 

		{
		case Loadout.spray:
			
			CurrentBullets -= 10;
			
//			if (CurrentBullets <= 0){
//				print ("reloading");
//
//				StartCoroutine (NormalReload ());
//				return;
//			}
			bulletManager.CreateBullet (FastBullet, Bullet_Emitter.transform.position, Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
//			bulletManager.CreateBullet (RyuBullet, Bullet_Emitter.transform.position, Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm + 10f, 0)));
//			bulletManager.CreateBullet (RyuBullet, Bullet_Emitter.transform.position, Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm - 10f, 0)));
			break;

		case Loadout.shield:
			CurrentBullets -= 3;
//			if ((MaxBullets - CurrentBullets)% specialBulletPeriod == 0){
//				StartCoroutine (Special ());
//				return;
//			}

//			if (CurrentBullets <= 0){
//				print ("reloading");
//
//				StartCoroutine (NormalReload ());
//				return;
//			}
			bulletManager.CreateBullet (
				HeavyBulletSecondary, 
				Bullet_Emitter.transform.position + transform.forward,
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
//			bulletManager.CreateBullet (
//				HeavyBulletSecondary, 
//				left_Side_Emitter.transform.position /* + (transform.forward) * 4 + transform.right * 3 */, 
//				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
//			);
//			bulletManager.CreateBullet (
//				HeavyBulletSecondary, 
//				right_Side_Emitter.transform.position /*+ (transform.forward) * 4 - transform.right * 3*/, 
//				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
//			);
			break;
		}
	}

	IEnumerator NormalReload(){
		playerAudio.clip = playerNoises [1];
		playerAudio.Play ();
		//gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
		yield return new WaitForSeconds (ReloadTime);
		CurrentBullets = MaxBullets;
		isReloading = false;
		bulletManager.Freeze (false);


	}

	IEnumerator Special (){

		var main = pSys.main;
		main.startColor = new Color(255, 0, 0, 255);
//		Vector3 originalScale = pSys.gameObject.transform.localScale;
//		Vector3 targetScale = originalScale + new Vector3 (30f, 30f, 30f);
//		float lerpTime = 2f;
//		pSys.gameObject.transform.localScale = Vector3.Lerp (originalScale, targetScale, lerpTime*Time.deltaTime);
		playerAudio.clip = playerNoises [1];
		playerAudio.Play();
		//gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
		myCont.isDashing = true;
		myCont.curForce = myCont.dashForce;

			yield return new WaitForSeconds (ReloadTime);
		myCont.isDashing = false;
		main = pSys.main;
		main.startColor = Color.yellow;
		pSys.Stop ();
		isReloading = false;
		bulletManager.Freeze (false);
		}


}