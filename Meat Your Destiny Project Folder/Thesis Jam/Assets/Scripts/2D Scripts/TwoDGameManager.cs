using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class TwoDGameManager : MonoBehaviour {
//	public PlayerHealth playerHealth;
    const int maxPlayers = 2;
    public TwoDGunBehaviorBigClip[] players;


void Update ()
	{
		if (Input.GetKeyDown ("space")) {
//			Debug.Log ("test");
			SceneManager.LoadScene("2DBigClipTest");
		}
}

}
