using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBetweener : MonoBehaviour {

	public GameObject object1;
	public GameObject object2;
	public Camera zoomCam;
	private Vector3 player1pos;
	private Vector3 player2pos;

	void Start ()
	{
		player1pos = object1.transform.position;
		player2pos = object2.transform.position;
	}
	void Update()
	{
		this.transform.position = 0.5f*(player1pos + player2pos);
	}
}
