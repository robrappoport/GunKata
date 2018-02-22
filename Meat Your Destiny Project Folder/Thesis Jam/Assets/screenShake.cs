using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenShake : MonoBehaviour {

    public float shakeTimer;
    public float shakeAmount;
    public float shakePow;
    public float shakeDuration;

    // Update is called once per frame
    void Update () {
		
        if (shakeTimer >= 0f)
        {
            Vector2 ShakePos = Random.insideUnitCircle * shakeAmount;

            transform.position = new Vector3(transform.position.x + ShakePos.x,
                                             transform.position.y + ShakePos.y,
                                             transform.position.z);
            shakeTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShakeCamera(shakePow, shakeDuration);
        }
	}

    public void ShakeCamera (float shakePwr, float shakeDur)
    {
        shakeAmount = shakePwr;
        shakeTimer = shakeDur;

    }
}
