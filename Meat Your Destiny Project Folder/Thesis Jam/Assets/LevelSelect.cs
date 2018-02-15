using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {
	public static PlayControl instance;
	public InputDevice myController;

	public Text[] levels;
	public GameObject controls;
	public int selectedText;//0 is start, 1 is controls
	public bool controlsActive, leftStickHeld;
	// Use this for initialization
	void Start () {
		selectedText = 0;
		controlsActive = false;
		myController = InputManager.Devices [0];
	}

	// Update is called once per frame
	void Update () {

	
		for (int i =0; i < levels.Length; i++) {
			if (i == selectedText) {
				levels [i].color = Color.white;
			}
				else{
					levels[i].color = Color.black;
				}
			}


		if(myController.AnyButtonWasReleased){

			if(Application.CanStreamedLevelBeLoaded( levels[selectedText].name)){
				SceneManager.LoadScene (levels [selectedText].name);	
			
			} else {
				SceneManager.LoadScene ("AuraVersion");

			}
		}

		if (!controlsActive) {
			if (!leftStickHeld) {
				if (myController.LeftStick.Y < 0) {
							selectedText = Mathf.Clamp (selectedText + 1, 0, levels.Length);
					leftStickHeld = true;
				} else if (myController.LeftStick.Y > 0) {
							selectedText = Mathf.Clamp (selectedText - 1, 0, levels.Length);
					leftStickHeld = true;

				}
			}
		}
		if (myController.LeftStick.Y == 0) {
			leftStickHeld = false;
		}
	}
}
