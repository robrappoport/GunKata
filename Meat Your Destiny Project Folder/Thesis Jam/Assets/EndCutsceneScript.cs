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
    public bool watchingPlayerRise = false;
    private Animator myAnim;
    public AudioClip takeOffSound;
    private bool takeOffSoundHasPlayed = false;
    public Vector3 finalPosition;
    Transform center;



	private void Start()
	{
        center = FindObjectOfType<TurretCarrier>().center;
        myAnim = GetComponent<Animator>();
        finalPosition = GetComponent<CameraFollow>().finalPos;
	}

	private void Update()
	{
        if (winner != null && watchingPlayerRise)
        {
            if (timeElapsed < timeToEnd)
            {
                if(!Sound.me.IsPlaying(takeOffSound) && !takeOffSoundHasPlayed){
                    takeOffSoundHasPlayed = true;
                    Sound.me.Play(takeOffSound);
                }
                timeElapsed += Time.deltaTime;
                winner.transform.Translate(Vector3.up * (Time.deltaTime * 500f), Space.World);
            }
            else
            {
                if (!fading)
                {
                    //Debug.Log("hello?");
                    fading = true;
                    StartCoroutine(FadeToBlack());

                }

            }
        }

      
	}
	private void LateUpdate()
	{
        if (timeElapsed < timeToEnd && winner)
        {
            Camera.main.transform.LookAt(winner.GetChild(5));
        }
	}
	public void DetermineWinner(int winNum)
    {
        GetComponentInChildren<CameraMultitarget>().enabled = false;
        winner = TwoDGameManager.thisInstance.GetPlayer(winNum).transform;
        winnerNum = winNum;
        Camera.main.transform.ResetPosition();
        Camera.main.transform.ResetRotation();
        Camera.main.transform.ResetLocalScale();
        StartCoroutine(SetPlayerToCenter());
        StartCoroutine(CameraFlyBy(1));

    }

    public IEnumerator CameraFlyBy(float lerpTime)
    {
        UIManager.thisInstance.UIFadeIn(false);
        myAnim.applyRootMotion = false;
        float elapsedTime = 0;
        while(elapsedTime<lerpTime){
            transform.position = Vector3.Lerp(transform.position, finalPosition, Easing.QuadEaseOut(elapsedTime/lerpTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //Debug.Log("checking");
        myAnim.SetTrigger("End Game");

    }

    public IEnumerator SetPlayerToCenter(float totalTime = 1, float movementSpeed = 1, float rotSpeed = 2){
        float elapsedTime = 0;
        Vector3 targetDir, newDir;
        Vector3 targetPos = new Vector3(center.position.x, winner.position.y, center.position.z);
        while (elapsedTime < totalTime)
        {
            //face that player over a period of time
            targetDir = new Vector3(Camera.main.transform.position.x, winner.transform.position.y, Camera.main.transform.position.z) - winner.transform.position;
            float rotationStep = rotSpeed * Time.deltaTime;
            newDir = Vector3.RotateTowards(winner.transform.forward, targetDir, rotationStep, 0.0F);
            winner.transform.rotation = Quaternion.LookRotation(newDir);

            winner.transform.position = Vector3.Lerp(winner.transform.position, targetPos, elapsedTime * movementSpeed / totalTime);


            elapsedTime += Time.deltaTime;
            //print(elapsedTime / totalTime);
            yield return null;
        }

    }


    public void WatchPlayerRise(){
        watchingPlayerRise = true;

    }

    public IEnumerator FadeToBlack()
    {
        float fadeTimeElapsed = 0f;
        float alpha = fadeImg.color.a;
        while (fadeTimeElapsed < fadeTime)
        {
            //Debug.Log("we're in the while");
            fadeTimeElapsed += Time.deltaTime;
            fadeImg.color = new Color(1, 1, 1, Mathf.Lerp(alpha, 1f, fadeTimeElapsed / fadeTime));
            yield return null;
        }
        TwoDGameManager.thisInstance.EndGame(winnerNum);
    }
}
