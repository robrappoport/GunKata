using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSpawnBehaviorScript : MonoBehaviour {
    public float initElapsedTime = 2f;
    public float secondElapsedTime = 3f;
    public GameObject obelisk;

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
        if (secondElapsedTime <= 0f)
        {
            Destroy(obelisk);
        }
        else
            obelisk.transform.Translate(Vector3.down * 30f * Time.deltaTime);
       
        
    }
}
