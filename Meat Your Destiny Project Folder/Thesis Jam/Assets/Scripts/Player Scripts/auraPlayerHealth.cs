using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class auraPlayerHealth : MonoBehaviour {
	//public Image HealthBar;
	public float MaxHealth;
	public float CurrentHealth;
	public bool takingDamage;
    public float groundCheckHeight;
	//public GameObject playerCanvas;
	public float damageTime, fallTimer = 1;
	public Renderer render;
	public bool invincibilityFramesActive, steppedOffLedge = false;
	public Material playerColor;
	public Material damagedColor;
	public Material normalColor;
    //public ParticleSystem standardHalo, DamagedHalo;
    public GameObject explosionPrefab;
    public GameObject followParticles;

	public int flashNum;
    public AudioClip damageSnd;
    public AudioClip deathSnd, dyingSound;
    public GameObject textPrefab;
	public GameObject deathBeamPrefab;
    public Color enemyPlayerColor;
    public float hitStun;
	public bool dying = false, dead= false;
	public float deathTime = 5;
	Animator anim;
    public bool gameIsOver = false;
	GameObject deathBeam;
	// Use this for initialization
	void Start () {
		dead = false;
		render.material = playerColor;
		//Debug.Log (render.material);
		playerColor = normalColor;
		CurrentHealth = MaxHealth;
		//SetHealth ();
		takingDamage = false;
		anim = GetComponent<Animator> ();


	}

	void Update ()
	{
		render.material = playerColor;
        GroundCheck();
		if (!dying) {
			if (CurrentHealth <= 0f) {
				
				StartDying ();
			}
		} 

	
	}
	void StartDying(){
		dying = true;
      //  Sound.me.Play(dyingSound);
		anim.SetTrigger ("Die");
        Invoke ("Die", deathTime);
        Invoke("Shake", 0);

	}
	public void Die(){

		dead = true;

		//Instantiate(explosionPrefab, transform.position, transform.rotation);
		ParticleFollowScript followParts = ((GameObject)Instantiate
			(followParticles, transform.position, 
				Quaternion.identity)).GetComponent<ParticleFollowScript>();
      		if (gameObject.GetComponent<auraGunBehavior>().playerNum == 0)
		{
			followParts.owner = 1;
		}
		else
		{
			followParts.owner = 0;
		}
        Sound.me.Play(deathSnd, .4f, true);
        deathBeam = Instantiate(deathBeamPrefab, transform) as GameObject;
        deathBeam.GetComponentInChildren<Renderer>().material.color = TwoDGameManager.thisInstance.playerColors[GetComponent<auraGunBehavior>().playerNum];

		deathBeam.transform.SetParent(null);
		deathBeam.transform.position = transform.position;
		deathBeam.transform.localScale = Vector3.one;

   		gameObject.SetActive(false);
        Destroy(deathBeam, 1f);

	}

    void Shake(){
        StartCoroutine(ShakeMeScript.Shake(Camera.main.transform.root, 4f, 1.2f, true, true, true));

    }

	public void takeDamage (float amount)
	{
        if (!invincibilityFramesActive && CurrentHealth > 0) {
			takingDamage = true;
            //SetHealth ();
            CurrentHealth += amount;

            if (CurrentHealth > 0)
            {
                Sound.me.Play(damageSnd, 1f, true);
                GetComponent<AuraCharacterController>().hitStunnedTimer = hitStun;
                //standardHalo.Clear();
                //standardHalo.Stop();
                //DamagedHalo.Play();
				StartCoroutine(colorChange(damageTime, flashNum));
            }

			//if (CurrentHealth <= 0f) {
   //             Instantiate(explosionPrefab, transform.position, transform.rotation);
   //             TextManager text = ((GameObject)Instantiate(textPrefab, transform.position, Quaternion.identity)).GetComponent<TextManager>();
   //             text.color = enemyPlayerColor;
   //             text.pointString = "100";
   //             Debug.Log("dying");
			//	gameObject.SetActive (false);
			//	//playerCanvas.gameObject.SetActive (false);
			//}
		}
	}

	//public void SetHealth ()
	//{
	//	HealthBar.fillAmount = CurrentHealth*.01f;

	//}
	void Fall(){
        Die();
	}

	public IEnumerator colorChange(float damageTime, float flashNum)
	{
		invincibilityFramesActive = true;

		for (int i = 0; i < flashNum; i++) {


			playerColor = damagedColor;

			yield return new WaitForSeconds (damageTime);



			playerColor = normalColor;

			yield return new WaitForSeconds (damageTime);

		}
		invincibilityFramesActive = false;
	}

    private void GroundCheck ()
    {
        Vector3 groundCheck = transform.TransformDirection(Vector3.down);

		if (!Physics.Raycast (transform.position, groundCheck, groundCheckHeight)) {
			//Debug.Log("hello?");
			if (!steppedOffLedge && !gameIsOver) {
				Invoke ("Fall", fallTimer);
				steppedOffLedge = true;
			}
            anim.SetBool("Falling", true);

		} else {
			CancelInvoke ("Fall");
			steppedOffLedge = false;
            anim.SetBool("Falling", false);

		}


    }


}
