using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

//	public static bulletManager Instance;

	public List<Bullet> bulletList = new List<Bullet> ();
	public bool freeze = false;
    public GameObject impactPrefab;

	public void CreateBullet(GameObject bulletType, Vector3 bulletPos, Quaternion bulletRot){
		GameObject bulletObj = Instantiate (bulletType, bulletPos, bulletRot);
		Rigidbody bulletRigid = bulletObj.GetComponent<Rigidbody> ();
//		bulletRigid.velocity = this.gameObject.GetComponent<Rigidbody> ().velocity;
		Bullet bullet = bulletObj.GetComponent<Bullet> ();
		bullet.ownerNumber = GetComponent<AuraCharacterController> ().playerNum;
        bullet.impactPrefab = impactPrefab;
        if (bullet.ownerNumber == 0)
        {
            bulletObj.layer = LayerMask.NameToLayer("Player1Bullet");
        }
        else if (bullet.ownerNumber == 1)
        {
            bulletObj.layer = LayerMask.NameToLayer("Player2Bullet");
        }
//		float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
//		float velocityOffset = Mathf.Max(Vector3.Dot (new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)), this.gameObject.GetComponent<Rigidbody> ().velocity), 0);
//		bullet.bulletSpeed += velocityOffset;
//		Debug.Log (velocityOffset);
		bulletList.Add(bullet);
		bullet.BMan = this;

	}

	public void DestroyBullet(Bullet bullet)
	{
		Destroy (bullet.gameObject);
		bulletList.Remove (bullet);
	}

    //public void Freeze(bool setFreeze){
    //	foreach (Bullet b in bulletList) {
    //		//b.SetFreeze (setFreeze);
    //	}
    //}
   
    void Update ()
	{
		for (int i = bulletList.Count - 1; i >= 0; i--) {
//			Debug.Log (bulletList [i].lifeTime);
			if (bulletList [i].lifeTime < 0) {
				DestroyBullet (bulletList [i]);
			}
		}

	}




}

