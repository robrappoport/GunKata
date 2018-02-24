using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBallScript : MonoBehaviour {
    public float health, rechargeSpeed, maxHealth, rechargePause;
    bool recharging;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (recharging)
        {
            health = Mathf.Clamp(health + Time.deltaTime * rechargeSpeed, 0, maxHealth);
        }
	}

    void Recharge(){
        recharging = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Bullet>())
        {
            Bullet bullet = other.GetComponent<Bullet>();
            CancelInvoke();
            Invoke("Recharge()", rechargePause);
            health += bullet.BulletDmg;

            if(health <= 0){
                Die(bullet.ownerNumber);
            }
        }
    }


    void Die(int playerNum){
        CancelInvoke();
        print("I, the ball, am dead!");
        auraGunBehavior killingPlayer = TwoDGameManager.thisInstance.GetPlayer(playerNum).GetComponent<auraGunBehavior>();
        if(killingPlayer){
            killingPlayer.superReady = true;
        }
        health = maxHealth;
        TwoDGameManager.thisInstance.OnBallDestroyed(playerNum);
        gameObject.SetActive(false);
    }
}
