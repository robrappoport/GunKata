using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class TwoDCharacterController : MonoBehaviour {
	public static TwoDCharacterController instance;
    public InputDevice myController;
	public TwoDGunBehavior gunBehave;
	LockOnScript lockedOn;
    //public InputDevice myController { get; set; }
    public Vector3 moveDirection;
	public int playerNum;
	public float walkSpeed = 2;
	public float maxDashTime = 1.0f;
	public float dashSpeed = 4.0f;
	public float dashStopSpeed = 0.1f;

	public float currentDashTime;
	private float currentSpeed = 0;
	public bool isDashing;
	private Quaternion previousRot;
	private CharacterController characterCtr;


    public PlayerAction Move;
    public PlayerAction Shoot;

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
		characterCtr = this.GetComponent<CharacterController>();
		myController = InputManager.Devices[playerNum];
		previousRot = transform.rotation;
		currentDashTime = maxDashTime;
		isDashing = false;

		//ANIMATORS
//		XAttackAnim = GetComponent<Animator>();
//		YAttackAnim = GetComponent<Animator>();
//		BAttackAnim = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y != .38f) {
			transform.position = new Vector3 (transform.position.x, .38f, transform.position.z);
		}
		moveDirection.y = 0;
		myController = InputManager.Devices[playerNum];
		MoveCharacter ();
//		MyCharacterActions ();

	}

	private Vector3 OnMove() {
		return new Vector3(myController.LeftStickX, 0, myController.LeftStickY);
        

	}

	public bool yButton (){
		return (myController.Action4.IsPressed);
	} 
	public bool yButtonUp (){
		return (myController.Action4.WasReleased);
	}
	public bool xButtonUp (){
		return (myController.Action3.WasReleased);
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
		currentSpeed = walkSpeed;
//		Debug.Log (currentSpeed);
			moveDirection = OnMove();

			moveDirection.y = 0;

		if (bButtonUp () && gunBehave.CurrentBullets > 0) {
			gunBehave.CurrentBullets--;
			currentDashTime = 0.0f;
		}

		if (currentDashTime < maxDashTime) {
			isDashing = true;
			Debug.Log (isDashing + "Isdashing in the thing");
			moveDirection = new Vector3 (moveDirection.x * dashSpeed, 0, moveDirection.z * dashSpeed);
			currentDashTime += dashStopSpeed;

		} else {
			isDashing = false;
		}

			moveDirection *= currentSpeed;
		characterCtr.Move(moveDirection * Time.deltaTime);


//		 else {
//			moveDirection = Vector3.zero;
//		}
//		RotateCharacter(moveDirection);
		if (OnMove ().magnitude < .01f) {
			transform.rotation = previousRot;
		} 

		else {
			if (!lockedOn.LockedOn) {
				float angle = Mathf.Atan2 (myController.LeftStickY, myController.LeftStickX) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (new Vector3 (0, -angle + 90, 0));
			} else {
			}
		}
		previousRot = transform.rotation;
		Debug.Log (isDashing+"is dashing outside of the thing");


		}
    public void MyCharacterActions()
    {
//		yButton ();
//		yButtonUp ();
    }



//
//	private void buttonDowns(){
//		if (myController.Action1.WasPressed) {
//			XAttack = true;
//		}
//		if (myController.Action2.WasPressed) {
//			YAttack = true;
//		}
//		if (myController.Action3.WasPressed) {
//			BAttack = true;
//		}
//		if (myController.Action4.WasPressed) {
//			Dodging = true;
//		}
//	}
}

