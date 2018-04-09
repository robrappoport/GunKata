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
    public AudioClip fallingSound, rumbleSound;

   

    // Use this for initialization

    void Start()
    {
		if (turretCarrier) {
			foreach (Transform t in turretCarrier.children) {
				sectionTurret.Add (t.GetComponent<Turret> ());
			}
		}
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
     //   Sound.me.Play(rumbleSound);
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
            // Sound.me.Play(fallingSound);

        }

        for (int i = 0; i < sectionTurret.Count; i++)
        {
            sectionTurret[i].withinTimerLimits = false;
            sectionTurret[i].key = false;
        }
		if (turretCarrier) {
			turretCarrier.StartCoroutine (turretCarrier.SendToEdge ());
		}
		TwoDGameManager.thisInstance.readyToActivateNextSections = true;
        while (floor.transform.position.y > dropTotal)
        {
            floor.transform.position = new Vector3(floor.transform.position.x, 
                                                   floor.transform.position.y - (dropSpeed * Time.deltaTime),
                                                   floor.transform.position.z);
            yield return null;
        }
    }
}
