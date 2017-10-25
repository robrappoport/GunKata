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
	public string weaponLabel;

	public enum Loadout {spray, heavy};
	public Loadout[] loadout;
	public Loadout currentWeapon;

	private bool isReloading;
	private int weaponSwitchCounter = 0;
    // Use this for initialization
    void Start()
    {
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
		if ((MaxBullets - CurrentBullets) % specialBulletPeriod == specialBulletPeriod - 1) {
			Debug.Log ("playing");
			pSys.Play ();
		} else if (pSys.isPlaying){
			Debug.Log ("stopping");
			pSys.Stop ();
		}

			
		Debug.DrawRay (transform.position, transform.forward * 50, Color.red);
		if (isReloading) {
			return;
		}
			
		if (CurrentBullets > 0) {
			if (myCont.secondaryFire () == true) {
				SecondaryFire ();
			}
			if (myCont.primaryFire () == true) {
				PrimaryFire ();
			}
		}
		if (myCont.rightBumperPressed()) {
			SwitchWeapons ();
		}
		if (myCont.xButtonUp () || (CurrentBullets <= 0 && !isReloading)){
			Reload ();
		}

	}

	void Reload(){
		if (CurrentBullets != MaxBullets) {
			myCont.OnShot ();
			if ((MaxBullets - CurrentBullets) % specialBulletPeriod == specialBulletPeriod - 1) {

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
		myCont.OnShot ();
		CurrentBullets -= 1;
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
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm + 5f, 0))
			);
			bulletManager.CreateBullet (
				RyuBullet, 
				Bullet_Emitter.transform.position, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm - 5f, 0))
			);
			break;
		case Loadout.heavy:
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
			
			CurrentBullets -= 3;
			
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

		case Loadout.heavy:
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
				left_Side_Emitter.transform.position /* + (transform.forward) * 4 + transform.right * 3 */, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
			bulletManager.CreateBullet (
				HeavyBulletSecondary, 
				right_Side_Emitter.transform.position /*+ (transform.forward) * 4 - transform.right * 3*/, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
			break;
		}
	}

	IEnumerator NormalReload(){

		isReloading = true;
		//gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
		yield return new WaitForSeconds (ReloadTime);
		CurrentBullets = MaxBullets;
		isReloading = false;
		bulletManager.Freeze (false);
	}

	IEnumerator Special (){

//		Debug.Log (CurrentBullets);
//		int invert = 0;
//		if (playerNum == 1) {
//			invert = 0;
//		} else {
//			invert = 1;
//		}
//		isReloading = true;
//		if (CurrentBullets != 0) {
////			bulletManager.Freeze (false);
//
//
//			gameManager.players [(invert)].bulletManager.Freeze (true);
//		}
//		else {
//			Debug.Log (playerNum+"player");
//			gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
//			Debug.Log (playerNum+"player");
//		}
		gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
			yield return new WaitForSeconds (ReloadTime);
		isReloading = false;
		bulletManager.Freeze (false);
		}


}