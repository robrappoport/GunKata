using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraGenerator : MonoBehaviour {
    public float auraScaleMin;
    private float auraScaleCurrent;
    public float auraGrowthRate;
	public float deformingForceModifier;
    public int auraPlayerNum;
	public float auraLifeTime;
    public float auraCurLife;
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
            Destroy(gameObject);
        }
        if (auraCurLife >= auraLifeTime)
        {
            auraCurLife = auraLifeTime;
            Destroy(gameObject);
        }
        if (auraCurLife > (.98f * auraLifeTime))
        {

            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3 (gameObject.transform.localScale.x+1, gameObject.transform.localScale.y+1, gameObject.transform.localScale.z+1), auraCurLife/auraLifeTime);

        }
	}

	void OnTriggerEnter(Collider col){
		Deform (col);

		//		MakeParticles (col, true);
	}
	void OnTriggerExit(Collider col){
//		MakeParticles (col, false);
		Deform(col);
	
	}

	void Deform(Collider col){
		
		GetComponent<MeshDeformer> ().AddDeformingForce (GetComponent<Collider> ().ClosestPointOnBounds (col.transform.position), -deformingForceModifier);
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

