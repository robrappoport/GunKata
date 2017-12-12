using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TwoDGameManager : MonoBehaviour {
    public static TwoDGameManager thisInstance;

	public float respawnTime = 1;
	public auraPlayerHealth playerHealth1;
	public auraPlayerHealth playerHealth2;
    const int maxPlayers = 2;
	public auraGunBehavior[] players;
    private int restartTime = 1;
	public Text playerWinner;
    public static Text player1Score;
    public static int player1ScoreNum = 0;
    public static Text player2Score;
    public static int player2ScoreNum = 0;
    private bool addedScore = false;
    public int maxScore;

    public GameObject player1Prefab, player2Prefab;
    public GameObject player1, player2;

    public Transform player1Start;
    public Transform player2Start;

    public UITracker player1Tracker;
    public UITracker player2Tracker;

    public GameObject player1Canvas, player2Canvas;

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
        setLevel();


	}


	void Update ()
	{
        playerScoreUpdate();
		if (playerHealth1.CurrentHealth <= 0 || playerHealth2.CurrentHealth <= 0) {
            if (playerHealth1.CurrentHealth > 0 && addedScore == false) {
				//player 1 wins
                addedScore = true;
                player2Canvas.SetActive(false);
                player1ScoreNum += 2;
//                playerWinner.text = "green wins";
                //StartCoroutine(gameRestart());
				Invoke("SpawnPlayer2", respawnTime);
                return;
            } 
            if (playerHealth2.CurrentHealth > 0 && addedScore == false){
				//player 2 wins
                addedScore = true;
                player1Canvas.SetActive(false);
                player2ScoreNum += 2;
//				playerWinner.text = "red wins";
               //StartCoroutine(gameRestart());
				Invoke("SpawnPlayer1", respawnTime);
                return;
			}
		}
        playerWin();



	}

	public IEnumerator gameRestart ()
	{
		yield return new WaitForSeconds (restartTime);
        player1ScoreNum = 0;
        player2ScoreNum = 0;
		SceneManager.LoadScene("AuraVersion");
        addedScore = false;
	}

    public void playerScoreUpdate ()
    {
        player1Score.text = "green divine ascensions\n" + player1ScoreNum;
        player2Score.text = "red divine ascensions\n" + player2ScoreNum;
    }
    void setLevel ()
    {
		SpawnPlayer1 ();
		SpawnPlayer2 ();
    }

	void SpawnPlayer1(){
		if (player1) {
			Destroy (player1);
		}
		player1 = Instantiate(player1Prefab, player1Start.position, Quaternion.identity) as GameObject;
		playerHealth1 = player1.GetComponent<auraPlayerHealth>();
		players[0] = player1.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan1 = player1.GetComponent<BulletManager>();
		player1.GetComponent<auraGunBehavior>().staminaBar = player1Canvas.transform.Find("GameObject/AuraBar").GetComponent<Image>();
		player1Tracker.Player = player1.transform;
		addedScore = false;


	}

	void SpawnPlayer2(){
		if (player2) {
			Destroy (player2);
		}
		player2 = Instantiate(player2Prefab, player2Start.position, Quaternion.identity) as GameObject;
		playerHealth2 = player2.GetComponent<auraPlayerHealth>();
		players[1] = player2.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan2 = player2.GetComponent<BulletManager>();
		player2.GetComponent<auraGunBehavior>().staminaBar = player2Canvas.transform.Find("GameObject/AuraBar").GetComponent<Image>();
		player2Tracker.Player = player2.transform;
		addedScore = false;

	}

    void playerWin()
    {
        if (player1ScoreNum >= maxScore)
        {
            player1.SetActive(false);
            player2.SetActive(false);
            player2Canvas.SetActive(false);
            player1Canvas.SetActive(false);
            playerWinner.text = "green wins";
            StartCoroutine(gameRestart());
        }
        if (player2ScoreNum >= maxScore)
        {
            player1.SetActive(false);
            player2.SetActive(false);
            player2Canvas.SetActive(false);
            player1Canvas.SetActive(false);
            playerWinner.text = "red wins";
            StartCoroutine(gameRestart());

        }
    }
}
