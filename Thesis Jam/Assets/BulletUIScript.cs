using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUIScript : MonoBehaviour
{

    public Sprite[] BulletSprites;

    public Image BulletUI;

	public TwoDGunBehavior gunReference;
	public Text weaponLabel;
	public void Start (){
		if (gunReference.CurrentBullets > 0) {
			BulletUI.sprite = BulletSprites [gunReference.CurrentBullets-1];
			Debug.Log (gunReference.CurrentBullets);
		} else if (gunReference.CurrentBullets == 0) {
			BulletUI.sprite = BulletSprites[7];
		} else{
			BulletUI.sprite = BulletSprites[8];
		}
		weaponLabel.text = gunReference.weaponLabel;
	}

	public void Update(){
		if (gunReference.CurrentBullets > 0) {
			BulletUI.sprite = BulletSprites [gunReference.CurrentBullets-1];
		} else if (gunReference.CurrentBullets == 0) {
			BulletUI.sprite = BulletSprites[7];
		} else{
			BulletUI.sprite = BulletSprites[8];
		}
	}




//    private Player bulletplayer;


//    private void Start()
//    {
//        bulletplayer = GameObject.FindGameObjectWithTag("Player").GetComponent<TwoDGunBehavior>();
//    }
//
//    private void Update()
//    {
//        BulletUI.sprite = BulletSprites[bulletplayer.currentBullets];
//    } 













}
