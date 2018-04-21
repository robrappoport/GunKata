using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutsceneScript : MonoBehaviour {
    public Transform winner;
    private int winnerNum;
    public float timeToEnd;
    private float timeElapsed = 0;
    public float fadeTime;

	private void Start()
	{
        
	}

	private void Update()
	{
        if (winner != null)
        {
            if (Vector3.Distance(winner.transform.position, Camera.main.transform.position) >= 2f)
            {
                timeElapsed += Time.deltaTime;
                Debug.Log(timeElapsed + "time elapsed");
                winner.transform.position = Vector3.Lerp(winner.transform.position, Camera.main.transform.position, Easing.QuadEaseIn(timeElapsed / timeToEnd));
            }
            else
            {
                StartCoroutine(FadeToBlack());
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
        
        yield return new WaitForSeconds(fadeTime);
        TwoDGameManager.thisInstance.EndGame(winnerNum);
    }
}
