using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {
    public Transform city, space, heaven;
    public Material citySkybox, spaceSkybox, heavenSkybox;
	public static PlayControl instance;
	public InputDevice myController;
    public ParticleSystem p;

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
							selectedTextCounter = Mathf.Clamp (selectedTextCounter + 1, 0, levels.Length);
					leftStickHeld = true;
				} else if (myController.LeftStick.Y > 0) {
							selectedTextCounter = Mathf.Clamp (selectedTextCounter - 1, 0, levels.Length);
					leftStickHeld = true;

				}
			}
		}
		if (myController.LeftStick.Y == 0) {
			leftStickHeld = false;
            AssignCameraMount();
            p.Play();


		}
	}
    void AssignCameraMount(){
        switch (levels[selectedTextCounter].name)
        {
            case "City":
                menuCamControl.setMount(city);
                RenderSettings.skybox = citySkybox;
                break;
            case "Space":
                menuCamControl.setMount(space);
                RenderSettings.skybox = spaceSkybox;
                break;
            case "Heaven":
                menuCamControl.setMount(heaven);
                RenderSettings.skybox = heavenSkybox;
                break;
        }
    }
}
