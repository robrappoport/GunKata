using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraGenerator : MonoBehaviour {
    public float auraScaleMin;
    private float auraScaleCurrent;
    public float auraGrowthRate;
    public int auraPlayerNum;
	public float auraLifeTime;
    private float auraCurLife;
    private Vector3 auraSizeMax;
    public AuraType auraType;
	public GameObject ps;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        auraCurLife = Mathf.Clamp(auraCurLife, 0, auraLifeTime);
        auraCurLife += Time.deltaTime;
//        Debug.Log(auraCurLife);
        if (auraCurLife <= 0)
        {
            auraCurLife = 0;
            Destroy(gameObject,2f);
        }
        if (auraCurLife >= auraLifeTime)
        {
            auraCurLife = auraLifeTime;
        }
        if (auraCurLife > (.5f * auraLifeTime))
        {

            gameObject.transform.localScale = Vector3.Lerp((auraSizeMax), new Vector3 (0,0,0), auraCurLife/auraLifeTime);

            gameObject.transform.localScale = Vector3.Lerp(auraSizeMax, new Vector3 (0,0,0), auraCurLife/auraLifeTime);
			if (transform.localScale.magnitude <= 1) {
				Destroy (gameObject);
			}

        }
	}

	void OnTriggerEnter(Collider col){
		MakeParticles (col, true);
	}
	void OnTriggerExit(Collider col){
		MakeParticles (col, false);

	}
	void MakeParticles(Collider col, bool entering){
		if (col.GetComponent<Bullet>() || col.GetComponent<Cannonball> ()) {
			
			GameObject newPs = Instantiate (ps, col.transform.position, col.transform.rotation, null) as GameObject;
			newPs.transform.LookAt(gameObject.transform);
			if (entering) {
				newPs.transform.Rotate (0, 180, 0);
			}
			Invoke ("DestroyParticles (newPs)", newPs.GetComponent<ParticleSystem> ().main.duration);
		}

	}
	void DestroyParticles(GameObject oldPs){
		Destroy (oldPs);
	}
   public void Init (int playerNum, float auraSize)
    {
        auraPlayerNum = playerNum;
        auraScaleCurrent = auraSize;

        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.localScale *= (auraSize * 100);
        auraSizeMax = gameObject.transform.localScale;
        //if (auraPlayerNum == 0)
        //{
        //    gameObject.tag = "player1Aura";
        //}
        //if (auraPlayerNum == 1)
        //{
        //    gameObject.tag = "player2Aura";
        //}
        auraLifeTime = auraSize * 10;
        auraCurLife = 0;

    }

    public enum AuraType
    {
        slowdown, projection
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("we done had a collision");
    }
}

