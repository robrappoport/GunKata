using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShake : MonoBehaviour {

	public float amount;
	public float time;
    public CameraMultitarget cam;

    private void Start()
    {
        cam = GetComponent<CameraMultitarget>();   
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestCameraShake();
        }
    }
	public void TestCameraShake()
	{
        cam.Shake(amount, time);
	}
}
