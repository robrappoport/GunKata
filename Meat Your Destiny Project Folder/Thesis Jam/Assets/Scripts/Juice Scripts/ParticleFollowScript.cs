using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowScript : MonoBehaviour {
    public ParticleSystem psys;
    ParticleSystem.Particle[] m_Particles;
    //public float m_drift = 0.0001f;
    public float force = 50;
    public Transform target;
    public int owner;
	// Use this for initialization
	void Start () {
        if (owner == 0)
        {
            target = GameObject.Find("Player1(Clone)").transform;
        }
        else
        {
            target = GameObject.Find("Player2(Clone)").transform;
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {
        m_Particles = new ParticleSystem.Particle[psys.particleCount];
        psys.GetParticles(m_Particles);
        for (int i = 0; i < m_Particles.Length; i++)
        {
            ParticleSystem.Particle p = m_Particles[i];
            Vector3 particleWorldPosition;
            if (psys.main.simulationSpace == ParticleSystemSimulationSpace.Local)
            {
                particleWorldPosition = transform.TransformPoint(p.position);
            }
            else if (psys.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
            {
                particleWorldPosition = psys.main.customSimulationSpace.TransformPoint(p.position);
            }
            else
            {
                particleWorldPosition = p.position;
            }
            Vector3 dir2Target = (target.position - m_Particles[i].position).normalized;
            Debug.Log(dir2Target + "direction");
            Vector3 seekForce = (dir2Target * force) * Time.deltaTime;

            p.velocity = seekForce;

            m_Particles[i] = p;
            //Debug.Log(m_Particles[i].velocity + "velocity of particles");
        }
        psys.SetParticles(m_Particles, m_Particles.Length);
	}

}
