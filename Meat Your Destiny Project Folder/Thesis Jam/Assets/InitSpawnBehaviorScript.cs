using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSpawnBehaviorScript : MonoBehaviour {
    public float initElapsedTime = 2f;
    public float secondElapsedTime = 3f;

	// Update is called once per frame
	void Update () {
        if (initElapsedTime <= 0f)
        {
            SpawnSink();
            secondElapsedTime -= Time.deltaTime;
        }
        else
        {
            initElapsedTime -= Time.deltaTime;
        }
	}

    void SpawnSink()
    {
        if (secondElapsedTime >= 0f)
        {
            Debug.Log("hello");
            transform.Translate(Vector3.down * Time.deltaTime * 30f, Space.World);
        }
        else
            Destroy(transform.GetComponentInChildren<Transform>().gameObject);
        
    }
}
