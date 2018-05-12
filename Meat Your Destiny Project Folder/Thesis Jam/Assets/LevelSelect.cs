using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {
    public Transform city, space, heaven;
    public Material citySkybox, spaceSkybox, heavenSkybox;
    public AudioSource cityAudio, spaceAudio, heavenAudio;
    public AudioClip stickSound;
    public AudioSource stickAudio;
	public static PlayControl instance;
	public InputDevice myController;
    //public ParticleSystem p;

	public Text[] levels;
	public GameObject controls;
	public int selectedTextCounter;
	public bool controlsActive, leftStickHeld;
    private MenuCamControl menuCamControl;
	// Use this for initialization
	void Start () {
		selectedTextCounter = 2;
		controlsActive = false;
		myController = InputManager.Devices [0];
        menuCamControl = Camera.main.GetComponent<MenuCamControl>();
	}

	// Update is called once per frame
	void Update () {

	
		for (int i =0; i < levels.Length; i++) {
			if (i == selectedTextCounter) {
                levels [i].color = Color.yellow;
			}
				else{
                levels[i].color = Color.white;
				}
			}


        if(myController.Action1.WasPressed){

			if(Application.CanStreamedLevelBeLoaded( levels[selectedTextCounter].name)){
                SceneManager.LoadScene (levels [selectedTextCounter].name);
			
			} else {
				SceneManager.LoadScene ("AuraVersion");

			}
		}

        if(myController.Action2.WasPressed){
            SceneManager.LoadScene("Start Screen");
        }

		if (!controlsActive) {
			if (!leftStickHeld) {
				if (myController.LeftStick.Y < 0) {
							selectedTextCounter = Mathf.Clamp (selectedTextCounter + 1, 0, levels.Length-1);
                    if (!stickAudio.isPlaying)
                    {
                        stickAudio.PlayOneShot(stickSound);
                    }
					leftStickHeld = true;
				} else if (myController.LeftStick.Y > 0) {
							selectedTextCounter = Mathf.Clamp (selectedTextCounter - 1, 0, levels.Length+1);
                    if (!stickAudio.isPlaying)
                    {
                        stickAudio.PlayOneShot(stickSound);
                    }
					leftStickHeld = true;

				}
			}
		}
		if (myController.LeftStick.Y == 0) {
			leftStickHeld = false;
            AssignCameraMount();
		}
       
	}
    void AssignCameraMount(){
        switch (levels[selectedTextCounter].name)
        {
            case "City":
                menuCamControl.setMount(city);
                RenderSettings.skybox = citySkybox;
                StartCoroutine(cityAudioSwitch());

                break;
            case "Space":
                menuCamControl.setMount(space);
                RenderSettings.skybox = spaceSkybox;
                StartCoroutine(spaceAudioSwitch());

                break;
            case "Heaven":
                menuCamControl.setMount(heaven);
                RenderSettings.skybox = heavenSkybox;
                StartCoroutine(heavenAudioSwitch());
                break;
        }
    }

    IEnumerator cityAudioSwitch()
    {
        float totalTime = 1f;
        float elapsedTime = 0;
        StopAllCoroutines();
        heavenAudio.Stop();
        if (spaceAudio.isPlaying)
        {
            //Debug.Log("debug city");
            if (!cityAudio.isPlaying)
            {
                cityAudio.Stop();
                cityAudio.Play();
                cityAudio.volume = 0f;
            }
            while (cityAudio.volume < 1)
            {
                //Debug.Log(elapsedTime);
                elapsedTime += Time.deltaTime;
                spaceAudio.volume = Mathf.Lerp(spaceAudio.volume, 0f, elapsedTime / totalTime);
                cityAudio.volume = Mathf.Lerp(cityAudio.volume, 1f, elapsedTime / totalTime);
                yield return 0;
            }
            //spaceAudio.Stop();
           
        }

    }
    IEnumerator spaceAudioSwitch()
    {
        float totalTime = 1f;
        float elapsedTime = 0;
        StopAllCoroutines();

        if (cityAudio.isPlaying)
        {
            heavenAudio.Stop();
            //Debug.Log("debug space");
            if (!spaceAudio.isPlaying)
            {
                spaceAudio.Stop();
                spaceAudio.Play();
                spaceAudio.volume = 0f;
            }
            while (spaceAudio.volume < 1)
            {
                //Debug.Log(elapsedTime);
                elapsedTime += Time.deltaTime;
                cityAudio.volume = Mathf.Lerp(cityAudio.volume, 0f, elapsedTime / totalTime);
                spaceAudio.volume = Mathf.Lerp(spaceAudio.volume, 1f, elapsedTime / totalTime);
                yield return 0;
            }
            cityAudio.Stop();
                }
        if (heavenAudio.isPlaying)
        {
            cityAudio.Stop();
            //Debug.Log("debug space2");
            if (!spaceAudio.isPlaying)
            {
                spaceAudio.Stop();
                spaceAudio.Play();
                spaceAudio.volume = 0f;
            }
            while (spaceAudio.volume < 1)
            {
                //Debug.Log(elapsedTime);
                elapsedTime += Time.deltaTime;
                heavenAudio.volume = Mathf.Lerp(heavenAudio.volume, 0f, elapsedTime / totalTime);
                spaceAudio.volume = Mathf.Lerp(spaceAudio.volume, 1f, elapsedTime / totalTime);
                yield return 0;
            }
            heavenAudio.Stop();
        }
    }
    IEnumerator heavenAudioSwitch()
    {
       
        float totalTime = 1f;
        float elapsedTime = 0;
        StopAllCoroutines();
        cityAudio.Stop();

        if (spaceAudio.isPlaying)
        {
            //Debug.Log("debug heaven");
            if (!heavenAudio.isPlaying)
            {
                heavenAudio.Stop();
                heavenAudio.Play();
                heavenAudio.volume = 0f;
            }
            while (heavenAudio.volume < 1)
            {
                //Debug.Log(elapsedTime);
                elapsedTime += Time.deltaTime;
                spaceAudio.volume = Mathf.Lerp(spaceAudio.volume, 0f, elapsedTime / totalTime);
                heavenAudio.volume = Mathf.Lerp(heavenAudio.volume, 1f, elapsedTime / totalTime);
                yield return 0;
            }
            spaceAudio.Stop();
                }
    }
}
