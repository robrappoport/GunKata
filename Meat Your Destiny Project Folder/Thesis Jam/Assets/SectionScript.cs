using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionScript : MonoBehaviour {
    public Turret[] sectionTurret;
    public GameObject floor;
    private float dropSpeed;
    private Renderer floorRend;
    public float dropTotal;
    public float flashTime;

    public Material normColor;
    public Material flashColor;
    public Material deadColor;
    // Use this for initialization

    void Start()
    {
        dropSpeed = 15f;
        floorRend = floor.GetComponent<Renderer>();
        normColor = flashColor;
        floorRend.material = normColor;

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
        for (int i = 0; i < flashTime; i++)
        {
            normColor = flashColor;
            floorRend.material = normColor;

            yield return new WaitForSeconds(.1f);


            normColor = deadColor;
            floorRend.material = normColor;

            yield return new WaitForSeconds(.1f);

        }
        for (int i = 0; i < sectionTurret.Length; i++)
        {
            sectionTurret[i].withinTimerLimits = false;
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
