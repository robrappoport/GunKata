using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

//	public static bulletManager Instance;

	public List<Bullet> bulletList = new List<Bullet> ();
	public bool freeze = false; 

	public void CreateBullet(GameObject bulletType, Vector3 bulletPos, Quaternion bulletRot){
		GameObject bulletObj = Instantiate (bulletType, bulletPos, bulletRot);
		Bullet bullet = bulletObj.GetComponent<Bullet> ();
		bulletList.Add(bullet);
		bullet.BMan = this;

	}

	public void DestroyBullet(Bullet bullet)
	{
		Destroy (bullet.gameObject);
		bulletList.Remove (bullet);
	}

	public void Freeze(bool x){
		foreach (Bullet b in bulletList) {
			b.SetFreeze (b);
		}
	}

	void Update ()
	{
		for (int i = bulletList.Count - 1; i >= 0; i--) {
//			Debug.Log (bulletList [i].lifeTime);
			if (bulletList [i].lifeTime < 0) {
				DestroyBullet (bulletList [i]);
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			freeze = !freeze;
			Freeze (freeze);	
		}
	}




}

