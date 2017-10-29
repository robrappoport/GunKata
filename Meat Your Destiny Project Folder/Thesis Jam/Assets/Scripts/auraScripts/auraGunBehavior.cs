using UnityEngine;
using System.Collections;

public class auraGunBehavior: MonoBehaviour
{
	//Drag in the Bullet Emitter from the Component Inspector.
	public GameObject Bullet_Emitter;
	public BulletManager bulletManager;
	public TwoDGameManager gameManager;
	public AuraCharacterController myCont;
	//Drag in the Bullet Prefab from the Component Inspector.
	public GameObject RyuBullet;
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

	void Start()
	{
		isFiring = false;
		shootTime = 0;
		Debug.Log ("Player Number"+playerNum);
		CurrentBullets = MaxBullets;
		bulletManager = GetComponent<BulletManager>();

	}
	// Update is called once per frame
	void Update ()
	{
		if (shootTime > 0) {
			Debug.Log ("test shootime");
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

		if (CurrentBullets > 0) {
			if (myCont.secondaryFire () == true) {
				auraProject ();
			}
			if (myCont.primaryFire () == true) {
				Debug.Log ("is pressing button");
				if (shootTime <= 0) {
					Debug.Log ("is shooting");
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
		//aura goes here
	}
		

	IEnumerator NormalReload(){
		yield return new WaitForSeconds (ReloadTime);
		CurrentBullets = MaxBullets;
		isReloading = false;
		bulletManager.Freeze (false);


	}
}