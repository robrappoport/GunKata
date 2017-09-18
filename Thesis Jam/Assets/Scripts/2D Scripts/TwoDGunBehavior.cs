using UnityEngine;
using System.Collections;

public class TwoDGunBehavior: MonoBehaviour
{
	//Drag in the Bullet Emitter from the Component Inspector.
	public GameObject Bullet_Emitter;
	[HideInInspector]
	public BulletManager bulletManager;
	public TwoDGameManager gameManager;
	//Drag in the Bullet Prefab from the Component Inspector.
	public GameObject Bullet;
	public GameObject BigBullet;
    public ParticleSystem pSys;
    [HideInInspector]
	public int playerNum;
    //Enter the Speed of the Bullet from the Component Inspector.
    public float Bullet_Forward_Force;
//	public float Bullet_Exist_Time;
	public float MaxBullets;
	public float CurrentBullets;
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
		
		if (TwoDCharacterController.instance.yButtonUp () == true && Charge >= maxCharge) {
			chargeShot ();
            pSys.Stop();
        } 

		if (TwoDCharacterController.instance.yButtonUp () == true && Charge < maxCharge)
		{
			Debug.Log ("is doing things");
			Shoot ();
		}
		if (TwoDCharacterController.instance.yButton () == true && CurrentBullets > 0) {
			Charge += Time.deltaTime;
//			Debug.Log (Charge);
		} else
			Charge = ChargeRate;
			



	}

	void Shoot ()
	{
		CurrentBullets--;
		//The Bullet instantiation happens here.
		GameObject Temporary_Bullet_Handler;
		Temporary_Bullet_Handler = Instantiate (Bullet, Bullet_Emitter.transform.position, Bullet_Emitter.transform.rotation) as GameObject;

		//Sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
		//This is EASILY corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
		Temporary_Bullet_Handler.transform.Rotate (Vector3.left);

		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
		Rigidbody Temporary_RigidBody;
		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody> ();

		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
		Temporary_RigidBody.AddForce (-transform.right * Bullet_Forward_Force);
		bulletManager.AddBullet (Temporary_Bullet_Handler.GetComponent<Bullet>());
//		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
//		Destroy (Temporary_Bullet_Handler, Bullet_Exist_Time);

//		Debug.Log (CurrentBullets);

	}
	IEnumerator Reload (){
		isReloading = true;
		Debug.Log (CurrentBullets);
		if (CurrentBullets != 0) {
			bulletManager.Freeze (true);
		} 

		else {
			gameManager.players [((playerNum - 1) + 1) % 2].bulletManager.Freeze (true);
		}
			yield return new WaitForSeconds (ReloadTime);
			CurrentBullets = MaxBullets;
		isReloading = false;
		}


	void chargeShot (){
		CurrentBullets-= 2;
		GameObject Temporary_Bullet_Handler;
		Temporary_Bullet_Handler = Instantiate (BigBullet, Bullet_Emitter.transform.position, Bullet_Emitter.transform.rotation) as GameObject;

		//Sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
		//This is EASILY corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
		Temporary_Bullet_Handler.transform.Rotate (Vector3.left);

		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
		Rigidbody Temporary_RigidBody;
		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody> ();

		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
		Temporary_RigidBody.AddForce (-transform.right * Bullet_Forward_Force);
		bulletManager.AddBullet (Temporary_Bullet_Handler.GetComponent<Bullet>());

		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
//		Destroy (Temporary_Bullet_Handler, Bullet_Exist_Time);
//		Debug.Log (CurrentBullets);

	}



}