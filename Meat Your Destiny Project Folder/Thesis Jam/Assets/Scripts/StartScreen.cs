using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour {
	public static PlayControl instance;
	public InputDevice myController;

	public Text startText, controlsText;
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


		if (selectedText== 0) {
            startText.color = Color.yellow;
            controlsText.color = Color.white;

		} else {
            startText.color = Color.white;
            controlsText.color = Color.yellow;

		}
		if(myController.AnyButtonWasReleased){

			if (selectedText == 1) {
				if (!controlsActive) {
					controlsActive = true;
					controls.SetActive (true);
				} else {
					controlsActive = false;
					controls.SetActive (false);
				}
			} else {
				SceneManager.LoadScene ("LevelSelectScreen");
               // SceneManager.LoadScene("Heaven");
			}
		}

		if (!controlsActive) {
			if (!leftStickHeld) {
				if (myController.LeftStick.Y < 0) {
					selectedText = Mathf.Clamp (selectedText + 1, 0, 1);
					leftStickHeld = true;
				} else if (myController.LeftStick.Y > 0) {
					selectedText = Mathf.Clamp (selectedText - 1, 0, 1);
					leftStickHeld = true;

				}
			}
		}
		if (myController.LeftStick.Y == 0) {
			leftStickHeld = false;
		}
	}
}
