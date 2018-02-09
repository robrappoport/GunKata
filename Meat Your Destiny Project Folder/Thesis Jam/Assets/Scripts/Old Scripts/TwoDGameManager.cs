using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TwoDGameManager : MonoBehaviour {
    public static TwoDGameManager thisInstance;
    public ZoneScript[] zones;
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
    private bool addedScore1 = false;
    private bool addedScore2 = false;
    private float displayedPlayer1Score;
    private float displayedPlayer2Score;
    public float maxScore;
    private float maxScale = 5;

    public GameObject player1Prefab, player2Prefab;
    public GameObject player1, player2;
    public Vector3 player1Scale;
    public Vector3 player2Scale;

    public Transform player1Start;
    public Transform player2Start;

    public Vector3[] player1Spawns;
    public Vector3[] player2Spawns;

    private int index1 = 0;
    private int index2 = 0;

    public UITracker player1Tracker;
    public UITracker player2Tracker;

    public GameObject player1Canvas, player2Canvas;

    public GameObject textPrefab;

	public int iFrameNumber = 10;
	public float iFrameFlashDuration = .1f;

//	GameObject audioManagerClone;
//	public GameObject audioManagerPrefab;

    void OnApplicationQuit()
    {
        thisInstance = null;
    }
	void Awake ()
	{
        StartCoroutine(TimerCo());
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

    private void Start()
    {
        player1Scale = player1.transform.localScale;
        player2Scale = player2.transform.localScale;
    }


    void Update ()
	{
        player1Start.position = player1Spawns[index1];
        player2Start.position = player2Spawns[index2];
        playerScoreUpdate();
		if (playerHealth1.CurrentHealth <= 0 || playerHealth2.CurrentHealth <= 0) {
//            if (playerHealth1.CurrentHealth > 0 && addedScore == false) {
//				//player 1 wins
//                addedScore = true;
//                //player2Canvas.SetActive(false);
//                player1ScoreNum += 2f;
//                Debug.Log("Adding to player1 score");
////                playerWinner.text = "green wins";
//                //StartCoroutine(gameRestart());
//                return;
//            } 
//            if (playerHealth2.CurrentHealth > 0 && addedScore == false){
//				//player 2 wins
//                addedScore = true;
//                //player1Canvas.SetActive(false);
//                player2ScoreNum += 2f;
//                Debug.Log("Adding to player2 score");

////				playerWinner.text = "red wins";
   //            //StartCoroutine(gameRestart());
   //             return;
			//}
            if (playerHealth1.CurrentHealth <= 0 && addedScore2 == false)
            {
                //player1Scale = player1.transform.localScale;
                StartCoroutine(DelayedSpawnPlayer1());
                addedScore2 = true;
                player2ScoreNum += 10f;
                return;
            }
            if (playerHealth2.CurrentHealth <= 0 && addedScore1 == false)
            {
                //player2Scale = player2.transform.localScale;
                StartCoroutine(DelayedSpawnPlayer2());
                addedScore1 = true;
                player1ScoreNum += 10f;
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
        addedScore1 = false;
        addedScore2 = false;
	}

    public void playerScoreUpdate ()
    {
        //Debug.Log("player 1 score: " + player1ScoreNum + " player2 score: " + player2ScoreNum);
        //Debug.Log(displayedPlayer2Score);

        player1ScoreNum = Mathf.Clamp(player1ScoreNum, 0, maxScore);
        player2ScoreNum = Mathf.Clamp(player2ScoreNum, 0, maxScore);
        float scale1 = remapRange(player1ScoreNum, 0, maxScore, 1, maxScale);
        float scale2 = remapRange(player2ScoreNum, 0, maxScore, 1, maxScale);
        displayedPlayer1Score = Mathf.Lerp(displayedPlayer1Score, player1ScoreNum / maxScore, Time.deltaTime * lerpTime);
        displayedPlayer2Score = Mathf.Lerp(displayedPlayer2Score, player2ScoreNum / maxScore, Time.deltaTime * lerpTime);
        scale1 = Mathf.Clamp(scale1, 1, maxScale);
        scale2 = Mathf.Clamp(scale2, 1, maxScale);
        player1Score.fillAmount = (displayedPlayer1Score);
        player2Score.fillAmount = (displayedPlayer2Score);
        Vector3 newScale1 = new Vector3(scale1, scale1, scale1);
        Vector3 newScale2 = new Vector3(scale2, scale2, scale2);
        //player1.transform.localScale = Vector3.Lerp((player1.transform.localScale), (newScale1), Time.deltaTime * lerpTime);
        //player2.transform.localScale = Vector3.Lerp((player2.transform.localScale), (newScale2), Time.deltaTime * lerpTime);




        //player1.transform.localScale = new Vector3((scale),
        //                                           (scale),
        //                                           (scale));
        //player2.transform.localScale = new Vector3(player2.transform.localScale.x + (displayedPlayer2Score * .01f),
                                                   //player2.transform.localScale.y + (displayedPlayer2Score * .01f),
                                                   //player2.transform.localScale.z + (displayedPlayer2Score * .01f));
   

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
        player1 = Instantiate(player1Prefab, new Vector3 (player1Spawns[index1].x, player1Spawns[index1].y + 5, player1Spawns[index1].z), Quaternion.identity) as GameObject;
        //player1.transform.localScale = player1Scale;
		playerHealth1 = player1.GetComponent<auraPlayerHealth>();
		players[0] = player1.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan1 = player1.GetComponent<BulletManager>();
        player1.GetComponent<auraGunBehavior>().curStamina = player1.GetComponent<auraGunBehavior>().staminaTotal;
        //Debug.Log("player 1's stamina is" + player1.GetComponent<auraGunBehavior>().curStamina);
		player1.GetComponent<auraGunBehavior>().staminaBar = player1Canvas.transform.Find("GameObject/AuraBar").GetComponent<Image>();
        player1.GetComponent<auraGunBehavior>().staminaBar.fillAmount = 1;
		player1Tracker.Player = player1.transform;

		addedScore2 = false;

		//add invincibility on spawn
		playerHealth1.StartCoroutine(playerHealth1.colorChange(iFrameFlashDuration, iFrameNumber));
	}

    void SpawnPlayer2(){
		if (player2) {
			Destroy (player2);
		}
        Debug.Log("Spawning player 2");

        player2 = Instantiate(player2Prefab, player2Start.position = new Vector3(player2Spawns[index2].x, player2Spawns[index2].y + 5, player2Spawns[index2].z), Quaternion.identity) as GameObject;
        //player2.transform.localScale = player2Scale;
		playerHealth2 = player2.GetComponent<auraPlayerHealth>();
		players[1] = player2.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan2 = player2.GetComponent<BulletManager>();
        player2.GetComponent<auraGunBehavior>().curStamina = player2.GetComponent<auraGunBehavior>().staminaTotal;
        //Debug.Log("player 2's stamina is"+ player2.GetComponent<auraGunBehavior>().curStamina);
		player2.GetComponent<auraGunBehavior>().staminaBar = player2Canvas.transform.Find("GameObject/AuraBar").GetComponent<Image>();
        player2.GetComponent<auraGunBehavior>().staminaBar.fillAmount = 1;
		player2Tracker.Player = player2.transform;

		addedScore1 = false;

		//add invincibility on spawn
		playerHealth2.StartCoroutine(playerHealth2.colorChange(iFrameFlashDuration, iFrameNumber));

	}

    IEnumerator DelayedSpawnPlayer1 ()
    {
        yield return new WaitForSeconds(respawnTime);
        Instantiate(respawnBulletDestroyer, player1Spawns[index1], Quaternion.identity);
        yield return new WaitForSeconds(.1f);
        SpawnPlayer1();
        
    }
    IEnumerator DelayedSpawnPlayer2()
    {
        yield return new WaitForSeconds(respawnTime);
        Instantiate(respawnBulletDestroyer, player2Spawns[index2], Quaternion.identity);
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

    public IEnumerator TimerCo ()
    {
        for (int i = 0; i < zones.Length; i++)
        {
            yield return new WaitForSeconds(zones[i].zoneTime);
            if (index1 < player1Spawns.Length-1)
            {
                index1++;
            }
            if (index2 < player2Spawns.Length-1)
            {
                index2++;
            }

            for (int j = 0; j < zones[i].sections.Length; j++)
            {
                zones[i].sections[j].Drop();

            }
        }
    }

    public static float remapRange(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        float newValue = 0;
        float oldRange = (oldMax - oldMin);
        float newRange = (newMax - newMin);
        newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
        return newValue;
    }
}
