using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowScript : MonoBehaviour {
    public ParticleSystem psys;
    ParticleSystem.Particle[] m_Particles;
    //public float m_drift = 0.0001f;
    public float force = 50f;
    public float turn = 20f;
    public float minDist = 15;
    public Transform target;
    public int owner;
    public float individualParticleStaminaValue;
    private bool respawning;
    ParticleSystem.MainModule particleSystemMainModule;
    private float [] particleRandCount;
    private float randTimer;
    public bool winParticles = false;

	// Use this for initialization
	void Start () {
        respawning = false;
        psys = GetComponent<ParticleSystem>();
        particleSystemMainModule = psys.main;

    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (target == null)
        {
            if (respawning == false)
            {
                respawning = true;
                StartCoroutine(PlayerSeek());
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
        if (!winParticles)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(UIManager.thisInstance.playerStamFillList[owner].rectTransform,
                                                                    UIManager.thisInstance.playerStamFillList[owner].rectTransform.position,
                                                                    Camera.main,
                                                                    out targetTransformedPosition);

        }
        else{
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
        }


            //UIManager.thisInstance.playerStamFillList[owner].rectTransform.rect.center);
                                                                           

        int particleCount = psys.particleCount;

        randTimer += Time.deltaTime;
        for (int i = 0; i < particleCount; i++)
        {
           
            if (particleRandCount[Mathf.Clamp(i, 0, particleRandCount.Length-1)] > randTimer)
            {
                continue;
            }
            Vector3 directionToTarget = Vector3.Normalize(targetTransformedPosition - m_Particles[i].position);
            Vector3 seekForce = directionToTarget;
            float dist = Vector3.Distance(targetTransformedPosition, m_Particles[i].position);
            //var targetRot = Quaternion.LookRotation(target.position - m_Particles[i].position);

            //this is where you add the forces
            //m_Particles[i].rotation = targetRot;
            m_Particles[i].velocity += seekForce * 600f;
            m_Particles[i].velocity = Vector3.ClampMagnitude(m_Particles[i].velocity, 500f);
            if (dist < minDist)
            { //player absorbed particle
                m_Particles[i].remainingLifetime = 0;
                StartCoroutine(TwoDGameManager.thisInstance.players[owner].RefillAuraOverTime(individualParticleStaminaValue));
            }

   
        }
        if (particleCount <= 0 && !winParticles)
        {
            Destroy(gameObject);
        }
        //this is where you set the particles back into the system
        psys.SetParticles(m_Particles, particleCount);
    }

    IEnumerator PlayerSeek ()
    {
        yield return new WaitForSeconds (.1f);
		if (TwoDGameManager.thisInstance.GetPlayer (owner)) {
			target = TwoDGameManager.thisInstance.GetPlayer (owner).transform;
		}

//            if (owner == 0)
//        {
//			if (TwoDGameManager.thisInstance.GetPlayer (owner)) {
//				target = TwoDGameManager.thisInstance.GetPlayer(owner)
//			}
//            target = GameObject.Find("Player1(Clone)").transform;
//        }
//        else if (owner == 1)
//        {
//            target = GameObject.Find("Player2(Clone)").transform;
//        }
        respawning = false;
        particleRandCount = new float[psys.particleCount];

        for (int i = 0; i < particleRandCount.Length; i++)
        {
            particleRandCount[i] = Random.Range(.01f, 1f);
        }
        randTimer = 0f;

    }
}

