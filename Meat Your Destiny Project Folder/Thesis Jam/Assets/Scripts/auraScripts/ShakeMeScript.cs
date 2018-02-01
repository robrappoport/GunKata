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
            Vector3 newPos = Random.insideUnitSphere * (shakeAmount);

            transform.position += newPos;
            newPos.y = originalPos.y;
        }
	}
    public void ShakeMe()
    {
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
