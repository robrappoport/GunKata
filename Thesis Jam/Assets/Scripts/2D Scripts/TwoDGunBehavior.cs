using UnityEngine;
using System.Collections;

public class TwoDGunBehavior: MonoBehaviour
{
	//Drag in the Bullet Emitter from the Component Inspector.
	public GameObject Bullet_Emitter;
	[HideInInspector]
	public BulletManager bulletManager;
	public TwoDGameManager gameManager;
	public TwoDCharacterController myCont;
	//Drag in the Bullet Prefab from the Component Inspector.
	public GameObject RyuBullet, HeavyBulletPrimary, HeavyBulletSecondary;
	public GameObject BigBullet;
    public ParticleSystem pSys;
	[HideInInspector]
	public int playerNum;
    //Enter the Speed of the Bullet from the Component Inspector.
    public float Bullet_Forward_Force;
	private float bulletOffsetNorm = 0f;
//	private float bulletOffsetRight = -180f;
//	private float bulletOffsetLeft = 0f;
//	public float Bullet_Exist_Time;
	public int MaxBullets;
	public int CurrentBullets;
	public float ReloadTime;
	public float ChargeRate;
	public float Charge;
	public float maxCharge;
	public string weaponLabel;

	public enum Loadout {Ryu, Heavy};
	public Loadout[] loadout;
	public Loadout currentWeapon;

	private bool isReloading;
	private int weaponSwitchCounter = 0;
    // Use this for initialization
    void Start()
    {
        Charge = ChargeRate;
        CurrentBullets = MaxBullets;
        bulletManager = GetComponent<BulletManager>();
        pSys.Stop();
		currentWeapon = loadout [0];
		weaponLabel = loadout [0].ToString () + " Gun";
    }
	// Update is called once per frame
	void Update ()
	{
		Debug.DrawRay (transform.position, transform.forward * 50, Color.red);
		if (isReloading) {
			return;
		}

		if (Charge >= maxCharge)
        {
            pSys.Play();
        }
		
		if (myCont.secondaryFire () == true && Charge >= maxCharge) {
			chargeShot ();
            pSys.Stop();
        } 

		if (myCont.secondaryFire () == true && Charge < maxCharge)
		{
			SecondaryFire ();
		}
		if (myCont.primaryFire () == true)
		{
			PrimaryFire ();
		}
		if (myCont.rightBumperPressed()) {
			SwitchWeapons ();
		}
		if (myCont.yButton () == true && CurrentBullets > 0) {
			Charge += Time.deltaTime;
//			Debug.Log (Charge);
		} else
			Charge = ChargeRate;

	}
	void SwitchWeapons(){
		weaponSwitchCounter += 1;
		currentWeapon = loadout [weaponSwitchCounter  % 2];

		weaponLabel = currentWeapon.ToString () + " Gun";
	}
	void PrimaryFire(){
		CurrentBullets -= 1;
		if (CurrentBullets <= 0){
			StartCoroutine (NormalReload ());
			return;
		}
		switch(currentWeapon)
		{
		case Loadout.Ryu:
			bulletManager.CreateBullet (
				RyuBullet, 
				Bullet_Emitter.transform.position, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
			break;
		case Loadout.Heavy:
			bulletManager.CreateBullet (
				HeavyBulletPrimary, 
				Bullet_Emitter.transform.position + transform.forward,
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
			bulletManager.CreateBullet (
				HeavyBulletPrimary, 
				Bullet_Emitter.transform.position + transform.forward,
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
			break;
		}

	}

	void SecondaryFire (){
		switch (currentWeapon) 
		{
		case Loadout.Ryu:
			
			CurrentBullets -= 3;
			if (CurrentBullets <= 0){
				StartCoroutine (SpecialReload ());
				return;
			}
			bulletManager.CreateBullet (RyuBullet, Bullet_Emitter.transform.position, Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
			bulletManager.CreateBullet (RyuBullet, Bullet_Emitter.transform.position, Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm + 10f, 0)));
			bulletManager.CreateBullet (RyuBullet, Bullet_Emitter.transform.position, Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm - 10f, 0)));
			break;

		case Loadout.Heavy:
			CurrentBullets -= 3;
			if (CurrentBullets <= 0){
				StartCoroutine (SpecialReload ());
				return;
			}
			bulletManager.CreateBullet (
				HeavyBulletSecondary, 
				Bullet_Emitter.transform.position + (transform.forward) * 4 + transform.right * 3, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
			bulletManager.CreateBullet (
				HeavyBulletSecondary, 
				Bullet_Emitter.transform.position + (transform.forward) * 4 - transform.right * 3, 
				Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0))
			);
			break;
		}
	}

	IEnumerator NormalReload(){
		isReloading = true;
		yield return new WaitForSeconds (ReloadTime);
		CurrentBullets = MaxBullets;
		isReloading = false;
	}

	IEnumerator SpecialReload (){
//		Debug.Log (CurrentBullets);
		isReloading = true;
		if (CurrentBullets != 0) {
			bulletManager.Freeze (true);
		} 

		else {
			gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
			Debug.Log (playerNum+"player");
		}
			yield return new WaitForSeconds (ReloadTime);
			CurrentBullets = MaxBullets;
		isReloading = false;
		bulletManager.Freeze (false);
		}


	void chargeShot (){
		CurrentBullets -= 4;
		bulletManager.CreateBullet (BigBullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));

	}



}