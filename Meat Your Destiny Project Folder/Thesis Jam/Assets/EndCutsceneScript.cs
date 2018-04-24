using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndCutsceneScript : MonoBehaviour {
    public Transform winner;
    private int winnerNum;
    public float timeToEnd;
    private float timeElapsed = 0;
    public float fadeTime;
    public Image fadeImg;
    private bool fading;

	private void Start()
	{
        
	}

	private void Update()
	{
        if (winner != null)
        {
            if (Vector3.Distance(winner.transform.position, Camera.main.transform.position) >= 20f)
            {
                timeElapsed += Time.deltaTime;
                winner.transform.position = Vector3.Lerp(winner.transform.position, Camera.main.transform.position, Easing.QuadEaseIn(timeElapsed / timeToEnd));

            }
            else
            {
                if (!fading)
                {
                    fading = true;
                    Debug.Log("fading now");
                    StartCoroutine(FadeToBlack());
                }

            }
        }
       
	}
	public void DetermineWinner(int winNum)
    {
        //Debug.Log("is this happening? " + winNum +" should be winner");
        GetComponentInChildren<CameraMultitarget>().enabled = false;
        TwoDGameManager.thisInstance.TogglePlayerControl();
        winner = TwoDGameManager.thisInstance.GetPlayer(winNum).transform;
        winnerNum = winNum;
        StartCoroutine(CameraFlyBy());
    }

    public IEnumerator CameraFlyBy()
    {
        //Debug.Log("checking");
        Camera.main.transform.LookAt(winner.transform.GetChild(5));
        winner.LookAt(Camera.main.transform, Vector3.up);

        yield return null;
    }

    public IEnumerator FadeToBlack()
    {
        float fadeTimeElapsed = 0f;
        float alpha = fadeImg.color.a;
        while (fadeTimeElapsed < fadeTime)
        {
            Debug.Log("we're in the while");
            fadeTimeElapsed += Time.deltaTime;
            fadeImg.color = new Color(1, 1, 1, Mathf.Lerp(alpha, 1f, fadeTimeElapsed / fadeTime));
            yield return null;
        }
        TwoDGameManager.thisInstance.EndGame(winnerNum);
    }
}
