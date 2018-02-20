using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowScript : MonoBehaviour {
    public ParticleSystem psys;
    ParticleSystem.Particle[] m_Particles;
    //public float m_drift = 0.0001f;
    public float force = 50f;
    public float turn = 20f;
    public Transform target;
    public int owner;
    private bool respawning;
    ParticleSystem.MainModule particleSystemMainModule;
	// Use this for initialization
	void Start () {
        respawning = false;
        //if (owner == 0)
        //{
        //    target = GameObject.Find("Player1(Clone)").transform;
        //}
        //else if (owner == 1)
        //{
        //    target = GameObject.Find("Player2(Clone)").transform;
           
        //}

        psys = GetComponent<ParticleSystem>();
        particleSystemMainModule = psys.main;
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (target == null)
        {
            if (respawning == false){
                respawning = true;
                StartCoroutine(PlayerIsDead());

            }
            return;
        }
        int maxParticles = particleSystemMainModule.maxParticles;

        if (m_Particles == null || m_Particles.Length < maxParticles)
        {
            m_Particles = new ParticleSystem.Particle[maxParticles];
        }

        psys.GetParticles(m_Particles);
        float forceDeltaTime = force * Time.deltaTime;

        Vector3 targetTransformedPosition;
        //this is to make sure it works regardless of what sim space the particles use
        switch (particleSystemMainModule.simulationSpace)
        {
            case ParticleSystemSimulationSpace.Local:
                {
                    targetTransformedPosition = transform.InverseTransformPoint(target.position);
                    break;
                }
            case ParticleSystemSimulationSpace.Custom:
                {
                    targetTransformedPosition = particleSystemMainModule.customSimulationSpace.InverseTransformPoint(target.position);
                    break;
                }
            case ParticleSystemSimulationSpace.World:
                {
                    targetTransformedPosition = target.position;
                    break;
                }
            default:
                {
                    throw new System.NotSupportedException(

                        string.Format("Unsupported simulation space '{0}'.",
                        System.Enum.GetName(typeof(ParticleSystemSimulationSpace), particleSystemMainModule.simulationSpace)));
                }
        }

        int particleCount = psys.particleCount;

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 directionToTarget = Vector3.Normalize(targetTransformedPosition - m_Particles[i].position);
            Vector3 seekForce = directionToTarget;
            float dist = Vector3.Distance(targetTransformedPosition, m_Particles[i].position);
            //var targetRot = Quaternion.LookRotation(target.position - m_Particles[i].position);

            //this is where you add the forces
            //m_Particles[i].rotation = targetRot;
            m_Particles[i].velocity += seekForce * 30f;
            m_Particles[i].velocity = Vector3.ClampMagnitude(m_Particles[i].velocity, 150f);
            if (dist<15f) { //player absorbed particle
                m_Particles[i].remainingLifetime = 0;
                if (owner == 0)
                {
                    TwoDGameManager.player1ScoreNum += .15f;
                }
                else if (owner == 1)
                {
                    TwoDGameManager.player2ScoreNum += .15f;
                }
            }
   
        }
        //this is where you set the particles back into the system
        psys.SetParticles(m_Particles, particleCount);
    }

    IEnumerator PlayerIsDead ()
    {
        yield return new WaitForSeconds(2f);
            if (owner == 0)
        {
            target = GameObject.Find("Player1(Clone)").transform;
        }
        else if (owner == 1)
        {
            target = GameObject.Find("Player2(Clone)").transform;
        }
        respawning = false;
    }
}

