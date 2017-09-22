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
	public GameObject Bullet;
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

	private bool isReloading;
    // Use this for initialization
    void Start()
    {
        Charge = ChargeRate;
        CurrentBullets = MaxBullets;
        bulletManager = GetComponent<BulletManager>();
        pSys.Stop();
    }
	// Update is called once per frame
	void Update ()
	{
		Debug.DrawRay (transform.position, transform.forward * 50, Color.red);
		if (isReloading) {
			return;
		}
		if (CurrentBullets <= 0){
			StartCoroutine (Reload ());
			return;
		}

        if (Charge >= maxCharge)
        {
            pSys.Play();
        }
		
		if (myCont.yButtonUp () == true && Charge >= maxCharge) {
			chargeShot ();
            pSys.Stop();
        } 

		if (myCont.yButtonUp () == true && Charge < maxCharge)
		{
			Shoot ();
		}
		if (myCont.xButtonUp () == true)
		{
			SingleShoot ();
		}
		if (myCont.yButton () == true && CurrentBullets > 0) {
			Charge += Time.deltaTime;
//			Debug.Log (Charge);
		} else
			Charge = ChargeRate;
			



	}
	void SingleShoot()
	{
		CurrentBullets -= 1;
		bulletManager.CreateBullet (
			Bullet, 
			Bullet_Emitter.transform.position, 
			Quaternion.Euler (new Vector3 (0, Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
	}

	void Shoot ()
	{
		CurrentBullets-= 3;

		bulletManager.CreateBullet (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
		bulletManager.CreateBullet (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm + 10f,0)));
		bulletManager.CreateBullet (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm -10f,0)));

	}
	IEnumerator Reload (){
//		Debug.Log (CurrentBullets);
		isReloading = true;
		if (CurrentBullets != 0) {
			bulletManager.Freeze (true);
		} 

		else {
			gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
			Debug.Log (playerNum);
		}
			yield return new WaitForSeconds (ReloadTime);
			CurrentBullets = MaxBullets;
		isReloading = false;
		}


	void chargeShot (){
		CurrentBullets -= 4;
		bulletManager.CreateBullet (BigBullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));

	}



}