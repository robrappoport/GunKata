using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(CharacterController))]

public class SimpleController : MonoBehaviour {
    [SerializeField]
    private float walkSpeed = 2;
    [SerializeField]
    //private float jumpSpeed = 8;

    public Transform rotationPivot;
    public CameraDynamicOrbit cameraPivot;

    [SerializeField]
    private Camera playerCamera;

    [HideInInspector]

    private float gravity = 20;
    private float rotationSpeed = 15;
    private float aimRotationSpeed = 40;
    private float currentSpeed = 0;

    private CharacterController characterCtr;

    private Vector3 moveDirection;
    private Vector3 rotationDirection;

    private void Awake() {
        characterCtr = this.GetComponent<CharacterController>();
        moveDirection = Vector3.zero;
    }

    // Use this for initialization
    void Start() {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        MoveCharacter();
//        Shooting();
        Meleeing();
    }

    private void MoveCharacter() {
        currentSpeed = walkSpeed;
        if (characterCtr.isGrounded) {

            moveDirection = ControllerManager.instance.OnMove();

            moveDirection = playerCamera.transform.TransformDirection(moveDirection);
            moveDirection.y = 0;
            moveDirection.Normalize();

            moveDirection *= currentSpeed;

            if (ControllerManager.instance.OnDodge() == true) {
                Debug.Log("Dodging!");
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        characterCtr.Move(moveDirection * Time.deltaTime);
        RotateCharacter(moveDirection);
    }

    private void RotateCharacter(Vector3 _direction) {
        //No Aiming, Player will facing to it's movement direction;
        if (Mathf.Abs(_direction.x) > 0.1f || Mathf.Abs(_direction.z) > 0.1f) {
            rotationDirection = playerCamera.transform.TransformDirection(new Vector3(_direction.x, 0, _direction.z));
            rotationDirection.y = 0;
            rotationDirection.Normalize();
            rotationDirection *= Time.deltaTime;

            rotationDirection *= (Mathf.Abs(_direction.x) > Mathf.Abs(_direction.z)) ? Mathf.Abs(_direction.x) : Mathf.Abs(_direction.z);

            if (ControllerManager.instance.onLock() == false) {
               rotationPivot.rotation = Quaternion.Slerp(rotationPivot.rotation, Quaternion.Euler(new Vector3(0, (Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg), 0)), rotationSpeed * Time.deltaTime);
            }

            if (ControllerManager.instance.onLock() == true) {
				Debug.Log ("hey dude");
                rotationPivot.rotation = Quaternion.Slerp(rotationPivot.rotation, Quaternion.Euler(new Vector3(0, cameraPivot.transform.localEulerAngles.y + this.transform.eulerAngles.y, 0)), aimRotationSpeed * Time.deltaTime);
            }
        }
    }

//    private void Shooting()
//    {
//
//        if (ControllerManager.instance.onShoot() == true)
//        {
//            Debug.Log("Shoot the gun!");
//
//        }
//    }

     private void Meleeing()
    {

        if (ControllerManager.instance.onMelee() == true)
        {
            Debug.Log("Punch the things!");
        }
    }

   
}
