using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class TwoDCharacterController : MonoBehaviour {
	public static TwoDCharacterController instance;
    public InputDevice myController;
    //public InputDevice myController { get; set; }
    public Vector3 moveDirection;
	public int playerNum;
	public float walkSpeed = 2;
	private float currentSpeed = 0;
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

		characterCtr = this.GetComponent<CharacterController>();
		previousRot = transform.rotation;

		//ANIMATORS
//		XAttackAnim = GetComponent<Animator>();
//		YAttackAnim = GetComponent<Animator>();
//		BAttackAnim = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		moveDirection.y = 0;
		myController = InputManager.Devices[playerNum];
		MoveCharacter ();
		MyCharacterActions ();

	}

	private Vector3 OnMove() {
		return new Vector3(myController.LeftStickX, 0, myController.LeftStickY);
        

	}

	public bool yButton (){
		return (myController.Action4.IsPressed);
		//triple burst attack goes here

	} 
	public bool yButtonUp (){
		return (myController.Action4.WasReleased);
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
		currentSpeed = walkSpeed;
//		Debug.Log (currentSpeed);

			moveDirection = OnMove();

//			moveDirection.y = 0;

			moveDirection *= currentSpeed;
		characterCtr.Move(moveDirection * Time.deltaTime);
//		RotateCharacter(moveDirection);
		if (OnMove ().magnitude < .01f) {
			transform.rotation = previousRot;
		} 

		else {
			float angle = Mathf.Atan2 (myController.LeftStickY, myController.LeftStickX) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (new Vector3 (0, -angle + 180, 0));
		}
		previousRot = transform.rotation;

		}
    public void MyCharacterActions()
    {
		yButton ();
		yButtonUp ();
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

