﻿using System.Collections;
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
        auraLifeTime = auraSize * 1000000;
        auraCurLife = 0;

    }

    public enum AuraType
    {
        slowdown, projection
    }
}

