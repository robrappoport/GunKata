using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Vector3 player1pos, player2pos, finalPos;
    public float rotationSpeed = 1, introductionTime = 3, cameraTransitionTime = 1;
    private Vector3 targetDir, newDir;
    Transform currentTarget;
    public Transform stageTransform;
    public Transform cityTransform;

	private void Awake()
	{
        GetComponentInChildren<CameraMultitarget>().enabled = false;

	}
	private void Start()
	{
        currentTarget = cityTransform;
        StartCoroutine(IntroSequence(introductionTime, cameraTransitionTime));
	}
    void LateUpdate()
    {
        RotateTowards(currentTarget, rotationSpeed);
    }

    IEnumerator IntroSequence(float introTime, float transitionTime){
        TwoDGameManager.thisInstance.TogglePlayerControl();
        float elapsedTime = 0;
        //go from start view to stage view
        yield return new WaitForSeconds(5f);
        currentTarget = stageTransform;
        while(elapsedTime<transitionTime){
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, finalPos, elapsedTime / transitionTime);
            yield return null;
        }
        yield return new WaitForSeconds(introTime);
        //go from stage view to p1 view
        elapsedTime = 0;
        currentTarget = TwoDGameManager.thisInstance.players[0].transform.GetChild(5);
        while(elapsedTime<transitionTime){
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, player1pos, elapsedTime / transitionTime);
            yield return null;
        }
        yield return new WaitForSeconds(introTime);
        //go from p1 view to p2 view
        elapsedTime = 0;
        currentTarget = TwoDGameManager.thisInstance.players[1].transform.GetChild(5);

        while(elapsedTime < transitionTime){
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, player2pos, elapsedTime / transitionTime);
            yield return null;
        }
        yield return new WaitForSeconds(introTime);
        //go from p2 view to stage view again

        elapsedTime = 0;
        currentTarget = stageTransform;

        while(elapsedTime < transitionTime){
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, finalPos, elapsedTime / transitionTime);
            yield return null;
        }
        //activate the multitarget 
        GetComponentInChildren<CameraMultitarget>().enabled = true;
        currentTarget = null;
        TwoDGameManager.thisInstance.TogglePlayerControl();
    }

    void RotateTowards(Transform target, float speed){
        if (target)
        {
            targetDir = targetDir - transform.position;
            float step = speed * Time.deltaTime;
            newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            Debug.DrawRay(transform.position, newDir, Color.red, 50000f);
            transform.LookAt(target);
        }
    }
}
