using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOnScript : MonoBehaviour {
//	public GameObject newTarget;
	public GameObject currentTarget;
	public bool LockedOn = false;
	public TwoDCharacterController myCont;
//    private GameObject playerCamera;
	Vector3 sphereLoc;
	public float sphereRad;
	private float reticleOffset = -94f;
	public Transform reticle;
	// Use this for initialization

	void Start ()
	{
		reticle.gameObject.SetActive (false);
	}

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
			/*RectTransform CanvasRect= reticle.transform.parent.GetComponent<RectTransform>();
			Vector2 ViewportPosition= Camera.main.WorldToViewportPoint(currentTarget.transform.position);
			Vector2 WorldObject_ScreenPosition=new Vector2(
				((ViewportPosition.x * CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
				((ViewportPosition.y * CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)));
			Vector2 pos = reticle.transform.parent.InverseTransformPoint (currentTarget.transform.position);Camera.main.WorldToScreenPoint (currentTarget.transform.position);*/
			Vector3 pos = currentTarget.transform.position;//Camera.main.ScreenToWorldPoint (Camera.main.WorldToScreenPoint (currentTarget.transform.position));
			reticle.gameObject.SetActive (true);
			reticle.transform.position = new Vector3(pos.x, pos.y+2f, pos.z);//WorldObject_ScreenPosition;
        }
	}
		void unlock ()
		{
		if (currentTarget != null && myCont.Unlock()) {
						currentTarget = null;
			LockedOn = false;
			reticle.gameObject.SetActive (false);
			}
		}

}


