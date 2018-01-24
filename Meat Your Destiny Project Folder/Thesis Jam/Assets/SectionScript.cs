using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionScript : MonoBehaviour {
    public Turret[] sectionTurret;
    public GameObject floor;
    private float dropSpeed;
    public float dropTotal;
    // Use this for initialization

    void Start()
    {
        dropSpeed = 15f;
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void Drop ()
    {
        StartCoroutine(DropCo());
    }

    public IEnumerator DropCo ()
    {
        for (int i = 0; i < sectionTurret.Length; i++)
        {
            //sectionTurret[i].LockTurret();
        }
        while (floor.transform.position.y > dropTotal)
        {
            floor.transform.position = new Vector3(floor.transform.position.x, 
                                                   floor.transform.position.y - (dropSpeed * Time.deltaTime),
                                                   floor.transform.position.z);
            yield return null;
        }
    }
}
