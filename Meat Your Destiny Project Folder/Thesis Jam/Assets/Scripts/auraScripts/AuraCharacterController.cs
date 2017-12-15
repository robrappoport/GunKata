using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class AuraCharacterController : PlayControl {
	public static PlayControl instance;
	public InputDevice myController;

//	LockOnScript lockedOn;
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
	public float curForce;
	public float moveForce;
	public float dashForce;
	public float slowForce;
    public float shootForce;
    public float prevMoveForce;
	float stuckTimer;
	public float stuckTime = .1f;
	public enum ControlType {Keyboard, Controller, NONE};/// <summary>
	/// Keyboard controls are as follows: WASD to move, YGHJ to rotate, V to shoot, left shift to project aura.
	/// </summary>
	ControlType controlType;

	private auraGunBehavior gunBehave;
    public float hitStunnedTimer;
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
        //prevMoveForce = moveForce;
        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
		gunBehave = GetComponent<auraGunBehavior> ();
//		lockedOn = GetComponent<LockOnScript> ();
		characterCtr = this.GetComponent<Rigidbody>();
		if (InputManager.Devices.Count == 2 || (InputManager.Devices.Count == 1 && playerNum == 0)) { //two controllers or player 1 with one controller
			controlType = ControlType.Controller;
			myController = InputManager.Devices [playerNum];

		} else {
			controlType = ControlType.Keyboard;
		}
		previousRot = transform.rotation;
		currentDashTime = maxDashTime;
		isDashing = false;

		//ANIMATORS
		//		XAttackAnim = GetComponent<Animator>();
		//		YAttackAnim = GetComponent<Animator>();
		//		BAttackAnim = GetComponent<Animator>();

	}

	public void OnShot(){
        //prevMoveForce = moveForce;
        moveForce = shootForce;
	}
    public void NotShot(){
        moveForce = prevMoveForce;
    }

	// Update is called once per frame
	void FixedUpdate () {
		if (transform.position.y >= 6f) {
			transform.position = new Vector3 (transform.position.x, 6f, transform.position.z);
		}
        //		moveDirection.y = 0;
        //myController = InputManager.Devices[playerNum];
        //if (stuckTimer <= 0) {
        if (hitStunnedTimer <= 0)
        {
            MoveCharacter();
        } else {
            hitStunnedTimer -= Time.deltaTime;
        }
		//} else {
		//	characterCtr.velocity = Vector3.zero;
		//}
		//		storeDir = cameraTrans.right;
		//		MyCharacterActions ();
		//stuckTimer -= Time.deltaTime;
		if (startButton())
        {
            TwoDGameManager.player1ScoreNum = 0;
            TwoDGameManager.player2ScoreNum = 0;
			SceneManager.LoadScene("AuraVersion");
		}
	}


	private Vector3 OnMove() {
		if (controlType == ControlType.Controller) {
			return new Vector3 (myController.LeftStickX, 0, myController.LeftStickY);
		} else {
			float leftxLeft;
			float leftxRight;
			float leftx;

			float leftyUp;
			float leftyDown;
			float lefty;

			if(Input.GetKey(KeyCode.D)){
				leftxLeft = 1;
			}else{
				leftxLeft = 0;
			}
			if (Input.GetKey (KeyCode.A)) {
				leftxRight = -1;
			} else {
				leftxRight = 0;
			}
			leftx = leftxLeft + leftxRight;

			if(Input.GetKey(KeyCode.W)){
				leftyUp = 1;
			}else{
				leftyUp = 0;
			}
			if (Input.GetKey (KeyCode.S)) {
				leftyDown = -1;
			} else {
				leftyDown = 0;
			}
			lefty = leftyDown + leftyUp;

			return new Vector3 (leftx, 0, lefty);
		}
	}
	public Vector3 RightStickMove (){
		if (controlType == ControlType.Controller) {
			return new Vector3 (myController.RightStickX, 0, myController.RightStickY);
		} else {
			
			float rightxLeft;
			float rightxRight;
			float rightx;

			float rightyUp;
			float rightyDown;
			float righty;

			if(Input.GetKey(KeyCode.J)){
				rightxLeft = 1;
			}else{
				rightxLeft = 0;
			}
			if (Input.GetKey (KeyCode.G)) {
				rightxRight = -1;
			} else {
				rightxRight = 0;
			}
			rightx = rightxLeft + rightxRight;

			if(Input.GetKey(KeyCode.Y)){
				rightyUp = 1;
			}else{
				rightyUp = 0;
			}
			if (Input.GetKey (KeyCode.H)) {
				rightyDown = -1;
			} else {
				rightyDown = 0;
			}
			righty = rightyDown + rightyUp;

			return new Vector3 (rightx, 0, righty);
		}
	}

	public bool startButton ()
	{
		if (controlType == ControlType.Controller) {
			return (myController.CommandWasPressed);
		} else {
			return(Input.GetKeyUp (KeyCode.F));
		}
	}

	public bool yButton (){
		return (myController.Action4.WasPressed);
	}
	public bool xButton (){
		return (myController.Action3.WasPressed);
	} 

	public bool xButtonUp (){
		if (controlType == ControlType.Controller) {
			return (myController.Action3.WasPressed);
		} else {
			return (Input.GetKeyDown(KeyCode.B));
		}
	} 
	public bool secondaryFire (){
		if (controlType == ControlType.Controller) {
			return (myController.LeftTrigger.IsPressed);
		} else {
			return (Input.GetKey (KeyCode.LeftShift));
		}
	}

	public bool secondaryFireDown (){
		if (controlType == ControlType.Controller) {
			return (myController.LeftTrigger.WasPressed);
		} else {
			return (Input.GetKeyDown (KeyCode.LeftShift));
		}	}
	public bool secondaryFireUp (){
		if (controlType == ControlType.Controller) {
			return (myController.LeftTrigger.WasReleased);
		} else {
			return (Input.GetKeyUp (KeyCode.LeftShift));
		}	}
	public bool primaryFire (){
		if (controlType == ControlType.Controller) {
			return (myController.RightTrigger.IsPressed);
		} else {
			return (Input.GetKey (KeyCode.V));
		}
	} 

	public bool bButtonUp (){
		if (controlType == ControlType.Controller) {
			return (myController.Action2.WasReleased);
		} else {
			return (Input.GetKeyDown (KeyCode.N));
		}
	}

	public bool onLock() {
		return myController.LeftBumper.WasPressed;
	}
	public bool Unlock()
	{
		return myController.LeftBumper.WasReleased;
	}

	//	public bool rightTriggerDown(){
	//		return myController.RightTrigger.IsPressed;
	//	}
	//
	//	public bool leftTriggerDown(){
	//		return myController.LeftTrigger.IsPressed;
	//	}

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
		if (!isDashing) {
			curForce = moveForce;
		}
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
			isDashing = false;
			//			curDrag = dashDrag;

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

		directionPos = transform.position + (RightStickMove());


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
//		if (RightStickMove().magnitude < .01f) {
//			directionPos = transform.position + (OnMove());
//			dir = directionPos - transform.position;
//			//			Debug.Log (dir + "checking secondary");
		if (RightStickMove().magnitude > .01f) {
				characterCtr.rotation = 
					Quaternion.Slerp (transform.rotation,
						Quaternion.LookRotation (dir), 
						turnSpeed * Time.deltaTime);
			
			return;
			//
		} 

		else {
//			if (!lockedOn.LockedOn) {
//				//				float angle = Mathf.Atan2 (myController.LeftStickY, myController.LeftStickX) * Mathf.Rad2Deg;
//				//				previousRot = Quaternion.Euler (new Vector3 (0, -angle + 90, 0));
//				//
//				//				characterCtr.MoveRotation(Quaternion.Euler (new Vector3 (0, -angle + 90, 0)));
//				characterCtr.rotation = 
//					Quaternion.Slerp(transform.rotation,
//						Quaternion.LookRotation(dir), 
//						turnSpeed * Time.deltaTime);
//			} else {
//			}
		}
		//		Debug.Log (isDashing+"is dashing outside of the thing");


	}

	void OnTriggerStay (Collider other)
	{
		
		GameObject otherObj = other.gameObject;
        Debug.Log(otherObj.tag);
		if (otherObj.tag == "player1Aura" || otherObj.tag == "player2Aura") {
			
				characterCtr.AddForce ((moveDirForward + moveDirSides).normalized * -slowForce);
		} else {
			return;
		}
	}

	void OnTriggerExit (Collider other)
	{
//		Debug.Log ("test3");
		curForce = moveForce;
	}

}

