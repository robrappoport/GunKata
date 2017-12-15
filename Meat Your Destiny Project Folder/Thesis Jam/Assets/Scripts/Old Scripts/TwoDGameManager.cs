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
    public GameObject respawnBulletDestroyer;
    private int restartTime = 1;
	public Text playerWinner;
    public static Image player1Score;
    public static float player1ScoreNum = 0;
    public static Image player2Score;
    public static float player2ScoreNum = 0;
    public float lerpTime;
    private bool addedScore = false;
    private float displayedPlayer1Score;
    private float displayedPlayer2Score;
    public float maxScore;

    public GameObject player1Prefab, player2Prefab;
    public GameObject player1, player2;

    public Transform player1Start;
    public Transform player2Start;

    public UITracker player1Tracker;
    public UITracker player2Tracker;

    public GameObject player1Canvas, player2Canvas;

    public GameObject textPrefab;

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

        player1Score = GameObject.Find("Player1ScoreFill").GetComponent<Image>();
        player2Score = GameObject.Find("Player2ScoreFill").GetComponent<Image>();
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
                //player2Canvas.SetActive(false);
                player1ScoreNum += 2f;
                Debug.Log("Adding to player1 score");
//                playerWinner.text = "green wins";
                //StartCoroutine(gameRestart());
                StartCoroutine(DelayedSpawnPlayer2());
                return;
            } 
            if (playerHealth2.CurrentHealth > 0 && addedScore == false){
				//player 2 wins
                addedScore = true;
                //player1Canvas.SetActive(false);
                player2ScoreNum += 2f;
                Debug.Log("Adding to player2 score");

//				playerWinner.text = "red wins";
               //StartCoroutine(gameRestart());
                StartCoroutine(DelayedSpawnPlayer1());
                return;
			}
		}
        playerWin();



	}

	public IEnumerator gameRestart ()
	{
		yield return new WaitForSeconds (restartTime);
        resetScore();
		SceneManager.LoadScene("AuraVersion");
        addedScore = false;
	}

    public void playerScoreUpdate ()
    {
        //Debug.Log("player 1 score: " + player1ScoreNum + " player2 score: " + player2ScoreNum);
        //Debug.Log(displayedPlayer2Score);
        player1ScoreNum = Mathf.Clamp(player1ScoreNum, 0, maxScore);
        player2ScoreNum = Mathf.Clamp(player2ScoreNum, 0, maxScore);
        displayedPlayer1Score = Mathf.Lerp(displayedPlayer1Score, player1ScoreNum / maxScore, Time.deltaTime * lerpTime);
        displayedPlayer2Score = Mathf.Lerp(displayedPlayer2Score, player2ScoreNum / maxScore, Time.deltaTime * lerpTime);
        player1Score.fillAmount = (displayedPlayer1Score);
        player2Score.fillAmount = (displayedPlayer2Score);
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
        Debug.Log("Spawning player 1");
		player1 = Instantiate(player1Prefab, player1Start.position, Quaternion.identity) as GameObject;
		playerHealth1 = player1.GetComponent<auraPlayerHealth>();
		players[0] = player1.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan1 = player1.GetComponent<BulletManager>();
        player1.GetComponent<auraGunBehavior>().curStamina = player1.GetComponent<auraGunBehavior>().staminaTotal;
        Debug.Log("player 1's stamina is" + player1.GetComponent<auraGunBehavior>().curStamina);
		player1.GetComponent<auraGunBehavior>().staminaBar = player1Canvas.transform.Find("GameObject/AuraBar").GetComponent<Image>();
        player1.GetComponent<auraGunBehavior>().staminaBar.fillAmount = 1;
		player1Tracker.Player = player1.transform;

		addedScore = false;


	}

    void SpawnPlayer2(){
		if (player2) {
			Destroy (player2);
		}
        Debug.Log("Spawning player 2");

		player2 = Instantiate(player2Prefab, player2Start.position, Quaternion.identity) as GameObject;
		playerHealth2 = player2.GetComponent<auraPlayerHealth>();
		players[1] = player2.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan2 = player2.GetComponent<BulletManager>();
        player2.GetComponent<auraGunBehavior>().curStamina = player2.GetComponent<auraGunBehavior>().staminaTotal;
        Debug.Log("player 2's stamina is"+ player2.GetComponent<auraGunBehavior>().curStamina);
		player2.GetComponent<auraGunBehavior>().staminaBar = player2Canvas.transform.Find("GameObject/AuraBar").GetComponent<Image>();
        player2.GetComponent<auraGunBehavior>().staminaBar.fillAmount = 1;
		player2Tracker.Player = player2.transform;

		addedScore = false;

	}

    IEnumerator DelayedSpawnPlayer1 ()
    {
        yield return new WaitForSeconds(respawnTime);
        Instantiate(respawnBulletDestroyer, player1Start.position, Quaternion.identity);
        yield return new WaitForSeconds(.1f);
        SpawnPlayer1();
        
    }
    IEnumerator DelayedSpawnPlayer2()
    {
        yield return new WaitForSeconds(respawnTime);
        Instantiate(respawnBulletDestroyer, player2Start.position, Quaternion.identity);
        yield return new WaitForSeconds(.1f);
        SpawnPlayer2();
    }

    void playerWin()
    {
        if (player1ScoreNum >= maxScore)
        {
            player1.SetActive(false);
            player2.SetActive(false);
            player2Canvas.SetActive(false);
            player1Canvas.SetActive(false);
            playerWinner.text = "blue wins";
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
    void resetScore ()
    {
        player1ScoreNum = 0f;
        player2ScoreNum = 0f;
    }
}
