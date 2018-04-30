using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamControl : MonoBehaviour {
    private Transform currentMount;
    public float speedFactor = 0.1f;
	// Use this for initialization
	
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, currentMount.position, speedFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentMount.rotation, speedFactor);
	}

    public void setMount(Transform newMount){
        currentMount = newMount;
    }
}
