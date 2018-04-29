using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTurretParticles", menuName = "TurretParticles")]
public class TurretParticles:ScriptableObject {
	public ParticleSystem chargeParticles;
	public ParticleSystem captureParticles;
	public ParticleSystem circleParticles;
    public GameObject[] victoryParticles;
	public Vector3 chargeParticlesLocation;
	public Vector3 captureParticlesLocation;
	public Vector3 circleParticlesLocation;
}
