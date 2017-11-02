using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShake : MonoBehaviour {

	public float amount;
	public float time;

	[ContextMenu("Test Shake")]
	public void TestCameraShake()
	{
		Camera.main.GetComponent<CameraMultitarget>().Shake(amount, time);
	}
}
