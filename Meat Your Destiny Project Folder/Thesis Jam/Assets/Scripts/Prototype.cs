using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype : MonoBehaviour
{

    public GameObject cityWhole;
    public Transform buildingsHeadNodesContainer;
    public GameObject buildingDestroyingEffect;
    public ParticleSystem cloudsEffect;
    [Range(0f, 1f)]
    public float animationSpeed = 0.1f;


    private Animator cityWholeAnimator;
    private Animator buildingsAnimator;
    private List<Transform> buildingsHeadNodes;

    private bool animFinished = false;

    private void Start()
    {
        buildingsHeadNodes = new List<Transform>();


        cityWholeAnimator = cityWhole.GetComponent<Animator>();
        buildingsAnimator = buildingsHeadNodesContainer.GetComponent<Animator>();

        cityWholeAnimator.speed = animationSpeed;
        buildingsAnimator.speed = animationSpeed;

        cityWholeAnimator.enabled = false;
        buildingsAnimator.enabled = false;


        for (int i = 0; i < buildingsHeadNodesContainer.childCount; i++)
        {
            buildingsHeadNodes.Add(buildingsHeadNodesContainer.GetChild(i));
        }

        PlaceEffects();

        //Invoke("PlayDemo", 5);
    }

    private void Update()
    {
        //Debug.Log(cityWholeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (!animFinished && cityWholeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            Debug.Log("Disappearing clouds");
            DisappearClouds();
            animFinished = true;
        }
    }

    public void PlayDemo()
    {


        cityWholeAnimator.enabled = true;
        buildingsAnimator.enabled = true;

        ParticleSystem.MainModule mm = cloudsEffect.main;
        mm.startLifetime = new ParticleSystem.MinMaxCurve(5f);
        mm.maxParticles = 200;

        ParticleSystem.EmissionModule em = cloudsEffect.emission;
        em.rate = new ParticleSystem.MinMaxCurve(50f);



    }

    private void PlaceEffects()
    {

        foreach (Transform t in buildingsHeadNodes)
        {
            GameObject effect = Instantiate(buildingDestroyingEffect, Vector3.zero, Quaternion.Euler(70f, 0f, 0f)) as GameObject;
            effect.transform.SetParent(t, false);
            effect.transform.localScale = Vector3.one;
			
			foreach (ParticleSystem ps in effect.GetComponentsInChildren<ParticleSystem>())
			{
				ps.Stop();
			}
        }
    }

    public void DisappearClouds()
    {
        cloudsEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }


}
