using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCarrier : MonoBehaviour {
	[Tooltip("Speed at which the turrets orbit")]
	public float orbitingSpeed = 1;
	[Tooltip("Speed at which the turrets leave the arena")]
	public float removalSpeed = 1;
	public float finalDistance;
	[Tooltip("Time it should take for the turret to rise to the top")]
	public float risingTime = 2;
	[Tooltip("Object to define as center")]
	public Transform center;
	[Tooltip("Children of the carrier (turret transforms)")]
	public List<Transform> children;
	Vector3 finalPos; //arrange before runtime

	void Awake () {
		risingTime = Mathf.Clamp (risingTime, 0.00001f, Mathf.Infinity);
		finalPos = transform.position;
		children = new List<Transform> ();
		//generate a list of all transforms, then deactivate each one
		Turret[] ts = GetComponentsInChildren<Turret> ();
		foreach (Turret t in ts) {
			if (t.gameObject != gameObject) {//checks if not self
				children.Add (t.transform);
				t.gameObject.SetActive (false);
			}
		}
		transform.position = transform.position + Vector3.down * 100;

//		//get centroid
//		float x =0, z = 0;
//		foreach (Transform t in children) {
//			x += t.position.x;
//			z += t.position.z;
//		}
//		x = x / children.Count;
//		z = z / children.Count;
//		transform.position = new Vector3 (x, transform.position.y, z);

		//StartCoroutine (SendToEdge());
	}

	public IEnumerator ActivateTurrets(){
		//activate the turrets
		foreach (Transform t in children) {
			if (t.GetComponent<Turret> ()) {
				t.gameObject.SetActive (true);
				t.GetComponent<Turret> ().RegisterTurret ();
			}
		}
		//send them up
		float elapsedTime = 0;
		while (Vector3.Distance (transform.position, finalPos) > 0) {
			transform.position = Vector3.MoveTowards (transform.position, finalPos, elapsedTime / risingTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		//register them as legal spawnpoints with active ui;
		foreach (Transform t in children) {
			if (t.GetComponent<Turret> ()) {
				t.GetComponent<Turret> ().RegisterTurret ();
				t.GetComponent<Turret> ().UICanvas.SetActive (true);
			}
		}


	}



	public IEnumerator SendToEdge(){
		//remove self from game manager's list of turrets
		foreach(Transform t in children){
			foreach (List<Turret> tur in TwoDGameManager.thisInstance.turrets) {
				if(tur.Contains(t.GetComponent<Turret>())){
					t.GetComponent<Turret> ().StartCoroutine (t.GetComponent<Turret> ().FadeUIBar (.1f));
					//t.GetComponent<Turret>().UICanvas.SetActive(false);
					tur.Remove (t.GetComponent<Turret> ());
				}
			}
		}
		bool allTurretsAtEdge = false;
		while (!allTurretsAtEdge) {
			allTurretsAtEdge = true;
			//check if all turrets are in position
			foreach (Transform t in children) {
				
				if (Vector2.Distance (new Vector2 (t.position.x, t.position.z), new Vector2 (center.position.x, center.position.z)) < finalDistance) {
					
					allTurretsAtEdge = false;
					t.Translate (new Vector3 (t.position.x - center.position.x, 0, t.position.z - center.position.z) * Time.deltaTime * removalSpeed, Space.World);

				} else {
					t.GetComponent<Turret> ().contestable = false;
				}
			}
			//if all turrets are not at the edge, translate them there

			yield return null;
		}
		StartCoroutine (OrbitTurrets ());
	}

	IEnumerator OrbitTurrets(){
		while (true) {
			transform.Rotate (Vector3.up * Time.deltaTime * orbitingSpeed, Space.World);
			yield return null;
		}
	}
}
