using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class auraPlayerHealth : MonoBehaviour {
	//public Image HealthBar;
	public float MaxHealth;
	public float CurrentHealth;
	public bool takingDamage;
	//public GameObject playerCanvas;
	public float damageTime;
	public Renderer render;
	public bool invincibilityFramesActive;
	public Material playerColor;
	public Material damagedColor;
	public Material normalColor;
    //public ParticleSystem standardHalo, DamagedHalo;
    public GameObject explosionPrefab;

	public int flashNum;
    public AudioClip damageSnd;

    public GameObject textPrefab;
    public Color enemyPlayerColor;

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
	}


	public void takeDamage (int amount)
	{
        if (!invincibilityFramesActive && CurrentHealth > 0) {
			takingDamage = true;
			
            //SetHealth ();
            CurrentHealth += amount;

            if (CurrentHealth > 0)
            {
                Sound.me.Play(damageSnd, 1f, true);
                invincibilityFramesActive = true;
                GetComponent<AuraCharacterController>().hitStunnedTimer = .2f;
                //standardHalo.Clear();
                //standardHalo.Stop();
                //DamagedHalo.Play();
                StartCoroutine(colorChange());
            }

			if (CurrentHealth <= 0f) {
                Instantiate(explosionPrefab, transform.position, transform.rotation);
                TextManager text = ((GameObject)Instantiate(textPrefab, transform.position, Quaternion.identity)).GetComponent<TextManager>();
                text.color = enemyPlayerColor;
                text.pointString = "100";
                Debug.Log("dying");
				gameObject.SetActive (false);
				//playerCanvas.gameObject.SetActive (false);
			}
		}
	}

	//public void SetHealth ()
	//{
	//	HealthBar.fillAmount = CurrentHealth*.01f;

	//}


	private IEnumerator colorChange()
	{
		for (int i = 0; i < flashNum; i++) {


			playerColor = damagedColor;

			yield return new WaitForSeconds (damageTime);



			playerColor = normalColor;

			yield return new WaitForSeconds (damageTime);

		}
		invincibilityFramesActive = false;
	}


}
