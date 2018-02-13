using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShakeController : MonoBehaviour {

	public float minJumpTime;
	public float maxJumpTime;
	public float jumpForce;
	float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer < 0)
		{
			timer = Random.Range(minJumpTime, maxJumpTime);
			GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);

		}
	}

	void OnCollisionEnter(Collision col)
	{
		if (timer > 0.5)
		{
			/// We test the shaking feature when we collide.
			float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
			float percent = 1 - (distance / 200);
			float amount = Mathf.Clamp(percent, 0f, 1f);
			float time = 0.5f;
			Debug.Log(distance  + " -> " + amount + " > " + percent);



			//Camera.main.GetComponent<CameraMultitarget>().Shake(amount, time);
		}
	}
}
