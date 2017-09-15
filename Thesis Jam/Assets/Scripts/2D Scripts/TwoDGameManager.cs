using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDGameManager : MonoBehaviour {
	public TwoDGunBehavior[] players;
	// Use this for initialization
	void Start () {
		players [0].playerNum = 1;
		players [1].playerNum = 2;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
