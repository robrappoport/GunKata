﻿using System.Collections;
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
	public float winChanceSendUpTime = 1, winChanceRemainTime = 1;
	public int winChanceTargetSize = 36;
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
					StartCoroutine (TimeManipulation.SlowTimeTemporarily (timeSlowScale, timeSlowingDuration, timeSlowedDuration, timeReturnDuration));
					StartCoroutine (SendWinChanceTextUp (winningPlayer, winChanceSendUpTime, winChanceRemainTime, winChanceTargetSize));
					StartCoroutine (TimeSlowCamZoom (timeSlowZoomMaxDistance, timeSlowZoomInDuration, timeSlowZoomOutDuration));
				
					//victoryText.enabled = true;

					GetComponent<Image> ().color = Color.yellow;
					switch (winningPlayer) {
					case 0:
                            victoryText.color = new Color (174, 255, 246, 255);
						break;
					case 1:
						victoryText.color = Color.red;
						break;
					default:
						victoryText.color = Color.black;
						break;
					}
				}

			} else {
				winChanceCoroutinesStarted = false;
				victoryText.enabled = false;
				GetComponent<Image> ().color = Color.gray;
			}
		}
	}


	IEnumerator TimeSlowCamZoom (float zoomDistance = 900, float zoomInDuration = 0.2f, float zoomOutDuration= 0.2f){
		CameraMultitarget cam = Camera.main.GetComponent<CameraMultitarget> ();
		float elapsedTime = 0;
		float totalTime = timeSlowedDuration + timeSlowingDuration + timeReturnDuration - zoomInDuration - zoomOutDuration;
		float startingMaxZoom = cam.maxDistanceToTarget;
		float startingMinZoom = cam.minDistanceToTarget;
		while (elapsedTime < zoomInDuration) {
			elapsedTime += Time.deltaTime;
			cam.maxDistanceToTarget = Mathf.Lerp (startingMaxZoom, zoomDistance, elapsedTime / zoomInDuration);
			if (cam.minDistanceToTarget >= cam.maxDistanceToTarget) {
				cam.minDistanceToTarget = Mathf.Lerp (startingMinZoom, zoomDistance, elapsedTime / zoomInDuration);

			}
			yield return null;
		}
		totalTime -= elapsedTime;
		elapsedTime = 0;
		yield return new WaitForSeconds (totalTime);
		while (elapsedTime < zoomOutDuration) {
			elapsedTime += Time.deltaTime;
			cam.maxDistanceToTarget = Mathf.Lerp (zoomDistance, startingMaxZoom, elapsedTime / zoomOutDuration);
			if (cam.minDistanceToTarget >= cam.maxDistanceToTarget) {
				cam.minDistanceToTarget = Mathf.Lerp (zoomDistance, startingMinZoom, elapsedTime / zoomInDuration);

			}

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
	IEnumerator SendWinChanceTextUp(int playerNum, float sendUpTime = 1, float remainTime = 1, int targetSize = 5){
		float elapsedTime = 0;
		float startTime = Time.realtimeSinceStartup;
		victoryText.enabled = true;
		victoryText.text = TwoDGameManager.thisInstance.playerNames [playerNum] + " Godhood Imminent \n Praise Be the New God!";
		RectTransform victorTextRectTransform = victoryText.GetComponent<RectTransform> ();
		int startingFont = victoryText.fontSize;
		int targetFont = targetSize;
//		Vector3 startingSize = victorTextRectTransform.localScale;
//		Vector3 finalSize = startingSize * targetSize;
		Vector2 startingPosition = victorTextRectTransform.anchoredPosition;
		Vector2 middleOfScreen = new Vector2 (0, 100);
		while (elapsedTime < sendUpTime) {
			elapsedTime = Time.realtimeSinceStartup - startTime;

			victorTextRectTransform.anchoredPosition = Vector2.Lerp (startingPosition, middleOfScreen, elapsedTime / sendUpTime);
		//	victorTextRectTransform.localScale = Vector3.Lerp (startingSize, finalSize, elapsedTime / sendUpTime);
			victoryText.fontSize = (int)Mathf.Lerp(startingFont, targetFont, elapsedTime/sendUpTime);
			yield return null;
		
		}
		yield return new WaitForSeconds (remainTime);
		victorTextRectTransform.anchoredPosition = startingPosition;
	//	victorTextRectTransform.localScale = startingSize;
		victoryText.fontSize = startingFont;
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
