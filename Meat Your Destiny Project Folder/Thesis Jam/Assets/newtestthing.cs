using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class newtestthing : MonoBehaviour {

	public GameObject[] markerList; 
	public AudioSource [] thisAudio;
	public bool ghostIsHere = false;


	void Start ()
	{
		//get audio component
		ghostIsHere = false;
	}

	void Update ()
	{
		
	}
}

