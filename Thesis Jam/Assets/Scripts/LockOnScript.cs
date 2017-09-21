using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnScript : MonoBehaviour {
//	public GameObject newTarget;
	public GameObject currentTarget;
	public bool LockedOn = false;
	public TwoDCharacterController myCont;
//    private GameObject playerCamera;
	Vector3 sphereLoc;
	public float sphereRad;
	// Use this for initialization
	void Update () {
		sphereLoc = transform.position;
		LockOnCheck ();
		lockOn ();
		unlock ();
	}
	
	void LockOnCheck (){
		Collider[] checkColliders = Physics.OverlapSphere (sphereLoc, sphereRad);
		if (currentTarget == null) {
            float distance = Mathf.Infinity;
			if (myCont.onLock()) { 
			
                foreach (Collider c in checkColliders)
                 {
					if (c.gameObject.tag == "Player")
					{
						if (c.gameObject.GetComponent<LockOnScript> () != this) {
							if (Vector3.Distance (c.gameObject.transform.position, transform.position) < distance) {
								currentTarget = c.gameObject;
								distance = Vector3.Distance (c.gameObject.transform.position, transform.position);
							}
						}
                    }
                 }
				}
			}

		}

	void lockOn ()
	{
		if (currentTarget != null) {
			LockedOn = true;
//            playerCamera = this.transform.Find("CameraPivot").gameObject;
            Transform target = currentTarget.transform;
//            Transform cameraTarget = playerCamera.transform;
		transform.LookAt (target);
			Vector3 lockDir = currentTarget.transform.position - transform.position;
			Debug.DrawRay (transform.position, lockDir * 50, Color.yellow);
//			playerCamera.transform.LookAt (target);
//            transform.LookAt(cameraTarget);




        }
	}
		void unlock ()
		{
		if (currentTarget != null && myCont.Unlock()) {
						currentTarget = null;
			LockedOn = false;
			}
		}

}


