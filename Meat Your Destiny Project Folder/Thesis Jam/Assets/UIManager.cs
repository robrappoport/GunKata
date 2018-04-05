using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public  class UIManager : MonoBehaviour {

	public static UIManager thisInstance;
	public Vector3 startPos;
	public List<GameObject> scoreCards, scoreCardPool;
	public Hashtable h;
	public GameObject scoreCardPrefab;
	public List<Turret> turretList = new List<Turret> ();
	public float timeSlowScale = 0.5f, timeSlowingDuration = 1, timeSlowedDuration = 1, timeReturnDuration = 1;
	public float winChanceSendUpTime = 1, winChanceRemainTime = 1, winChanceTargetFontSize = 36;
	public float timeSlowZoomMaxDistance = 900, timeSlowZoomInDuration = 0.2f, timeSlowZoomOutDuration = 0.2f;
	Vector3 endPos;
	float maxDist;
	Text victoryText;
	bool winChanceCoroutinesStarted = false;

	void Awake(){
		thisInstance = this;

	}
	// Use this for initialization
	void Start () {
		victoryText = GetComponentInChildren<Text> ();
		victoryText.enabled = false;
		endPos = new Vector3 (startPos.x * -1, startPos.y, 0);
		maxDist = Vector3.Distance (startPos, endPos); 
		GenerateCardPool (30);
	}
		
	// Update is called once per frame
	void Update () {
		CheckWin ();

		UpdateScore ();	

	}

	void GenerateCardPool (int j){
		for (int i = 0; i < j; i++) {
			GameObject scoreCard = Instantiate (scoreCardPrefab) as GameObject;
			scoreCardPool.Add (scoreCard);
			scoreCard.transform.SetParent (thisInstance.transform);
			scoreCard.SetActive (false);


		}
	}
	void CheckWin(){
		if (turretList.Count > 0) {
			bool canWin = true;
			int winningPlayer = 2;
			for (int i = 0; i < turretList.Count; i++) {
				if (turretList [i].ownerNum != turretList [Mathf.Clamp (i - 1, 0, turretList.Count)].ownerNum || turretList [i].ownerNum == 2) {
					canWin = false;
					break;
				}
			}
			winningPlayer = turretList [0].GetComponent<Turret> ().ownerNum;
			if (canWin) {
				if (!winChanceCoroutinesStarted) {
					winChanceCoroutinesStarted = true;
					StartCoroutine (SlowTimeTemporarily (timeSlowScale, timeSlowingDuration, timeSlowedDuration, timeReturnDuration));
					StartCoroutine (SendWinChanceTextUp (winningPlayer, winChanceSendUpTime, winChanceSendUpTime));
					StartCoroutine (TimeSlowCamZoom (timeSlowZoomMaxDistance, timeSlowZoomInDuration, timeSlowZoomOutDuration));
				}
				//victoryText.enabled = true;

//				GetComponent<Image> ().color = Color.yellow;
//				switch (winningPlayer) {
//				case 0:
//					victoryText.text = "blue can win";
//					victoryText.color = Color.blue;
//					break;
//				case 1:
//					victoryText.text = "red can win!";
//					victoryText.color = Color.red;
//					break;
//				default:
//					victoryText.text = "Error";
//					victoryText.color = Color.black;
//					break;
//				}

			} else {
				winChanceCoroutinesStarted = false;
				victoryText.enabled = false;
				GetComponent<Image> ().color = Color.gray;
			}
		}
	}
	/// <summary>
	/// Slows time temporarily.
	/// </summary>
	/// <param name="timeScale">Time scale while slowed.</param>
	/// <param name="slowingDuration">How long it should take to slow down.</param>
	/// <param name="slowedDuration">How long it should stay slowed.</param>
	/// <param name="returnDuration">How long it should take to return back to normal time.</param>
	public static IEnumerator SlowTimeTemporarily(float slowedTimeScale = 0.5f, float slowingDuration = 1, float slowedDuration = 1, float returnDuration = 1){

		float startingTimeScale = Time.timeScale;
		float elapsedTime = 0;
		while (elapsedTime < slowingDuration) {

			elapsedTime += Time.deltaTime;
			Time.timeScale = Mathf.Lerp (1, slowedTimeScale, elapsedTime / slowingDuration);
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			yield return null;
		}
		yield return new WaitForSeconds (slowedDuration);
		elapsedTime = 0;
		while (elapsedTime < returnDuration) {
			elapsedTime += Time.deltaTime; 
			Time.timeScale = Mathf.Lerp (slowedTimeScale, 1, elapsedTime / returnDuration);
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			yield return null;
		}
	}

	IEnumerator TimeSlowCamZoom (float zoomDistance = 900, float zoomInDuration = 0.2f, float zoomOutDuration= 0.2f){
		CameraMultitarget cam = Camera.main.GetComponent<CameraMultitarget> ();
		float elapsedTime = 0;
		float totalTime = timeSlowedDuration + timeSlowingDuration + timeReturnDuration - zoomInDuration - zoomOutDuration;
		float startingZoom = cam.maxDistanceToTarget;

		while (elapsedTime < zoomInDuration) {
			elapsedTime += Time.deltaTime;
			cam.maxDistanceToTarget = Mathf.Lerp (startingZoom, zoomDistance, elapsedTime / zoomInDuration);
			yield return null;
		}
		totalTime -= elapsedTime;
		elapsedTime = 0;
		yield return new WaitForSeconds (totalTime);
		while (elapsedTime < zoomOutDuration) {
			elapsedTime += Time.deltaTime;
			cam.maxDistanceToTarget = Mathf.Lerp (zoomDistance, startingZoom, elapsedTime / zoomOutDuration);
			yield return null;
		}


	}
	/// <summary>
	/// Sends the win chance text up 
	/// </summary>
	/// <returns>The window chance text up.</returns>
	/// <param name="playerNum">Number of player who can win.</param>
	/// <param name="sendUpTime">Time it takes for text to rise.</param>
	/// <param name="remainTime">Time text remains on screen.</param>
	IEnumerator SendWinChanceTextUp(int playerNum, float sendUpTime = 1, float remainTime = 1, float targetFontSize = 36){
		float elapsedTime = 0;
		victoryText.enabled = true;
		int startingFontSize = victoryText.fontSize;
		victoryText.text = TwoDGameManager.thisInstance.playerNames [playerNum].ToLower() + " can seize victory!";
		RectTransform victorTextRectTransform = victoryText.GetComponent<RectTransform> ();
		Vector2 startingPosition = victorTextRectTransform.anchoredPosition;
		Vector2 middleOfScreen = new Vector2 (Screen.width / 2, Screen.height / 2);
		while (Vector2.Distance (victorTextRectTransform.anchoredPosition, middleOfScreen) < 0.01f) {
			elapsedTime += Time.deltaTime;
			victorTextRectTransform.anchoredPosition = Vector2.Lerp (startingPosition, middleOfScreen, elapsedTime / sendUpTime);
			victoryText.fontSize = (int)Mathf.Lerp (startingFontSize, targetFontSize, elapsedTime / sendUpTime);
			yield return null;
		
		}
		yield return new WaitForSeconds (remainTime);
		victorTextRectTransform.anchoredPosition = startingPosition;
		victoryText.fontSize = startingFontSize;
		victoryText.enabled = false;
	}
	void UpdateScore(){
		turretList = turretList.OrderBy (
			x => Camera.main.WorldToScreenPoint (x.transform.position).x).ToList ();

		if (turretList.Count > 0) {
			for (int i = 0; i < scoreCards.Count; i++) {
				int j = Mathf.Clamp (i, 0, turretList.Count - 1);
				scoreCards [i].transform.Find ("Top Card").GetComponent<Image> ().color = turretList [j].currentColor;
				scoreCards [i].transform.Find ("Bottom Card").GetComponent<Image> ().color = turretList [j].neutralColor;

				scoreCards [i].transform.Find ("Top Card").GetComponent<Image> ().fillAmount = turretList [j].charge / turretList [i].segmentNum;

			}
		} else {
			foreach (GameObject s in scoreCards) {
				s.SetActive (false);
			}
		}
	}
	void DrawScore(){
		scoreCards = new List<GameObject> ();
		//for each turret, draw a number of scorecards 
	
		DrawCard(0);
		for(int i = 1; i < Mathf.Clamp(turretList.Count - 1, 0, Mathf.Infinity); i++){
			DrawCard (i);
		}

		DrawCard (turretList.Count -1);

		//register each of the current turrets to it 
	}

	void DrawCard(int i = 0){
		if (scoreCards.Count < turretList.Count ) {
			GameObject scoreCard = scoreCardPool [i];
			scoreCard.SetActive (true);
			scoreCardPool.Remove (scoreCard);
			scoreCard.transform.SetParent (this.transform);
			scoreCard.transform.localPosition = Vector3.zero;
			Vector2 cardPos = new Vector2 (startPos.x + ((float)(i) / (turretList.Count - 1) * maxDist), startPos.y);
			scoreCard.transform.Find ("Top Card").transform.GetComponent<RectTransform> ().anchoredPosition = cardPos;
			scoreCard.transform.Find ("Bottom Card").transform.GetComponent<RectTransform> ().anchoredPosition = cardPos;
			scoreCards.Add (scoreCard);
		}

	}

	public void Reset (){
		foreach (GameObject s in scoreCards) {
			s.SetActive (false);
		}
		scoreCards.Clear ();
		turretList.Clear ();
	}
}
