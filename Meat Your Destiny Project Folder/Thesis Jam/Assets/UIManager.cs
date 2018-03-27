using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public  class UIManager : MonoBehaviour {

	public static UIManager thisInstance;
	public Vector3 startPos;
	public List<GameObject> scoreCards;
	public Hashtable h;
	public GameObject scoreCardPrefab;
	public List<Turret> turretList = new List<Turret> ();

	Vector3 endPos;
	float maxDist;
	Text victoryText;

	void Awake(){
		thisInstance = this;

	}
	// Use this for initialization
	void Start () {
		victoryText = GetComponentInChildren<Text> ();
		victoryText.enabled = false;
		endPos = new Vector3 (startPos.x * -1, startPos.y, 0);
		maxDist = Vector3.Distance (startPos, endPos); 

	}

	// Update is called once per frame
	void Update () {
		CheckWin ();
		UpdateScore ();	
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
				victoryText.enabled = true;

				GetComponent<Image> ().color = Color.yellow;
				switch (winningPlayer) {
				case 0:
					victoryText.text = "blue can win";
					victoryText.color = Color.blue;
					break;
				case 1:
					victoryText.text = "red can win!";
					victoryText.color = Color.red;
					break;
				default:
					victoryText.text = "Error";
					victoryText.color = Color.black;
					break;
				}

			} else {
				victoryText.enabled = false;
				GetComponent<Image> ().color = Color.gray;
			}
		}
	}
	void UpdateScore(){
		turretList = turretList.OrderBy (
			x => Camera.main.WorldToScreenPoint (x.transform.position).x).ToList ();
		for(int i = 0; i < scoreCards.Count ; i++){
			scoreCards [i].transform.Find ("Top Card").GetComponent<Image> ().color = turretList [i].currentColor;
			scoreCards [i].transform.Find ("Bottom Card").GetComponent<Image> ().color = turretList [i].neutralColor;

			scoreCards [i].transform.Find ("Top Card").GetComponent<Image> ().fillAmount = turretList [i].charge / turretList [i].segmentNum;

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
		GameObject scoreCard = Instantiate (scoreCardPrefab) as GameObject;
		scoreCard.transform.SetParent (this.transform);
		scoreCard.transform.localPosition = Vector3.zero;
		Vector2 cardPos = new Vector2 (startPos.x + ((float)(i)/(turretList.Count - 1) *maxDist), startPos.y);
		scoreCard.transform.Find ("Top Card").transform.GetComponent<RectTransform> ().anchoredPosition = cardPos;
		scoreCard.transform.Find ("Bottom Card").transform.GetComponent<RectTransform>().anchoredPosition = cardPos;
		scoreCards.Add (scoreCard);

	}

	public void Reset (){
		scoreCards.Clear ();
		turretList.Clear ();
	}
}
