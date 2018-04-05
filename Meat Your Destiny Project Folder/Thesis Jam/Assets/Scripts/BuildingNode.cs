using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingNode : MonoBehaviour
{

    private ParticleSystem[] _particleSystems;
    private bool isEffectStarted = false;

    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        if (!isEffectStarted)
        {
            if (transform.position.y != initialPos.y)
            {
                isEffectStarted = true;

                if (_particleSystems == null || _particleSystems.Length == 0)
                    _particleSystems = GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem ps in _particleSystems)
                {
                    ps.Play();
                }

                StartCoroutine(WatchIfStopped(0.1f));
            }
        }
    }

    private IEnumerator WatchIfStopped(float repeatCallDelay)
    {
        yield return new WaitForSeconds(repeatCallDelay);

        Vector3 prevPos = initialPos;
        Vector3 currentPos = transform.position;

        while (currentPos.y != prevPos.y)
        {
            yield return new WaitForSeconds(repeatCallDelay);
            prevPos = currentPos;
            currentPos = transform.position;

        }

        foreach (ParticleSystem ps in _particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

    }

}
