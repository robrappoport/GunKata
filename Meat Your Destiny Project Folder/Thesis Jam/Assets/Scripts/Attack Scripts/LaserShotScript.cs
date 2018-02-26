using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotScript : MonoBehaviour {

    LineRenderer lineRend;
    public Transform startPos;
    public Transform endPos;
	public AuraCharacterController owner;
    public int ownerNumber;
    private float textureOffset = 0f;
    public bool on = false;
    public Vector3 endPosExtendedPos;
    public float extendDuration = 5f;
    public float lifeTime;
    public float lifeTimePassed;
    BoxCollider col;
    float timeOn;
	// Use this for initialization
	void Start () {

        timeOn = 0;
        lineRend = GetComponent<LineRenderer>();
        col = GetComponent<BoxCollider>();
		ownerNumber = owner.playerNum;
		Color myColor = Color.black;
		if (ownerNumber == 0) {
			myColor = Color.cyan;
		} else if (ownerNumber == 1) {
			myColor = Color.red;
		}
        float dist = Vector3.Distance(endPos.position, startPos.position);
        endPosExtendedPos = Vector3.forward * dist *( GetComponentInParent<auraGunBehavior>().wingMatChangeValue)/3;
        lifeTime = GetComponentInParent<auraGunBehavior>().totalLaserShotTime;
		float alpha = 1.0f;
		Gradient gradient = new Gradient ();
		gradient.SetKeys (
			new GradientColorKey[] {
				new GradientColorKey (myColor, 0.0f),
				new GradientColorKey (myColor, 1f)
			},
			new GradientAlphaKey[]{ new GradientAlphaKey (alpha, 0.0f), new GradientAlphaKey (alpha, 1.0f) }
		);
		lineRend.colorGradient = gradient;
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPos.position,1.0f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(endPos.position,1.0f);
    }

    // Update is called once per frame
    void Update () {

        adjustCollider();
        lifeTimePassed += Time.deltaTime;

        if (lifeTimePassed>= lifeTime)
        {Destroy(gameObject);}


        //extend the line//
        if (on)
        {
            timeOn += Time.deltaTime / extendDuration; //goes to 1.0 after 5 seconds
            timeOn = Mathf.Clamp01((timeOn));
//            Debug.Log("start position "+startPos.position);
//            Debug.Log("start local position " + startPos.localPosition);
//            Debug.Log("end position " + endPos.position);
//            Debug.Log("end local position " + endPos.localPosition);
            //endPos.localPosition = Vector3.Lerp(endPos.localPosition, endPosExtendedPos, Time.deltaTime * extendDuration);
            endPos.localPosition = Vector3.Lerp(startPos.localPosition, endPosExtendedPos, timeOn);    
           
        }
        //retract the line
        else
        {
            timeOn = 0f;
            //endPos.localPosition = Vector3.Lerp(endPos.localPosition, startPos.localPosition, Time.deltaTime * extendDuration);
            //if (endPos.localPosition == startPos.localPosition)
            //{
            //    Destroy(gameObject);
            //}
        }
        lineRend.SetPosition(0, startPos.position);
        lineRend.SetPosition(1, endPos.position);
        //pan the texture for cool effect//
        textureOffset -= Time.deltaTime * 2f;
        if (textureOffset < -10f)
        {
            textureOffset += 10f;
        }
        lineRend.sharedMaterials[1].SetTextureOffset("_MainTex", new Vector2(textureOffset, 0f));
	}

    private void adjustCollider()
    {
        
        col.transform.parent = lineRend.transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance(startPos.localPosition, endPos.localPosition); // length of line
        col.size = new Vector3(2f, 10f, lineLength); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos.localPosition + endPos.localPosition) / 2;
        col.center = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        //float angle = (Mathf.Abs(startPos.localPosition.y - endPos.localPosition.y) / Mathf.Abs(startPos.localPosition.x - endPos.localPosition.x));
        //if ((startPos.localPosition.y < endPos.localPosition.y && startPos.localPosition.x > endPos.localPosition.x) 
        //    || (endPos.localPosition.y < startPos.localPosition.y && endPos.localPosition.x > startPos.localPosition.x))
        //{
        //    angle *= -1;
        //}
        //angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        //col.transform.Rotate(0, 0, angle);
    }

    private void OnTriggerEnter(Collider collision)
	{
		Trigger (collision);
	}

	void OnTriggerStay(Collider collision){
		Trigger (collision);
	}
	private void Trigger(Collider collision){
		if (collision.gameObject) {
			if (collision.gameObject.GetComponent<Bullet> ()) {
				print ("destroying");
				Destroy (collision.gameObject);
			} else if (collision.gameObject.GetComponent<Cannonball> ()) {
				collision.gameObject.GetComponent<Cannonball> ().SelfDestruct ();
			} else if (collision.gameObject.GetComponent<auraPlayerHealth> ()) {
				collision.gameObject.GetComponent<auraPlayerHealth> ().takeDamage (-collision.gameObject.GetComponent<auraPlayerHealth> ().MaxHealth);
			}
		}
	
	}
}
