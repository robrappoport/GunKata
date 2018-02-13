using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletManagerManager : MonoBehaviour {
    public BulletManager bMan1;
    public BulletManager bMan2;
    public float slowTime;
    public float slowDist;
	void Start () {
        
		
	}
	
	// Update is called once per frame
	void Update () {
		bool isSlow = false;
        for (int i = 0; i < bMan1.bulletList.Count; i++)

        {
            Bullet bullet = bMan1.bulletList[i];
            if (bullet.inactiveTime <= 0 && !(bullet.player1AuraTriggered || bullet.player2AuraTriggered))
            {
                if (Vector3.Distance(bullet.transform.position, bMan1.transform.position) < slowDist ||
                    Vector3.Distance(bullet.transform.position, bMan2.transform.position) < slowDist)
                {
                    isSlow = true;
                    break;
                }
            }
			


        }

		for (int i = 0; i < bMan2.bulletList.Count; i++)

		{
			Bullet bullet = bMan2.bulletList[i];
            if (bullet.inactiveTime <= 0 && !(bullet.player1AuraTriggered || bullet.player2AuraTriggered))
            {
                if (Vector3.Distance(bullet.transform.position, bMan1.transform.position) < slowDist && bullet.inactiveTime <= 0 ||
                    Vector3.Distance(bullet.transform.position, bMan2.transform.position) < slowDist && bullet.inactiveTime <= 0)
                {
                    isSlow = true;
                    break;
                }
            }



		}
		if (isSlow)
        {
			Time.timeScale = slowTime;

		}
        else
        {
            Time.timeScale = 1;
        }

    }
}
