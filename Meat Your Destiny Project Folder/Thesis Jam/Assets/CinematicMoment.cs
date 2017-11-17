using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicMoment : MonoBehaviour {

    public CameraMultitarget thisCamera;
    public float cinematicDist;
    public float threshHold;
    bool cinematicMode;
    public float zoomSpeed;
    public float cameraAngle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float dist = (TwoDGameManager.thisInstance.players[0].transform.position - TwoDGameManager.thisInstance.players[1].transform.position).magnitude;
        if (dist <= cinematicDist - threshHold) {
            cinematicMode = true;
        } else if (dist >= cinematicDist + threshHold){
            cinematicMode = false;
        }
        float targetRot = cinematicMode ? cameraAngle : 0f;
        thisCamera.orbitRotation.x = Mathf.Lerp(thisCamera.orbitRotation.x, targetRot, zoomSpeed * Time.deltaTime);
		
	}
}
