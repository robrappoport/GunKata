using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

	public Renderer topRenderer, middleRenderer, bottomRenderer;
	public Color p1Color, p2Color, neutralColor, currentColor;
	public GameObject CannonballPrefab;

	TwoDGameManager gm;
	enum Owner {Player1, Player2, NONE};
	Owner owner = Owner.NONE;
	public int litSegments = 0, ownerNum = 2;
	GameObject Cannon;
	public float startTime, repeatTime, immuneTime;
	bool completelyOwned = false, contestable = true;

	//ownerNum will be received from the playerNum variable from AuraCharacterController script, where 2 acts as "none"
	//I know, I know, 0 makes you think "none" more than 2, but that's how the players are determined and I don't wanna fuck with that.
	void Start () {
		gm = FindObjectOfType<TwoDGameManager> ();
		topRenderer = transform.Find ("Turret Top").GetComponent<Renderer> ();
		middleRenderer =transform.Find ("Turret Middle").GetComponent<Renderer> ();
		bottomRenderer = transform.Find ("Turret Bottom").GetComponent<Renderer> ();
		Cannon = topRenderer.gameObject;
		p1Color = gm.player1.GetComponentInChildren<Renderer> ().material.color;
		p2Color = gm.player2.GetComponentInChildren<Renderer> ().material.color;
		currentColor = neutralColor;
		InvokeRepeating ("Fire", startTime, repeatTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.GetComponent<Bullet> ()) {
			if (contestable) {
				//resolve the ownerNum
				if (owner == Owner.NONE) {//set the owner to whoever hits the turret when the turret is unowned
					ownerNum = col.gameObject.GetComponent<Bullet> ().ownerNumber;
					litSegments = 1;

				} else {
					if (ownerNum == col.gameObject.GetComponent<Bullet> ().ownerNumber) {//if the owning player hits the turret, increment the number of lit segments up to a max of 3
						litSegments = (int)(Mathf.Clamp (litSegments + 1, 0, 3)); 
					} else {//return the tower to neutral if the last segment is depleted; otherwise decrease the number of lit segments by 1
						if (litSegments == 1) {
							litSegments = 0;
							ownerNum = 2;
						} else {
							litSegments = (int)(Mathf.Clamp (litSegments - 1, 0, 3)); 
						}
					}
				}

				AdjustOwnership (ownerNum);
				AdjustCannonStatus ();

				if (litSegments > 2) {
					if (!completelyOwned) {

						completelyOwned = true;
						contestable = false;
						Invoke ("MakeContestable", immuneTime);

					}
				} else {
					completelyOwned = false;
				}
			
			}
			col.gameObject.GetComponent<Bullet> ().BMan.DestroyBullet (col.gameObject.GetComponent<Bullet> ());
		}
	}
		
	void AdjustOwnership(int ownership){
		//adjust ownership and color based on owner number;

		switch (ownership) {
		case 0: //player 1
			owner = Owner.Player1;
			currentColor = p1Color;
			break;
		case 1: //player 2
			owner = Owner.Player2;
			currentColor = p2Color;
			break;
		default: //neutral
			owner = Owner.NONE;
			currentColor = neutralColor;
			break;
		}
	}

	void AdjustCannonStatus(){//adjusts turret based on the number of lit segments;
		if (litSegments > 0) {
			bottomRenderer.material.color = currentColor;
		} else {
			bottomRenderer.material.color = neutralColor;
		}

		if (litSegments > 1) {
			middleRenderer.material.color = currentColor;
		} else {
			middleRenderer.material.color = neutralColor;
		}

		if (litSegments > 2) {
			topRenderer.material.color = currentColor;
		} else {
			topRenderer.material.color = neutralColor;
		}

	}
	void Fire(){
		GameObject cannonBall = Instantiate (CannonballPrefab, Cannon.transform.position, Quaternion.identity, null) as GameObject;
		cannonBall.GetComponent<Cannonball> ().ownerNum = ownerNum;
		if (completelyOwned) {
			if (ownerNum == 0) {
				cannonBall.GetComponent<Renderer> ().material = gm.player1.GetComponentInChildren<Renderer> ().material;
				Physics.IgnoreCollision (gm.player1.GetComponentInChildren<Collider> (), cannonBall.GetComponent<Collider> ());

			} else if (ownerNum == 1) {
				cannonBall.GetComponent<Renderer> ().material = gm.player2.GetComponentInChildren<Renderer> ().material;
				Physics.IgnoreCollision (gm.player2.GetComponentInChildren<Collider> (), cannonBall.GetComponent<Collider> ());

			} else {
				cannonBall.GetComponent<Renderer> ().material.color = neutralColor;
			}
		}
	}

	void Reset(){
		contestable = true;

		ownerNum = 2;
		litSegments = 0;
		completelyOwned = false;
		AdjustOwnership (ownerNum);
		AdjustCannonStatus ();

	}

}
