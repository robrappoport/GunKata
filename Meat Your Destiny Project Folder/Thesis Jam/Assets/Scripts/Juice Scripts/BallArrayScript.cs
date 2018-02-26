using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallArrayScript : MonoBehaviour
{
    LineRenderer lineRend;
    private Vector3 startPos;
    private Vector3 currentEndPos;
    //public Turret owner;
    //public int ownerNumber;
    private float textureOffset = 0f;
    public bool on = false;
    public Vector3 endPosExtendedPos;
    public float extendDuration = 5f;
    float timeOn;
    // Use this for initialization
    void Start()
    {
        timeOn = 0;
        lineRend = GetComponent<LineRenderer>();
        startPos = gameObject.transform.position;
        currentEndPos = startPos;
        //endPos = transform.Find("EndPos");
        //float dist = Vector3.Distance(currentEndPos, startPos.position);
        endPosExtendedPos = TwoDGameManager.thisInstance.ballLoc;
        Debug.Log(endPosExtendedPos + "sphere pos");

    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            timeOn += Time.deltaTime / extendDuration; //goes to 1.0 after 5 seconds
            timeOn = Mathf.Clamp01((timeOn));
            currentEndPos = Vector3.Lerp(startPos, endPosExtendedPos, timeOn);
            //           
        }
        else
        {
            currentEndPos = Vector3.Lerp(currentEndPos, startPos, Time.deltaTime * extendDuration);
            timeOn = 0f;
        }
        lineRend.SetPosition(0, startPos);
        lineRend.SetPosition(1, currentEndPos);
        //pan the texture for cool effect//
        textureOffset -= Time.deltaTime * 2f;
        if (textureOffset < -10f)
        {
            textureOffset += 10f;
        }
        lineRend.sharedMaterials[1].SetTextureOffset("_MainTex", new Vector2(textureOffset, 0f));

    }

}

