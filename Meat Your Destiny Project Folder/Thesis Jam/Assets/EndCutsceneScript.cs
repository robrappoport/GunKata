using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutsceneScript : MonoBehaviour {
    public Transform winner;
    public float movementSpeed;


	private void Start()
	{
		
	}

	private void Update()
	{
		
	}
	public void DetermineWinner(int winNum)
    {
        //Debug.Log("is this happening? " + winNum +" should be winner");
        GetComponentInChildren<CameraMultitarget>().enabled = false;
        TwoDGameManager.thisInstance.TogglePlayerControl();
        winner = TwoDGameManager.thisInstance.GetPlayer(winNum).transform;
        StartCoroutine(CameraFlyBy());
    }

    public IEnumerator CameraFlyBy()
    {
        //Debug.Log("checking");
        Camera.main.transform.LookAt(winner);
        while (Vector3.Distance(winner.transform.position, Camera.main.transform.position) >= 2f)
        {
            winner.transform.position = Vector3.Lerp(winner.transform.position, Camera.main.transform.position, Easing.QuadEaseIn(movementSpeed * Time.deltaTime));
        }
            //fade to black
            yield return new WaitForSeconds(2f);
            Debug.Log("restart now");

       

    }
}
