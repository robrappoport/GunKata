using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionScript : MonoBehaviour {
	public TurretCarrier turretCarrier;
	public List<Turret> sectionTurret = new List<Turret>();
    public GameObject floor;
    public ShakeMeScript shake;
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
        //if (Input.GetKeyDown(KeyCode.Space)){
        //    shake.ShakeMe();
        //}
       
	}

    public void Drop ()
    {
        StartCoroutine(DropCo());
        shake.ShakeMe();
    }

    public IEnumerator DropCo ()
    {
       
        for (int i = 0; i < flashTime; i++)
        {
            normColor = flashColor;
            floorRend.material = normColor;
          

            yield return new WaitForSeconds(.5f);


            normColor = deadColor;
            floorRend.material = normColor;

            yield return new WaitForSeconds(.5f);

        }

        for (int i = 0; i < sectionTurret.Count; i++)
        {
            sectionTurret[i].withinTimerLimits = false;
            sectionTurret[i].key = false;
        }
		turretCarrier.StartCoroutine (turretCarrier.SendToEdge ());

        while (floor.transform.position.y > dropTotal)
        {
            floor.transform.position = new Vector3(floor.transform.position.x, 
                                                   floor.transform.position.y - (dropSpeed * Time.deltaTime),
                                                   floor.transform.position.z);
            yield return null;
        }
    }
}
