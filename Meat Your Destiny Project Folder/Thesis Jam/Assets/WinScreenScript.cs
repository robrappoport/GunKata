using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenScript : MonoBehaviour
{
    public static PlayControl instance;
    public int winnerNum;
    public MovieTexture[] movies;
    public InputDevice myController;
    public MovieTexture movie;
    public Text yesText, noText;
    public int selectedText;//0 is start, 1 is controls
    public bool controlsActive, leftStickHeld;
    // Use this for initialization
    void Start()
    {
		instance = FindObjectOfType<PlayControl> ();
        movie = movies[winnerNum];

        selectedText = 0;
        controlsActive = false;
        myController = InputManager.Devices[0];
        GetComponent<RawImage>().texture = movie;
        movie.Play();
        movie.loop = true;
        TwoDGameManager.thisInstance.TogglePlayerControl();

    }

    // Update is called once per frame
    void Update()
    {
        if (selectedText == 0)
        {
            yesText.color = Color.yellow;
            noText.color = Color.white;

        }
        else
        {
            yesText.color = Color.white;
            noText.color = Color.yellow;

        }
        if (myController.AnyButtonWasReleased)
        {

            if (selectedText == 1)
            {
                SceneManager.LoadScene ("LevelSelectScreen");
                noText.color = Color.white;
            }
            else
            {
                yesText.color = Color.white;

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
