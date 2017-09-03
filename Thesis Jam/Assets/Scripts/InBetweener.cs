using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBetweener : MonoBehaviour {

	public GameObject object1;
	public GameObject object2;
	void Update()
	{
		this.transform.position = 0.5f*(object1.transform.position + object2.transform.position);
	}
}
