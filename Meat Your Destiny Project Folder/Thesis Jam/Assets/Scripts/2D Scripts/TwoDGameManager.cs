using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TwoDGameManager : MonoBehaviour {
	public auraPlayerHealth playerHealth1;
	public auraPlayerHealth playerHealth2;
    const int maxPlayers = 2;
	public auraGunBehavior[] players;
	private int restartTime = 3;
	public Text playerWinner;
//	GameObject audioManagerClone;
//	public GameObject audioManagerPrefab;

	void Awake ()
	{
		playerWinner.text = " ";

//		if (!GameObject.Find ("AudioManager(Clone)")) {
//			audioManagerClone = Instantiate (audioManagerPrefab);
//		}
	}
	void Update ()
	{
		if (playerHealth1.CurrentHealth <= 0 || playerHealth2.CurrentHealth <= 0) {
			StartCoroutine (gameRestart());
			if (playerHealth1.CurrentHealth > 0) {
				playerWinner.text = "player one wins";
			} else {
				playerWinner.text = "player two wins";
			}
		}

	}

	public IEnumerator gameRestart ()
	{
		yield return new WaitForSeconds (restartTime);
		SceneManager.LoadScene("AuraVersion");
	}
}
