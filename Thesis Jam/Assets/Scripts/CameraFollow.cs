﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public TwoDCharacterController myCont;
	public float CameraMoveSpeed = 120.0f;
	public GameObject CameraFollowObj;
	Vector3 FollowPOS;
	public float clampAngle = 80.0f;
	public float inputSensitivity = 150.0f;
	public GameObject CameraObj;
	public GameObject PlayerObj;
	public float smoothX;
	public float smoothY;
	private float rotY = 0.0f;
	private float rotX = 0.0f;
	// Use this for initialization
	void Start () {
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;
	}

	// Update is called once per frame
	void Update () {
		//Here we set up the rotation of the sticks

//		float inputX = myCont.CameraMove ().x;
//		float inputZ = myCont.CameraMove ().z;

//		rotY += inputX * inputSensitivity * Time.deltaTime;
//		rotX += inputZ * inputSensitivity * Time.deltaTime;

//		rotX = Mathf.Clamp (rotX, -clampAngle, clampAngle);
//
//		Quaternion localRotation = Quaternion.Euler (rotX, rotY, 0.0f);
//		transform.rotation = localRotation;

	}

	void LateUpdate (){
		CameraUpdater ();
	}

	void CameraUpdater (){
		//set target to follow
		Transform target = CameraFollowObj.transform;

		//move towards the game object that is the target
		float step = CameraMoveSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, target.position, step);

	}
}
