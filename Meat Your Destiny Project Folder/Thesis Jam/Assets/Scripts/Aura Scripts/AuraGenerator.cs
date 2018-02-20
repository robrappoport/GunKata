using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraGenerator : MonoBehaviour {
    public float auraScaleMin;
    private float auraScaleCurrent;
    public float auraGrowthRate;
	public float deformingForceModifier, deformingLimit;
    public int auraPlayerNum;
	public float auraLifeTime;
    public float auraCurLife;
    public AuraType auraType;
	public GameObject ps;

	// Use this for initialization
	void Start () {
        
	}

	void DeformMultiple(Collider col, int totalPoints = 6, float radiusFactor = 1, int iterations = 1){
		bool firstIteration;
		for(int i = 0; i<iterations; i++){
			if (i == 0) {
				firstIteration = true;
			} else {
				firstIteration = false;
			}
			Deform (col, totalPoints, radiusFactor / iterations * i, firstIteration);
		}
	}

	void Deform(Collider col, int totalPoints = 6, float radiusFactor = 1, bool firstIteration = true){
		Vector3 point = GetComponent<Collider> ().ClosestPointOnBounds (col.transform.position);
		List<Vector3> points = new List<Vector3> ();
		List<Vector3> hits = new List<Vector3> ();
		if (firstIteration) {
			points.Add (point);
		}


//		GameObject temp = new GameObject ("temp");
//		temp.transform.position = point;
//		temp.transform.LookAt (transform, transform.up);
		for (int i = 0; i <totalPoints; i++) {
			float l;
			if (col.GetComponent<SphereCollider>()){
				l = col.GetComponent<SphereCollider>().radius * radiusFactor;
			}else{
				l = col.bounds.size.x;
			}
				
			Vector3 newPoint = point + Quaternion.AngleAxis (360 / totalPoints * i, (transform.position - point) / Vector3.Distance (transform.position, point)) * (Vector3.right * l);
			points.Add (newPoint);
		}
		for (int i = 0; i < points.Count; i++) {
			RaycastHit hit;
			if (Physics.Raycast (points [i], (transform.position - col.transform.position) / Vector3.Distance (transform.position, col.transform.position), out hit,
				Vector3.Distance (transform.position, col.transform.position), ~LayerMask.NameToLayer("Aura"), QueryTriggerInteraction.Collide)) {
				hits.Add (hit.point);
			}
		}
		for (int i = 0; i < hits.Count; i++) {

			GetComponent<MeshDeformer> ().AddDeformingForce (hits[i],
				
				Mathf.Clamp(col.GetComponent<Rigidbody>().velocity.magnitude *  Vector3.Distance (hits[i], col.ClosestPoint(hits[i])) * deformingForceModifier, 0, deformingLimit ));
		}
	}
	// Update is called once per frame
	void Update () {
		if (auraCurLife >0.1f && !GetComponent<MeshDeformer> ()) {
			gameObject.GetComponent<MeshDeformer> ().enabled = true;
			GetComponent<MeshDeformer> ().damping = 2;
		}
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
		if (GetComponent<MeshDeformer> () && col.GetComponent<Rigidbody>()) {
			Deform (col, 6, .5f);
		}

		//		MakeParticles (col, true);
	}
	void OnTriggerExit(Collider col){
//		MakeParticles (col, false);
		if (GetComponent<MeshDeformer> () && col.GetComponent<Rigidbody>()) {
			Deform (col, 6, .5f);
		}
	
	}

//	void Deform(Collider col){
//		if (GetComponent<MeshDeformer> ()) {
//			GetComponent<MeshDeformer> ().AddDeformingForce (GetComponent<Collider> ().ClosestPointOnBounds (col.transform.position), -Mathf.Clamp(GetComponent<Rigidbody>().velocity.magnitude * deformingForceModifier, 5000, deformingLimit ));
//		}
//	}
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

