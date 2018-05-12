using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 
AuraGenerator : MonoBehaviour {
    public float auraScaleMin;
    public float auraScaleCurrent;
    public float auraGrowthRate;
	public float deformingForceModifier, deformingLimit;
    public int auraPlayerNum;
	public float auraLifeTime;
    public float auraCurLife;
    public AuraType auraType;
	public GameObject ps;
    public bool isSuper;
    ParticleSystem p;
    ParticleSystem.MainModule auraParticles;
	Renderer renderer;
	Color initParticleColor;
	Color initMainColor;
	Color particlesTempColor;
	Color mainTempColor;
	bool fading = false;

	// Use this for initialization

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


		for (int i = 0; i <totalPoints; i++) {
			float l;
			if (col.GetComponent<SphereCollider>()){
				l = col.GetComponent<SphereCollider>().radius * radiusFactor;
			}else{
				l = col.bounds.size.x;
			}
				
			Vector3 newPoint = point + Quaternion.AngleAxis (360 / totalPoints * i, (transform.position - point) / Vector3.Distance (transform.position, point)) * (Vector3.right * l);
			points.Add (newPoint);
			if (i <= points.Count) {
				RaycastHit hit;
				if (Physics.Raycast (points [i], (transform.position - col.transform.position) / Vector3.Distance (transform.position, col.transform.position), out hit,
					Vector3.Distance (transform.position, col.transform.position), ~LayerMask.NameToLayer("Aura"), QueryTriggerInteraction.Collide)) {
					hits.Add (hit.point);
				}
			}
			if (i < hits.Count) {

				GetComponent<MeshDeformer> ().AddDeformingForce (hits [i],

					Mathf.Clamp (col.GetComponent<Rigidbody> ().velocity.magnitude * Vector3.Distance (hits [i], col.ClosestPoint (hits [i])) * deformingForceModifier, 0, deformingLimit));
			}			

		}
//		for (int i = 0; i < points.Count; i++) {
//		}
//		for (int i = 0; i < hits.Count; i++) {
//
//		}
	}



	// Update is called once per frame
	void Update () {
		if (auraCurLife >0.1f && !GetComponent<MeshDeformer> ()) {
			gameObject.GetComponent<MeshDeformer> ().enabled = true;
			GetComponent<MeshDeformer> ().damping = 2;
		}
        auraCurLife = Mathf.Clamp(auraCurLife, 0, auraLifeTime);

        if (transform.parent == null) { 
            auraCurLife += Time.deltaTime; 
        }
//        Debug.Log(auraCurLife);

        if (auraCurLife >= auraLifeTime)
        {
            auraCurLife = auraLifeTime;
            Destroy(gameObject);
        }
        if (!isSuper){
            if (auraCurLife > (.85f * auraLifeTime))
            {
				if(!fading){
					StartCoroutine(Fade(auraLifeTime - auraCurLife));
				}
				fading = true;
                
                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(gameObject.transform.localScale.x + 5, gameObject.transform.localScale.y + 5, gameObject.transform.localScale.z + 5), auraCurLife / auraLifeTime);
				auraParticles.startColor = Color.Lerp(initParticleColor, particlesTempColor, auraCurLife / auraLifeTime);
				renderer.material.color = Color.Lerp(initMainColor, mainTempColor, auraCurLife / auraLifeTime);
				print(auraParticles.startColor.color.a);
            }
        }
        else
        {
            if (auraCurLife > (.30f * auraLifeTime))
            {
                
                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(0,0,0), auraCurLife / auraLifeTime);

            } 
        }


	}

	IEnumerator Fade(float totalTime){
		float elapsedTime = 0;
		float elapsedRatio;
		while(elapsedTime < totalTime){
			elapsedRatio = elapsedTime / totalTime;
			gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(gameObject.transform.localScale.x + 5, gameObject.transform.localScale.y + 5, gameObject.transform.localScale.z + 5), elapsedRatio);
			auraParticles.startColor = Color.Lerp(initParticleColor, particlesTempColor, elapsedRatio);
			renderer.material.color = Color.Lerp(initMainColor, mainTempColor, elapsedRatio);

			elapsedTime += Time.deltaTime;
			yield return null;

		}
		
	}

//	void OnTriggerEnter(Collider col){
//		if (GetComponent<MeshDeformer> () && col.GetComponent<Rigidbody>()) {
//		//	Deform (col, 6, .5f);
//		}

//		//		MakeParticles (col, true);
//	}
//	void OnTriggerExit(Collider col){
////		MakeParticles (col, false);
	//	if (GetComponent<MeshDeformer> () && col.GetComponent<Rigidbody>()) {
	//	//	Deform (col, 6, .5f);
	//	}
	
	//}

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
    public void SetLifeTime(float auraSize){
        auraLifeTime = Mathf.Clamp(auraSize, 2, 5);

    }
   public void Init (int playerNum, float auraSize)
    {
        auraPlayerNum = playerNum;
        auraScaleCurrent = auraSize;

       // gameObject.transform.localScale = new Vector3(1, 1, 1);
        //gameObject.transform.localScale *= (auraSize * 100);
        //if (auraPlayerNum == 0)
        //{
        //    gameObject.tag = "player1Aura";
        //}
        //if (auraPlayerNum == 1)
        //{
        //    gameObject.tag = "player2Aura";
        //}
        SetLifeTime(auraSize);
        auraCurLife = 0;

        p = GetComponentInChildren<ParticleSystem>();
        auraParticles = p.main;
        GetComponent<Renderer>().material.color = TwoDGameManager.thisInstance.playerColors[auraPlayerNum];
        auraParticles.startColor = TwoDGameManager.thisInstance.playerColors[auraPlayerNum];

		initParticleColor = auraParticles.startColor.color;
		particlesTempColor = initParticleColor;
        particlesTempColor.a = 0;
		renderer = GetComponent<Renderer>();
		initMainColor = renderer.material.color;
		mainTempColor = initMainColor;
		mainTempColor.a = 0;


    }
	public static Collider GetCurrentAura(List<Collider> colliders, Collider otherCollider){
		//remove any missing refs from the list; aura no longer exists
		//if there is only one left in the list, it becomes the aura by default
		switch (colliders.Count) {
		case 1:
                if (otherCollider.bounds.Intersects(colliders[0].bounds))
                {
                    return colliders[0];
                }
                else
                {
                    return null;
                }
		case 0:
			return null;
		default:
			Collider finalCol = colliders [0];
                for (int i = 0; i < colliders.Count; i++)
                {
                    if (colliders[i].GetComponent<AuraGenerator>().auraScaleCurrent >= finalCol.GetComponent<AuraGenerator>().auraScaleCurrent)
                    {
                        finalCol = colliders[i];
                    }
                }
                if (otherCollider.bounds.Intersects(finalCol.bounds))
                {
                    return finalCol;
                }else{
                    return null;
                }
		}
	}

    public enum AuraType
    {
        slowdown, projection
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("we done had a collision");
    //}
}

