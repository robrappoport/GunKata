﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenScript : MonoBehaviour
{
    public static PlayControl instance;
    public InputDevice myController;

    public Text yesText, noText, winText;
    public int selectedText;//0 is start, 1 is controls
    public bool controlsActive, leftStickHeld;
    // Use this for initialization
    void Start()
    {
		instance = FindObjectOfType<PlayControl> ();
		
        selectedText = 0;
        controlsActive = false;
        myController = InputManager.Devices[0];
    }

    // Update is called once per frame
    void Update()
    {

        if (selectedText == 0)
        {
            yesText.color = Color.white;
            noText.color = Color.black;

        }
        else
        {
            yesText.color = Color.black;
            noText.color = Color.white;

        }
        if (myController.AnyButtonWasReleased)
        {

            if (selectedText == 1)
            {
                SceneManager.LoadScene("Start Screen");
				noText.color = Color.blue;
            }
            else
            {
                ///TODO: Replace when more levels are active
//              SceneManager.LoadScene ("LevelSelectScreen");
				yesText.color = Color.blue;

				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }


        if (!leftStickHeld)
        {
            if (myController.LeftStick.Y < 0)
            {
                selectedText = Mathf.Clamp(selectedText + 1, 0, 1);
                leftStickHeld = true;
            }
            else if (myController.LeftStick.Y > 0)
            {
                selectedText = Mathf.Clamp(selectedText - 1, 0, 1);
                leftStickHeld = true;

            }
        }

        if (myController.LeftStick.Y == 0)
        {
            leftStickHeld = false;
        }
    }
}
