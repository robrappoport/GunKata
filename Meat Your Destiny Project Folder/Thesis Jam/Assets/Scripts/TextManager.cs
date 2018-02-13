using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour {

    public float spd;
    public Color color;
    public string pointString;
    public TextMesh text;

	// Use this for initialization
	void Start () {
        text = GetComponent<TextMesh>();
        text.text = pointString;
        text.color = color;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(Camera.main.transform);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + 180, transform.eulerAngles.y, transform.eulerAngles.z + 180);
        transform.position += Vector3.up * spd;
        color.a -= .5f * Time.deltaTime;
        text.color = color;
        if (text.color.a < 0f) {
            Destroy(gameObject);
        }
	}
}
