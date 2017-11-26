using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITracker : MonoBehaviour {
	public Transform Player;
	public float UIOffset;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = new Vector3 (Player.position.x, Player.position.y, Player.position.z - UIOffset);
		
	}
}
