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

    public GameObject textPrefab;
    public Color enemyPlayerColor;
    public float hitStun;

	// Use this for initialization
	void Start () {
		render.material = playerColor;
		//Debug.Log (render.material);
		playerColor = normalColor;
		CurrentHealth = MaxHealth;
		//SetHealth ();
		takingDamage = false;



	}

	void Update ()
	{
		render.material = playerColor;
        GroundCheck();
        if (CurrentHealth <= 0f)
        {
			Die ();
        }
	}

	public void Die(){
		CurrentHealth = 0;
		Instantiate(explosionPrefab, transform.position, transform.rotation);
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
		//Debug.Log("dying");
		gameObject.SetActive(false);

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
			if (!steppedOffLedge) {
				Invoke ("Die", fallTimer);
				steppedOffLedge = true;
			}
		} else {
			CancelInvoke ();
			steppedOffLedge = false;
		}


    }


}
