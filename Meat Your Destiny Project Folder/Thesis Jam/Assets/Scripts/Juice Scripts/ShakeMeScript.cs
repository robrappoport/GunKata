using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeMeScript : MonoBehaviour {

    public bool shaking = false;
    [SerializeField]
    private float shakeAmount;
    [SerializeField]
    private float shakeTime;
    private SectionScript section;
    //public GameObject thingThatShakes;

	// Use this for initialization
	void Start () {
        section = GetComponent<SectionScript>();
	}
	
	// Update is called once per frame
	void Update () {
        if (shaking)
        {
            Vector3 originalPos = transform.position;
            Vector3 newPos = Random.insideUnitCircle * (shakeAmount);

            originalPos += newPos;
            newPos.x = originalPos.x;
            newPos.z = originalPos.z;
            transform.position = originalPos;
        }
	}

    public static IEnumerator ShakeMe2D(RectTransform r, float shakeMagnitude = .25f, float shakeTime = 0.5f){
        Vector2 originalPos = r.anchoredPosition;
        float elapsedTime = 0;
        while(elapsedTime < shakeTime){
            elapsedTime += Time.deltaTime;
            r.anchoredPosition = r.anchoredPosition + Random.insideUnitCircle * shakeMagnitude;
            yield return null;

        }
        elapsedTime = 0;
        while(elapsedTime < .1f){
            elapsedTime += Time.deltaTime;
            r.anchoredPosition = Vector2.Lerp(r.anchoredPosition, originalPos, elapsedTime / .1f);
            yield return null;
        }

    }

    public void ShakeMe(){
        StartCoroutine(ShakeNow());
    }
    IEnumerator ShakeNow()
    {
        Vector3 originalPos = transform.position;
        if (!shaking)
        {
            shaking = true;
        }

        yield return new WaitForSeconds(shakeTime);
        shaking = false;
        transform.position = originalPos;
    }
}
