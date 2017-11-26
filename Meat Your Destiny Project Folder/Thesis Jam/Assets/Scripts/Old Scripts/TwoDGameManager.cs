﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TwoDGameManager : MonoBehaviour {
    public static TwoDGameManager thisInstance;
	public auraPlayerHealth playerHealth1;
	public auraPlayerHealth playerHealth2;
    const int maxPlayers = 2;
	public auraGunBehavior[] players;
	private int restartTime = 3;
	public Text playerWinner;
    public static Text player1Score;
    public static int player1ScoreNum = 0;
    public static Text player2Score;
    public static int player2ScoreNum = 0;
    private bool addedScore = false;

    public GameObject player1Prefab, player2Prefab;
    public GameObject player1, player2;

    public Transform player1Start;
    public Transform player2Start;
//	GameObject audioManagerClone;
//	public GameObject audioManagerPrefab;

    void OnApplicationQuit()
    {
        thisInstance = null;
    }
	void Awake ()
	{
        if (thisInstance == null)
        {
            thisInstance = GameObject.Find("gameManager").GetComponent<TwoDGameManager>();
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
        player1Score = GameObject.Find("Player1Score").GetComponent<Text>();
        player2Score = GameObject.Find("Player2Score").GetComponent<Text>();
		playerWinner.text = " ";

        //		if (!GameObject.Find ("AudioManager(Clone)")) {
        //			audioManagerClone = Instantiate (audioManagerPrefab);
        //		}

        /////////GAME INSTANTIATION OCCURS HERE/////////

        player1 = Instantiate(player1Prefab, player1Start.position, Quaternion.identity);
        player2 = Instantiate(player2Prefab, player2Start.position, Quaternion.identity);

        players[0] = player1.GetComponent<auraGunBehavior>();
        players[1] = player2.GetComponent<auraGunBehavior>();
        playerHealth1 = player1.GetComponent<auraPlayerHealth>();
        playerHealth2 = player2.GetComponent<auraPlayerHealth>();
	}


	void Update ()
	{
        playerScoreUpdate();
		if (playerHealth1.CurrentHealth <= 0 || playerHealth2.CurrentHealth <= 0) {
            if (playerHealth1.CurrentHealth > 0 && addedScore == false) {
                addedScore = true;
                player1ScoreNum ++;
				playerWinner.text = "green wins";
                StartCoroutine(gameRestart());
                return;
            } 
            if (playerHealth2.CurrentHealth > 0 && addedScore == false){
                addedScore = true;
                player2ScoreNum++;
				playerWinner.text = "red wins";
                StartCoroutine(gameRestart());
                return;
			}
		}

	}

	public IEnumerator gameRestart ()
	{
		yield return new WaitForSeconds (restartTime);
		SceneManager.LoadScene("AuraVersion");
        addedScore = false;
	}

    public void playerScoreUpdate ()
    {
        player1Score.text = "green divine ascensions\n" + player1ScoreNum;
        player2Score.text = "red divine ascensions\n" + player2ScoreNum;
    }
}
