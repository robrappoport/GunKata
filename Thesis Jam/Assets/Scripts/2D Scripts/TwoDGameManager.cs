﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class TwoDGameManager : MonoBehaviour {
    const int maxPlayers = 2;
    public TwoDGunBehavior[] players;


void Update ()
	{
		if (Input.GetKeyDown ("space")) {
			Debug.Log ("test");
			SceneManager.LoadScene("2DTest");
		}
}

}
