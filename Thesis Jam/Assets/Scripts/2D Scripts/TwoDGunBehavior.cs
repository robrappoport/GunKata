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
		GameObject Temporary_Bullet_Handler = (GameObject)GameObject.Instantiate (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
		//Sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
		//This is EASILY corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
		Temporary_Bullet_Handler.transform.Rotate (Vector3.left);

		//		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
		//		Rigidbody Temporary_RigidBody;
		//		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody> ();

		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
		//		Temporary_RigidBody.AddForce (-transform.right * Bullet_Forward_Force);
		bulletManager.AddBullet (Temporary_Bullet_Handler.GetComponent<Bullet>());

		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
		//		Destroy (Temporary_Bullet_Handler, Bullet_Exist_Time);
		//		Debug.Log (CurrentBullets);
	}

	void Shoot ()
	{
		CurrentBullets-= 2;

		GameObject Temporary_Bullet_Handler1 = (GameObject)GameObject.Instantiate (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));
		GameObject Temporary_Bullet_Handler2 = (GameObject)GameObject.Instantiate (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm + 10f,0)));
		GameObject Temporary_Bullet_Handler3 = (GameObject)GameObject.Instantiate (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm -10f,0)));
		//The Bullet instantiation happens here.
//		GameObject Temporary_Bullet_Handler;
//		Temporary_Bullet_Handler = Instantiate (Bullet, Bullet_Emitter.transform.position, Bullet_Emitter.transform.rotation) as GameObject;
//		Temporary_Bullet_Handler = Instantiate (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,0,10))) as GameObject;
//		Temporary_Bullet_Handler = Instantiate (Bullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,0,-10))) as GameObject;

		//Sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
		//This is EASILY corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
//		Temporary_Bullet_Handler1.transform.Rotate (Vector3.left);
//		Temporary_Bullet_Handler2.transform.Rotate (Vector3.left);
//		Temporary_Bullet_Handler3.transform.Rotate (Vector3.left);

		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
//		Rigidbody Temporary_RigidBody1;
//		Rigidbody Temporary_RigidBody2;
//		Rigidbody Temporary_RigidBody3;
//		Temporary_RigidBody1 = Temporary_Bullet_Handler1.GetComponent<Rigidbody> ();
//		Temporary_RigidBody2 = Temporary_Bullet_Handler2.GetComponent<Rigidbody> ();
//		Temporary_RigidBody3 = Temporary_Bullet_Handler3.GetComponent<Rigidbody> ();
//
//		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
//		Temporary_RigidBody1.AddForce (transform.forward * Bullet_Forward_Force);
//		Temporary_RigidBody2.AddForce (transform.forward * Bullet_Forward_Force);
//		Temporary_RigidBody3.AddForce (transform.forward * Bullet_Forward_Force);
		bulletManager.AddBullet (Temporary_Bullet_Handler1.GetComponent<Bullet>());
		bulletManager.AddBullet (Temporary_Bullet_Handler2.GetComponent<Bullet>());
		bulletManager.AddBullet (Temporary_Bullet_Handler3.GetComponent<Bullet>());
//		Destroy (Temporary_Bullet_Handler, Bullet_Exist_Time);

//		Debug.Log (CurrentBullets);

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
		CurrentBullets -= 3;
		GameObject Temporary_Bullet_Handler = (GameObject)GameObject.Instantiate (BigBullet, Bullet_Emitter.transform.position, Quaternion.Euler(new Vector3(0,Bullet_Emitter.transform.rotation.eulerAngles.y + bulletOffsetNorm, 0)));

		//Sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
		//This is EASILY corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
		Temporary_Bullet_Handler.transform.Rotate (Vector3.left);

//		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
//		Rigidbody Temporary_RigidBody;
//		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody> ();

		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
//		Temporary_RigidBody.AddForce (-transform.right * Bullet_Forward_Force);
		bulletManager.AddBullet (Temporary_Bullet_Handler.GetComponent<Bullet>());

		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
//		Destroy (Temporary_Bullet_Handler, Bullet_Exist_Time);
//		Debug.Log (CurrentBullets);

	}



}