using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TwoDGameManager : MonoBehaviour {
	public PlayerHealth playerHealth1;
	public PlayerHealth playerHealth2;
    const int maxPlayers = 2;
    public TwoDGunBehaviorBigClip[] players;
	private int restartTime = 3;
	public Text playerWinner;


	void Start ()
	{
		playerWinner.text = " ";
	}
	void Update ()
	{
		if (playerHealth1.CurrentHealth <= 0 || playerHealth2.CurrentHealth <= 0) {
			StartCoroutine (gameRestart());
			if (playerHealth1.CurrentHealth > 0) {
				playerWinner.text = "player   one  wins    congratulations  citizen";
			} else {
				playerWinner.text = "player   two  wins    congratulations  citizen";
			}
		}

	}

	public IEnumerator gameRestart ()
	{
		yield return new WaitForSeconds (restartTime);
		SceneManager.LoadScene("2DBigClipTest");
	}
}
