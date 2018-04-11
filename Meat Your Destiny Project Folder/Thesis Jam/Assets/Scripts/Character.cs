using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character")]
public class Character:ScriptableObject {
	[Header("General vars")]
	public float staminaRechargeRate;
	[Header("Movement vars")]
	public float TurnSpeed;
	public float PrevMoveForce, SlowForce, shootForce, moveMultipler, stuckTime, forceMultiplier, dashCost, dashStaminaDrainRate;

	[Header("Aura charge vars")]
	public float auraDrainRate;
	public float auraChargeRate, coolDownDuration;
	[Header("Charge shot vars")]
	public float laserStaminaDrainRate;
	public float initialChargeBuffer, LoadedChargeTime, TotalLaserShotTime;


}
