using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class TwoDCharacterController : MonoBehaviour {
	public static TwoDCharacterController instance;
	public InputDevice myController;
	public TwoDGunBehaviorBigClip gunBehave;
	LockOnScript lockedOn;
	//public InputDevice myController { get; set; }
	//    public Vector3 moveDirection;
	public Vector3 moveDirForward;
	public Vector3 moveDirSides;
	//	private Vector3 storeDir;
	private Vector3 directionPos;
	//	public Transform cameraTrans;
	public int playerNum;
	public float turnSpeed = 4f;
	//	public float walkSpeed = 2;
	public float maxDashTime = 1.0f;
	public float dashSpeed = 4.0f;
	public float dashStopSpeed = 0.1f;

	public float currentDashTime;
	//	private float currentSpeed = 0;
	public bool isDashing;
	private Quaternion previousRot;
	private Rigidbody characterCtr;


	public PlayerAction Move;
	public PlayerAction Shoot;
	public float moveForce;
	public float dashForce;
	float stuckTimer;
	public float stuckTime = .1f;
	//	public float dragForce;
	//	public float dashDrag;
	//	public float stopForce;
	//	public GameObject ringAttack;
	//	public GameObject bounceAttack;
	//	public GameObject StraightAttack;

	//ANIMATORS
	//	public Animator XAttackAnim;
	//	public Animator YAttackAnim;
	//	public Animator BAttackAnim;
	// Use this for initialization

	void Awake (){

		instance = this;
	}
	void Start () {
		lockedOn = GetComponent<LockOnScript> ();
		characterCtr = this.GetComponent<Rigidbody>();
		myController = InputManager.Devices[playerNum];
		previousRot = transform.rotation;
		currentDashTime = maxDashTime;
		isDashing = false;

		//ANIMATORS
		//		XAttackAnim = GetComponent<Animator>();
		//		YAttackAnim = GetComponent<Animator>();
		//		BAttackAnim = GetComponent<Animator>();

	}

	public void OnShot(){
		stuckTimer = stuckTime;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (transform.position.y != 6f) {
			transform.position = new Vector3 (transform.position.x, 6f, transform.position.z);
		}
		//		moveDirection.y = 0;
		myController = InputManager.Devices[playerNum];
		if (stuckTimer <= 0) {
			MoveCharacter ();
		} else {
			characterCtr.velocity = Vector3.zero;
		}
		//		storeDir = cameraTrans.right;
		//		MyCharacterActions ();
		stuckTimer -= Time.deltaTime;
	}

	private Vector3 OnMove() {
		return new Vector3(myController.LeftStickX, 0, myController.LeftStickY);

	}

	public Vector3 CameraMove (){
		return new Vector3 (myController.RightStickX, 0, myController.RightStickY); 
		print (myController.RightStickX);
	}

	public bool yButton (){
		return (myController.Action4.IsPressed);
	}
	public bool xButton (){
			return (myController.Action3.IsPressed);
	} 

	public bool xButtonUp (){
		return (myController.Action3.WasReleased);
	} 
	public bool secondaryFire (){
		return (myController.LeftTrigger.WasReleased);
	}
	public bool primaryFire (){
		return (myController.RightTrigger.WasReleased);
	} 

	public bool bButtonUp (){
		return (myController.Action2.WasReleased);
	}

	public bool onLock() {
		return myController.LeftBumper.IsPressed;
	}
	public bool Unlock()
	{
		return myController.LeftBumper.WasReleased;
	}

	public bool rightTriggerDown(){
		return myController.RightTrigger.IsPressed;
	}

	public bool leftTriggerDown(){
		return myController.LeftTrigger.IsPressed;
	}

	public bool rightBumperPressed(){
		return myController.RightBumper.WasReleased;
	}
	//	public bool YAttacking (){
	//		//ring attack goes here
	//		if (YAttack == true) {
	//			GameObject Temporary_Attack_Handler;
	//			Temporary_Attack_Handler = Instantiate(ringAttack,transform.position,transform.rotation) as GameObject;
	//			YAttackAnim.Play("RingExpansionAttack");
	//		}
	//	}

	//	public bool BAttacking (){
	//		//charge shot goes here
	//		if (BAttack == true) {
	//			BAttackAnim.Play ("nameHere");
	//		}
	//	}

	//	public bool Dodge (){
	//		if (Dodging == true) {
	//			//dodge scripting goes here
	//		}
	//	}

	private void MoveCharacter() {
		//		Debug.Log (isDashing);
		//		currentSpeed = walkSpeed;
		float curForce = moveForce;
		//		float curDrag = dragForce;
		//		Debug.Log (currentSpeed);
		//		moveDirection = OnMove();
		moveDirForward = new Vector3 (OnMove ().x, 0, 0);
		//		moveDirForward = storeDir * OnMove ().x;
		moveDirSides = new Vector3 (0,0,OnMove ().z);//cameraTrans.forward

		//		moveDirection.y = 0;

		if (bButtonUp () && gunBehave.CurrentBullets > 0) {
			gunBehave.CurrentBullets--;
			currentDashTime = 0.0f;
		}

		if (currentDashTime < maxDashTime) {
			isDashing = true;
			//Debug.Log (isDashing + "Isdashing in the thing");
			//moveDirection = new Vector3 (moveDirection.x * dashSpeed, 0, moveDirection.z * dashSpeed);
			currentDashTime += dashStopSpeed;
			curForce = dashForce;
			//			curDrag = dashDrag;

		} else {
			isDashing = false;
		}

		//moveDirection *= currentSpeed;

		//		if (moveDirection.magnitude < .25f) {
		//			//if (characterCtr.velocity.magnitude > 5f) {
		//			//characterCtr.AddForce (-characterCtr.velocity.normalized * stopForce);
		//			//}
		//		} else {
		//			moveDirection = moveDirection.normalized;

		characterCtr.AddForce((moveDirForward + moveDirSides).normalized * curForce/ Time.deltaTime);
		//		}

		directionPos = transform.position + (CameraMove());


		//		directionPos = transform.position + (storeDir * OnMove().x) + (cameraTrans.forward * OnMove().z);
		Vector3 dir = directionPos - transform.position;
//		Debug.Log (dir + "checking initial");

		dir.y = 0;

		//		characterCtr.AddForce (-characterCtr.velocity.normalized * curDrag * characterCtr.velocity.sqrMagnitude);


		//		 else {
		//			moveDirection = Vector3.zero;
		//		}
		//		RotateCharacter(moveDirection);

//		Debug.Log (CameraMove ().magnitude);
		if (CameraMove().magnitude < .01f) {
			directionPos = transform.position + (OnMove());
			dir = directionPos - transform.position;
//			Debug.Log (dir + "checking secondary");
			characterCtr.rotation = 
				Quaternion.Slerp(transform.rotation,
					Quaternion.LookRotation(dir), 
					turnSpeed * Time.deltaTime);
			return;
		} 

		else {
			if (!lockedOn.LockedOn) {
				//				float angle = Mathf.Atan2 (myController.LeftStickY, myController.LeftStickX) * Mathf.Rad2Deg;
				//				previousRot = Quaternion.Euler (new Vector3 (0, -angle + 90, 0));
				//
				//				characterCtr.MoveRotation(Quaternion.Euler (new Vector3 (0, -angle + 90, 0)));
				characterCtr.rotation = 
					Quaternion.Slerp(transform.rotation,
						Quaternion.LookRotation(dir), 
						turnSpeed * Time.deltaTime);
			} else {
			}
		}
		//		Debug.Log (isDashing+"is dashing outside of the thing");


	}



}

