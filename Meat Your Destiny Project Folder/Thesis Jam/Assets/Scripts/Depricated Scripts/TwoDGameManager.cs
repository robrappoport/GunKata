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

    //public UITracker player1Tracker;
    //public UITracker player2Tracker;

    public GameObject player1Canvas, player2Canvas;

    public GameObject textPrefab;
    public CameraMultitarget cam;

	public int iFrameNumber = 10;
	public float iFrameFlashDuration = .1f;

    public float shakeTime = 2f, shakeWeight = .5f;

    [Header("SuperBall Vars")]
    public List<Turret> keyTurrets;
    bool readyToMakeNewOrb;
    GameObject ball;
    public Vector3 ballLoc;
    public float ballTime;
    public float ballTimeTotal;
    public bool ballDestroyed = false;
    public int zoneIndex;
    public float zoneTimeElapsed;

//	GameObject audioManagerClone;
//	public GameObject audioManagerPrefab;

	[Header("Turret Spawn Vars")]
	public List<List<Turret>>turrets;
	public float turretDistanceMod = 3;
	public float spawnResetTimeLimit = 1;
	float spawnResetTimer;
	//uncomment these to watch the lists in realtime
	public List<Turret> neutralTurrets, p1Turrets, p2Turrets;
	public bool readyToActivateNextSections = true;


    void OnApplicationQuit()
    {
        thisInstance = null;
    }
	void Awake ()
	{
		
        cam = Camera.main.GetComponent<CameraMultitarget>();
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
		turrets = new List<List<Turret>> (players.Length + 1);
		for (int i = 0; i < turrets.Capacity; i++) {
			turrets.Add (new List<Turret> ());
		}
		neutralTurrets = turrets [2];
		p2Turrets = turrets [1];
		p1Turrets = turrets [0];


        /////////GAME INSTANTIATION OCCURS HERE/////////

		setLevel();


        ball = FindObjectOfType<TheBallScript>().gameObject;

        ballLoc = ball.transform.position;


        ball.SetActive(false);


	}

    private void Start()
    {
        player1Scale = player1.transform.localScale;
        player2Scale = player2.transform.localScale;
		StartCoroutine(ActivateSections (0));
    }


    void Update()
    {
		spawnResetTimer += Time.deltaTime;
        StartCoroutine(BallTimer());
     //   player1Start.position = player1Spawns[index1];
     //   player2Start.position = player2Spawns[index2];
        playerScoreUpdate();
        if (playerHealth1.CurrentHealth <= 0 || playerHealth2.CurrentHealth <= 0)
        {
			CheckPlayerWin ();
			cam.Shake(shakeWeight, shakeTime);
            if (playerHealth1.CurrentHealth <= 0 && addedScore2 == false)
            {
                //player1Scale = player1.transform.localScale;
                StartCoroutine(DelayedSpawnPlayer1());
                addedScore2 = true;
                //player2ScoreNum += 10f;
                return;
            }
            if (playerHealth2.CurrentHealth <= 0 && addedScore1 == false)
            {
                //player2Scale = player2.transform.localScale;
                StartCoroutine(DelayedSpawnPlayer2());
                addedScore1 = true;
                //player1ScoreNum += 10f;
                return;
            }

        }
      //  playerWin();
        //for (int i = 0; i < keyTurrets.Count; i++)
        //{
        //    if(keyTurrets[i].ownerNum == 2){
        //        readyToMakeNewOrb = false;
        //        break;
        //    }else{
        //        //readyToMakeNewOrb = true;
        //    }
        //}

        if(readyToMakeNewOrb){
            ball.SetActive(true);
            //readyToMakeNewOrb = false;

        }
    }
        public IEnumerator gameRestart ()
	{
		yield return new WaitForSeconds (restartTime);
        resetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        addedScore1 = false;
        addedScore2 = false;
	}
    public GameObject GetPlayer(int playerNum){
        
        if(playerNum == 0){
            if (player1)
            {
                return player1;
            }
            else
            {
                return null;
            }
        }
        else if(playerNum == 1){
            if (player2)
            {
                return player2;
            }
            else
            {
                return null;
            }
        }else{
            return null;
            
        }
    }

    public void OnBallDestroyed(int playerNum){
        readyToMakeNewOrb = false;
        if (zoneTimeElapsed > 1f)
        {
            zoneTimeElapsed = 1f;  
        }
        foreach (Turret t in zones[zoneIndex].GetComponentInChildren<SectionScript>().sectionTurret)
        {
            keyTurrets.Remove(t);

			//remove turrets from score calculations
			for (int i = 0; i < turrets.Count; i++) {
				turrets [i].Remove (t);
			}
        }
        foreach(Turret t in keyTurrets){
            t.Reset();
        }
        //
    }
    public void playerScoreUpdate ()
    {
        //Debug.Log("player 1 score: " + player1ScoreNum + " player2 score: " + player2ScoreNum);
        //Debug.Log(displayedPlayer2Score);

        player1ScoreNum = Mathf.Clamp(player1ScoreNum, 0, maxScore);
        player2ScoreNum = Mathf.Clamp(player2ScoreNum, 0, maxScore);
        //float scale1 = remapRange(player1ScoreNum, 0, maxScore, 1, maxScale);
        //float scale2 = remapRange(player2ScoreNum, 0, maxScore, 1, maxScale);
        displayedPlayer1Score = Mathf.Lerp(displayedPlayer1Score, player1ScoreNum / maxScore, Time.deltaTime * lerpTime);
        displayedPlayer2Score = Mathf.Lerp(displayedPlayer2Score, player2ScoreNum / maxScore, Time.deltaTime * lerpTime);
        //scale1 = Mathf.Clamp(scale1, 1, maxScale);
        //scale2 = Mathf.Clamp(scale2, 1, maxScale);
        player1Score.fillAmount = (displayedPlayer1Score);
        player2Score.fillAmount = (displayedPlayer2Score);
        //Vector3 newScale1 = new Vector3(scale1, scale1, scale1);
        //Vector3 newScale2 = new Vector3(scale2, scale2, scale2);
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
        //Debug.Log("What's up?");
		SpawnPlayer1 ();
		SpawnPlayer2 ();
    }

	void SpawnPlayer1(Vector3 spawnPos = default(Vector3)){
		if (player1) {
			Destroy (player1);
		}
        //player1.transform.localScale = player1Scale;
		if (spawnResetTimer > spawnResetTimeLimit) {
			spawnPos = new Vector3 (spawnPos.x, 168.8f, spawnPos.z);

			player1 = Instantiate (player1Prefab, player2Start.position = spawnPos, Quaternion.identity) as GameObject;
		} else {
			player1 = Instantiate(player1Prefab, new Vector3 (player1Spawns[index1].x, player1Spawns[index1].y + 5, player1Spawns[index1].z), Quaternion.identity) as GameObject;
		}
		playerHealth1 = player1.GetComponent<auraPlayerHealth>();
		players[0] = player1.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan1 = player1.GetComponent<BulletManager>();
        //Debug.Log("player 1's stamina is" + player1.GetComponent<auraGunBehavior>().curStamina);
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[0] = player1Canvas.transform.Find("AuraLvl1/AuraBar1").GetComponent<Image>();
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[0].fillAmount = 1;
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[1] = player1Canvas.transform.Find("AuraLvl2/AuraBar2").GetComponent<Image>();
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[1].fillAmount = 1;
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[2] = player1Canvas.transform.Find("AuraLvl3/AuraBar3").GetComponent<Image>();
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[2].fillAmount = 1;
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[3] = player1Canvas.transform.Find("AuraLvl4/AuraBar4").GetComponent<Image>();
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[3].fillAmount = 1;
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[4] = player1Canvas.transform.Find("AuraLvl5/AuraBar5").GetComponent<Image>();
        player1.GetComponent<auraGunBehavior>().auraStamImgArray[4].fillAmount = 1;
		//player1Tracker.Player = player1.transform;

		addedScore2 = false;

		//add invincibility on spawn
		playerHealth1.StartCoroutine(playerHealth1.colorChange(iFrameFlashDuration, iFrameNumber));
	}

	void SpawnPlayer2(Vector3 spawnPos = default(Vector3)){
		if (player2) {
			Destroy (player2);
		}
			
        Debug.Log("Spawning player 2");
		if (spawnResetTimer > spawnResetTimeLimit) {
			spawnPos = new Vector3 (spawnPos.x, 168.8f, spawnPos.z);
			player2 = Instantiate (player2Prefab, player2Start.position = spawnPos, Quaternion.identity) as GameObject;
		} else {
			player2 = Instantiate (player2Prefab, player2Start.position = new Vector3 (player2Spawns [index2].x, player2Spawns [index2].y + 5, player2Spawns [index2].z), Quaternion.identity) as GameObject;
		}

        //player2.transform.localScale = player2Scale;
		playerHealth2 = player2.GetComponent<auraPlayerHealth>();
		players[1] = player2.GetComponent<auraGunBehavior>();
		GetComponent<bulletManagerManager>().bMan2 = player2.GetComponent<BulletManager>();
        //Debug.Log("player 2's stamina is"+ player2.GetComponent<auraGunBehavior>().curStamina);

		//aura charge stuff
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[0] = player2Canvas.transform.Find("AuraLvl1/AuraBar1").GetComponent<Image>();
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[0].fillAmount = 1;
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[1] = player2Canvas.transform.Find("AuraLvl2/AuraBar2").GetComponent<Image>();
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[1].fillAmount = 1;
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[2] = player2Canvas.transform.Find("AuraLvl3/AuraBar3").GetComponent<Image>();
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[2].fillAmount = 1;
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[3] = player2Canvas.transform.Find("AuraLvl4/AuraBar4").GetComponent<Image>();
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[3].fillAmount = 1;
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[4] = player2Canvas.transform.Find("AuraLvl5/AuraBar5").GetComponent<Image>();
        player2.GetComponent<auraGunBehavior>().auraStamImgArray[4].fillAmount = 1;
		//player2Tracker.Player = player2.transform;

		addedScore1 = false;

		//add invincibility on spawn
		playerHealth2.StartCoroutine(playerHealth2.colorChange(iFrameFlashDuration, iFrameNumber));

	}

	Vector3 GetSpawnPosition(int playerNum, int neutralNum = 2){
		Vector3 spawnPos = Vector3.zero;
		if (PlayerCanSpawn (playerNum)) {

			if (TwoDGameManager.thisInstance.turrets [playerNum].Count > 0) {//check if there are any turrets available in the player's spawn; otherwise, spawn from an unowned turret
				spawnPos = TwoDGameManager.thisInstance.turrets [playerNum] [Random.Range (0, TwoDGameManager.thisInstance.turrets [1].Count - 1)].transform.position;
			} else {
				spawnPos = TwoDGameManager.thisInstance.turrets [neutralNum] [Random.Range (0, TwoDGameManager.thisInstance.turrets [2].Count - 1)].transform.position;
			}

			spawnPos = new Vector3 (spawnPos.x, 168.8f, spawnPos.z);
		} 
		return spawnPos;

	}
	bool PlayerCanSpawn(int playerNum, int unownedNum = 2){
		if (turrets [playerNum].Count < 1 && turrets [unownedNum].Count < 1) {//check if both the player's list and the neutral list are empty
			return false;
		} else {
			return true;
		}
	}
    IEnumerator DelayedSpawnPlayer1 ()
    {
		Vector3 spawnPos = GetSpawnPosition (0) + Vector3.back * turretDistanceMod;

        yield return new WaitForSeconds(respawnTime);
		Instantiate(respawnBulletDestroyer, spawnPos, Quaternion.identity);
//        Instantiate(respawnBulletDestroyer, player1Spawns[index1], Quaternion.identity);
        yield return new WaitForSeconds(.1f);
		SpawnPlayer1(spawnPos);
        
    }
    IEnumerator DelayedSpawnPlayer2()
    {
		Vector3 spawnPos = GetSpawnPosition (1) + Vector3.back * turretDistanceMod;

        yield return new WaitForSeconds(respawnTime);
		Instantiate(respawnBulletDestroyer, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(.1f);
		SpawnPlayer2(spawnPos);
    }

	void CheckPlayerWin(){
		//        if (player1ScoreNum >= maxScore)
		if (!PlayerCanSpawn (1) && playerHealth2.CurrentHealth <= 0){
			player1.SetActive (false);
			player2.SetActive (false);
			player2Canvas.SetActive (false);
			player1Canvas.SetActive (false);
			playerWinner.text = "blue wins";
			StartCoroutine (gameRestart ());
		}
		//       if (player2ScoreNum >= maxScore)
		else if (!PlayerCanSpawn (0) && playerHealth1.CurrentHealth <=0){			
			player1.SetActive (false);
			player2.SetActive (false);
			player2Canvas.SetActive (false);
			player1Canvas.SetActive (false);
			playerWinner.text = "red wins";
			StartCoroutine (gameRestart ());

		}

	}
    void playerWin()
    {
//        if (player1ScoreNum >= maxScore)
		if(!PlayerCanSpawn(1))
		{
            player1.SetActive(false);
            player2.SetActive(false);
            player2Canvas.SetActive(false);
            player1Canvas.SetActive(false);
            playerWinner.text = "blue wins";
            StartCoroutine(gameRestart());
			spawnResetTimer = 0;
        }
		if(!PlayerCanSpawn(0))
 //       if (player2ScoreNum >= maxScore)
		{
            player1.SetActive(false);
            player2.SetActive(false);
            player2Canvas.SetActive(false);
            player1Canvas.SetActive(false);
            playerWinner.text = "red wins";
            StartCoroutine(gameRestart());
			spawnResetTimer = 0;

        }
    }
    void resetScore ()
    {
        player1ScoreNum = 0f;
        player2ScoreNum = 0f;
    }

    public IEnumerator BallTimer ()
    {
        if (!readyToMakeNewOrb)
        {
            ballTime += Time.deltaTime;
        }
       
        if (ballTime >= ballTimeTotal)
        {
            ballTime = 0f;
            readyToMakeNewOrb = true;
        }
        yield return null;
    }
    public IEnumerator TimerCo ()
    {
        for (int i = 0; i < zones.Length; i++)
        {
           zoneTimeElapsed = zones[i].zoneTime;
            zoneIndex = i;
            while (zoneTimeElapsed > 0)
            {
                zoneTimeElapsed -= Time.deltaTime;
                yield return null;
            }
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
                foreach (Turret t in zones[zoneIndex].GetComponentInChildren<SectionScript>().sectionTurret)
                {
                    keyTurrets.Remove(t);
                }
                zones[i].sections[j].Drop();

            }
			StartCoroutine(ActivateSections (Mathf.Clamp (zoneIndex + 1, 0, zones.Length - 1)));
        }
    }

	IEnumerator ActivateSections(int i = 0){
		while (!readyToActivateNextSections) {
			yield return null;
		}
		foreach (SectionScript s in zones[i].sections) {
			if (s.turretCarrier) {
				StartCoroutine(s.turretCarrier.ActivateTurrets ());
			}
		}
		readyToActivateNextSections = false;

	}

    //public static float remapRange(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
    //{
    //    float newValue = 0;
    //    float oldRange = (oldMax - oldMin);
    //    float newRange = (newMax - newMin);
    //    newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
    //    return newValue;
    //}
}
