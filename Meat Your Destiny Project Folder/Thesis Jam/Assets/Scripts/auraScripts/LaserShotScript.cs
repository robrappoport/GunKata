using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotScript : MonoBehaviour {

    LineRenderer lineRend;
    public Transform startPos;
    public Transform endPos;

    private float textureOffset = 0f;
    public bool on = false;
    public Vector3 endPosExtendedPos;
    public float extendDuration = 5f;
    BoxCollider col;

	// Use this for initialization
	void Start () {
        lineRend = GetComponent<LineRenderer>();
        endPosExtendedPos = endPos.localPosition;
        col = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {

        adjustCollider();

        //extend the line//
        if (on)
        {
            endPos.localPosition = Vector3.Lerp(endPos.localPosition, endPosExtendedPos, Time.deltaTime * extendDuration);
         
                
           
        }
        //retract the line
        else
        {
            endPos.localPosition = Vector3.Lerp(endPos.localPosition, startPos.localPosition, Time.deltaTime * extendDuration);
        

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
        col.size = new Vector3(lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
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
}
