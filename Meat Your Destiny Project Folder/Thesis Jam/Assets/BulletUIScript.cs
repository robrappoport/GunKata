using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUIScript : MonoBehaviour
{
	public GameObject[] Bullets;
    public Sprite[] BulletSprites;
	public GameObject defaultBullet;

    public Image BulletUI;
	public Sprite LoadedBullet, emptyBullet, specialBullet;

	public TwoDGunBehaviorBigClip gunReference;
	public Text weaponLabel;
	public Text ammoCounter;

	private int BulletCounter = 0;

	public void Awake(){
		BuildBulletSpriteArray ();
		DrawBullets ();
	}
	public void Start (){
//		if (gunReference.CurrentBullets > 0) {
//			BulletUI.sprite = BulletSprites [gunReference.CurrentBullets-1];
//			Debug.Log (gunReference.CurrentBullets);
//		} else if (gunReference.CurrentBullets == 0) {
//			BulletUI.sprite = BulletSprites[7];
//		} else{
//			BulletUI.sprite = BulletSprites[8];
//		}
		weaponLabel.text = gunReference.weaponLabel;
		ammoCounter.text = gunReference.CurrentBullets.ToString();
	}

	public void Update(){
		if (BulletCounter != gunReference.CurrentBullets) {
			BulletCounter = gunReference.CurrentBullets;
			DrawBullets ();
		}
		weaponLabel.text = gunReference.weaponLabel;
		ammoCounter.text = gunReference.CurrentBullets.ToString();


	}

	void BuildBulletSpriteArray (){//prepares the default bullet layout
		for (int i = 0; i < Bullets.Length; i++) {
			Bullets [i] = Instantiate (defaultBullet, this.transform) as GameObject;
		}
	}
	void DrawBullets(){//draws the bullets from left to right, changing the sprites each time the gun shoots
		
		for (int i = 0; i <Bullets.Length;  i++) {
			Bullets [i].GetComponent<Image> ().sprite = LoadedBullet;
			Bullets [i].GetComponent<RectTransform> ().localPosition = new Vector3 (Bullets [i].GetComponent<RectTransform> ().rect.width * (i + 1), 
				Bullets [i].GetComponent<RectTransform> ().rect.height, 0);

			if ((gunReference.MaxBullets - gunReference.CurrentBullets + i + 1) % gunReference.specialBulletPeriod == 0) {
				Bullets [i].GetComponent<Image> ().sprite = specialBullet;			

			}
			if (gunReference.CurrentBullets - i -1 < 0) {
				Bullets [i].GetComponent<Image> ().sprite = emptyBullet;
			}
		}
	}
		
}
