using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class AuraCharacterController : PlayControl
{
    public static PlayControl instance;
    public InputDevice myController;

    //	LockOnScript lockedOn;
    //public InputDevice myController { get; set; }
    //    public Vector3 moveDirection;
    public Vector3 moveDirForward;
    public Vector3 moveDirSides;
    public float heightValue;
    //	private Vector3 storeDir;
    private Vector3 directionPos;
    //	public Transform cameraTrans;
    public int playerNum;
    public float turnSpeed = 4f;
    //	public float walkSpeed = 2;
    public float maxDashTime = 1.0f;
    public float dashSpeed = 4.0f;
    public float dashStopSpeed = 0.1f;
    private float initDistance;
    public float currentDashTime;
    //	private float currentSpeed = 0;
    public bool isDashing;
    private Quaternion previousRot;
    private Rigidbody characterCtr;
    private Animator anim;
	Collider auraCol;
	List<Collider> cols = new List<Collider> ();

	[Header("MOVEMENT VARS")]
	bool slow = false;
	public float currentForce;
    public float moveForce;
    public float dashForce;
    public float slowForce;
    public float shootForce;
    public float prevMoveForce;
    float stuckTimer;
    public float stuckTime = .1f;
    public enum ControlType { Keyboard, Controller, NONE };/// <summary>
    public float forceMultiplier;
    /// Keyboard controls are as follows: WASD to move, YGHJ to rotate, V to shoot, left shift to project aura.
    /// </summary>
    public ControlType controlType;

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


    void Awake()
    {

        instance = this;
    }
    void Start()
    {
		moveForce = prevMoveForce;

        anim = GetComponent<Animator>();
        //prevMoveForce = moveForce;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //heightValue = transform.position.y;
        gunBehave = GetComponent<auraGunBehavior>();
        //		lockedOn = GetComponent<LockOnScript> ();
        characterCtr = this.GetComponent<Rigidbody>();
        if (InputManager.Devices.Count == 2 || (InputManager.Devices.Count == 1 && playerNum == 0))
        { //two controllers or player 1 with one controller
            controlType = ControlType.Controller;
            myController = InputManager.Devices[playerNum];

        }
        else
        {
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

    public void shootSlowDown()
    {
        //prevMoveForce = moveForce;
        moveForce = shootForce;
    }
    public void NotShot()
    {
        moveForce = prevMoveForce;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hitStunnedTimer <= 0)
        {
            MoveCharacter();
        }
        else
        {
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
            SceneManager.LoadScene("LevelSelectScreen");
        }
    }
    private void Update()
    {

		AuraCheck ();
		anim.SetFloat("Velocity X", transform.InverseTransformDirection(OnMove()).x);
		anim.SetFloat("Velocity Z", transform.InverseTransformDirection(OnMove()).z);
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
		if (controlType == ControlType.Controller) {
			return (myController.Action4.WasPressed);
		} else {
			return (Input.GetKeyDown(KeyCode.B));
		}

	}
	public bool xButton (){
		if (myController != null) {
			return (myController.Action3.WasPressed);
		} else {
			return false;
		}

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
			return (Input.GetKey (KeyCode.X));
		}
	}

	public bool secondaryFireDown (){
		if (controlType == ControlType.Controller) {
			return (myController.LeftTrigger.WasPressed);
		} else {
			//print ("spawning aura");
			return (Input.GetKeyDown (KeyCode.X));

		}	
	}
	public bool secondaryFireUp (){
		if (controlType == ControlType.Controller) {
            return (myController.LeftTrigger.WasReleased);
		} else {
			return (Input.GetKeyUp (KeyCode.X));
		}	
	}
	
	public bool primaryFire (){
		if (controlType == ControlType.Controller) {
			return (myController.RightTrigger.IsPressed);
		} else {
            return (Input.GetKey(KeyCode.V));
		}
	} 
    public bool primaryFireUp (){
        if (controlType == ControlType.Controller) {
            return (myController.RightTrigger.WasReleased);
        } else {
            return (Input.GetKeyUp (KeyCode.V));
        }
    } 
    public bool primaryFireDown (){
        if (controlType == ControlType.Controller) {
            return (myController.RightTrigger.WasPressed);
        } else {
			return (Input.GetKeyDown (KeyCode.V));
        }
    } 

	public bool bButtonUp (){
		if (controlType == ControlType.Controller) {
			return (myController.Action2.WasReleased);
		} else {
			return (Input.GetKeyUp (KeyCode.N));
		}
	}

	public bool bButtonDown (){
		if (controlType == ControlType.Controller) {
			return (myController.Action2.WasPressed);
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
	
		//		float curDrag = dragForce;
		//		Debug.Log (currentSpeed);
		//		moveDirection = OnMove();
		moveDirForward = new Vector3 (OnMove ().x, 0, 0);
		//		moveDirForward = storeDir * OnMove ().x;
		moveDirSides = new Vector3 (0,0,OnMove ().z);//cameraTrans.forward

		//		moveDirection.y = 0;

		if (bButtonDown () && gunBehave.remainingAuraCharge > 0) {
			gunBehave.DrainAura (1);
			gunBehave.Invoke ("ResetAuraCooldown", gunBehave.coolDownDuration);
			gunBehave.CurrentBullets--;
			currentDashTime = 0.0f;
			isDashing = true;
		}

		if (currentDashTime < maxDashTime) {
			//isDashing = true;
			//Debug.Log (isDashing + "Isdashing in the thing");
			//moveDirection = new Vector3 (moveDirection.x * dashSpeed, 0, moveDirection.z * dashSpeed);
			currentDashTime += dashStopSpeed;
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


		characterCtr.AddForce((moveDirForward + moveDirSides).normalized * curForce()/ Time.deltaTime);

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

	float curForce(){
		//TODO redo in terms of multipliers instead of mutual exclusive branches
		float dashBuff =1, slowDebuff = 1, ledgeDebuff = 1;
		if (GetComponent<auraPlayerHealth> ().steppedOffLedge) {
			ledgeDebuff = slowForce / moveForce;
			if (slow) {
				ledgeDebuff = 0.5f;
			}
		}
		if (slow) {
			slowDebuff = slowForce / moveForce;
		}
		if (isDashing && currentDashTime < maxDashTime) {
			dashBuff = dashForce / prevMoveForce;
			slowDebuff = 1;
			ledgeDebuff = 1;
		}

//		if (GetComponent<auraPlayerHealth>().steppedOffLedge) {
//			if (isDashing && currentDashTime < maxDashTime) {
//				return dashForce;
//			}
//			else if (slow) {
//				return slowForce / 2;
//			} else {
//				return slowForce;
//			}
//		}
//		if (slow) {
//			return slowForce;
//		} else if (isDashing && currentDashTime < maxDashTime) {
//			print ("dashing");
//			return dashForce;
//		} else {
//			return moveForce;
//		}
	
		return moveForce * slowDebuff * ledgeDebuff * dashBuff;
	}

	public bool playerInteractingWithOwnAura(AuraGenerator testAura){//use this to exclude auras that don't interact with their owner
		if (testAura.auraPlayerNum == playerNum) {
			return true;
		} else {
			return false;
		}
	}
//	void OnTriggerStay (Collider other)
//	{
//		
//		GameObject otherObj = other.gameObject;
//        //Debug.Log(otherObj.tag);
//		if (otherObj.tag == "PlayerAura"){
//			
//            switch (otherObj.gameObject.GetComponent<AuraGenerator>().auraType)
//            {
//			case AuraGenerator.AuraType.slowdown:
//				slow = true;
//                break;
//			case AuraGenerator.AuraType.projection:
//				if (!playerInteractingWithOwnAura(otherObj.gameObject.GetComponent<AuraGenerator>())) {//excludes this aura from interacting with its owner
//					AuraProject (otherObj.transform);
//				}
//                    break;
//            }
//				
//		} else {
//			slow = false;
//			return;
//		}
//	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAura")
        {
            initDistance = Vector3.Distance(transform.position, other.gameObject.transform.position);
			cols.Add (other);
        }
    }
//    void OnTriggerExit (Collider other)
//	{
////		Debug.Log ("test3");
//		slow = false;
//
//	}


    void AuraProject(Transform t1)
    {
        Transform auraCenter = t1;
        float distanceBtwn = Vector3.Distance(transform.position, auraCenter.position);
        float distancePercent = 1f - (distanceBtwn / initDistance);
		float projectForce = Mathf.Abs(forceMultiplier * distancePercent);
        Vector3 auraVector = transform.position - auraCenter.position;
		characterCtr.AddForce(auraVector.normalized * projectForce, ForceMode.Impulse);
    }

	Collider GetCurrentAura(){
		//remove any missing refs from the list; aura no longer exists
		List<Collider> tempList = new List<Collider>();
		foreach (Collider c in cols) {
			if (c) {
				tempList.Add (c);
			}
		}
		cols = tempList;
		//if there is only one left in the list, it becomes the aura by default
		switch (cols.Count) {
		case 1:
			return cols [0];
		case 0:
			return null;
		default:
			Collider finalCol = cols [0];
			for (int i = 0; i < cols.Count; i++) {
				if (cols [i].GetComponent<AuraGenerator> ().auraScaleCurrent >= finalCol.GetComponent<AuraGenerator>().auraScaleCurrent) {
					finalCol = cols [i];
				}
			}
			return finalCol;
		}
	}

	void AuraCheck(){
		auraCol = GetCurrentAura ();
		if (auraCol) {
			if (auraCol.bounds.Contains (transform.position)) {
				switch (auraCol.GetComponent<AuraGenerator> ().auraType) {
				case AuraGenerator.AuraType.slowdown:
					slow = true;
					break;
				case AuraGenerator.AuraType.projection:
					if (!playerInteractingWithOwnAura (auraCol.GetComponent<AuraGenerator> ())) {//excludes this aura from interacting with its owner
						AuraProject (auraCol.transform);
					}
					break;
				}

			} else {
				slow = false;

			}
		} else {
			slow = false;
		}
	}

}

