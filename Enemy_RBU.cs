using System;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class Enemy_RBU : MonoBehaviour
{
	// Token: 0x06000785 RID: 1925 RVA: 0x000450A0 File Offset: 0x000432A0
	private void FixedUpdate()
	{
		if (this.fireAtPosition)
		{
			this.fireAtPosition = false;
			this.eventCameraSet = false;
			this.firingAtPlayerTimer = 0f;
			this.firingPhase = 1;
			this.mortarsFired = 0;
		}
		if (this.firingPhase != 0)
		{
			this.firingAtPlayerTimer += Time.deltaTime;
			if (this.firingPhase == 1 || this.firingPhase == 2)
			{
				float num = Vector3.Distance(this.rbuLaunchers[this.rbuFiring].transform.position, this.vesselmovement.parentVessel.vesselai.transformToAttack.position);
				float num2 = 1000f - GameDataManager.playervesselsonlevel[0].transform.position.y;
				float num3 = num2 * 5f;
				num3 += num / UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[this.rbuFiring]].velocity;
				Transform transformToAttack = this.vesselmovement.parentVessel.vesselai.transformToAttack;
				if (transformToAttack != null)
				{
					this.targetPosition = transformToAttack.position + transformToAttack.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * num3;
				}
				this.targetPosition.y = 1000f;
				float num4 = num * GameDataManager.yardsScale;
				if (num4 < 1000f)
				{
					this.firingAtPlayerTimer = 10f;
					this.rbuLaunchEffects[this.rbuFiring].Stop();
					this.rbuLaunchEffects[this.rbuFiring].gameObject.SetActive(false);
					return;
				}
				float elevationToDistance = this.GetElevationToDistance(num, this.rbuLaunchers[this.rbuFiring].transform.position);
				this.rbuLaunchers[this.rbuFiring].transform.localRotation = Quaternion.Slerp(this.rbuLaunchers[this.rbuFiring].transform.localRotation, Quaternion.Euler(-elevationToDistance, 0f, 0f), 1f * Time.deltaTime);
				this.rbuPositions[this.rbuFiring].transform.localRotation = Quaternion.Slerp(this.rbuPositions[this.rbuFiring].transform.localRotation, Quaternion.Euler(0f, this.rbuBearingToFire, 0f), 1f * Time.deltaTime);
				if (this.firingPhase == 1)
				{
					if (this.firingAtPlayerTimer > 5f)
					{
						bool flag = true;
						if (!UIFunctions.globaluifunctions.combatai.CheckClearLOSToTarget(this.vesselmovement.parentVessel, base.transform.position, this.targetPosition))
						{
							flag = false;
						}
						if (flag)
						{
							this.firingPhase = 2;
							this.firingAtPlayerTimer = 0f;
							this.salvosFired[this.rbuFiring]++;
						}
						else
						{
							this.firingPhase = 4;
							this.firingAtPlayerTimer = 0f;
						}
					}
				}
				else if (this.firingPhase == 2 && this.firingAtPlayerTimer > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[this.rbuFiring]].rateOfFire)
				{
					this.firingAtPlayerTimer = 0f;
					this.FireRBUMortar(this.targetPosition);
				}
			}
			else if (this.firingPhase == 3)
			{
				this.rbuLaunchers[this.rbuFiring].transform.localRotation = Quaternion.Slerp(this.rbuLaunchers[this.rbuFiring].transform.localRotation, Quaternion.Euler(270f, 0f, 0f), 1f * Time.deltaTime);
				this.rbuPositions[this.rbuFiring].transform.localRotation = Quaternion.Slerp(this.rbuPositions[this.rbuFiring].transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 1f * Time.deltaTime);
				if (this.firingAtPlayerTimer > 60f)
				{
					this.firingPhase = 4;
				}
			}
			else if (this.firingPhase == 4)
			{
				this.rbuLaunchers[this.rbuFiring].transform.localRotation = Quaternion.Slerp(this.rbuLaunchers[this.rbuFiring].transform.localRotation, Quaternion.Euler(355f, 0f, 0f), 1f * Time.deltaTime);
				this.rbuPositions[this.rbuFiring].transform.localRotation = Quaternion.Slerp(this.rbuPositions[this.rbuFiring].transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 1f * Time.deltaTime);
				if (this.firingAtPlayerTimer > 60f)
				{
					this.firingPhase = 0;
				}
			}
		}
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x000455C0 File Offset: 0x000437C0
	private void FireRBUMortar(Vector3 targetPosition)
	{
		if (!this.eventCameraSet)
		{
			UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(this.vesselmovement.transform, GameDataManager.playervesselsonlevel[0].transform, 10f, true, false, false, this.vesselmovement.parentVessel.databaseshipdata.minCameraDistance + 2f, this.vesselmovement.parentVessel.databaseshipdata.minCameraDistance, -1f, true);
			this.eventCameraSet = true;
		}
		this.rbuHubs[this.rbuFiring].transform.localRotation = Quaternion.Slerp(this.rbuHubs[this.rbuFiring].transform.localRotation, Quaternion.Euler(0f, 0f, UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[this.rbuFiring]].mortarPositions[this.mortarsFired]), 1f);
		GameObject gameObject = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[this.rbuFiring]].depthChargeObject, this.rbuLaunchPositions[this.rbuFiring].transform.position, Quaternion.identity);
		gameObject.SetActive(true);
		float spreadRadius = UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[this.rbuFiring]].spreadRadius;
		targetPosition.x += UnityEngine.Random.Range(-spreadRadius, spreadRadius);
		targetPosition.z += UnityEngine.Random.Range(-spreadRadius, spreadRadius);
		float elevationToDistance = this.GetElevationToDistance(Vector3.Distance(this.rbuLaunchers[this.rbuFiring].transform.position, targetPosition), this.rbuLaunchers[this.rbuFiring].transform.position);
		gameObject.transform.LookAt(targetPosition);
		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(-elevationToDistance, gameObject.transform.eulerAngles.y, 0f), 1f);
		Projectile_DepthCharge component = gameObject.GetComponent<Projectile_DepthCharge>();
		component.depthChargeID = this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[this.rbuFiring];
		component.velocity = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].velocity;
		if (GameDataManager.optionsFloatSettings[7] == 0f)
		{
			component.velocity *= 1.5f;
		}
		component.sinkSpeed = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].sinkRate;
		component.isDepthExploded = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].depthExploded;
		component.isContactExploded = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].contactExploded;
		component.Start();
		component.FixedUpdate();
		this.mortarsFired++;
		if (this.mortarsFired > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[this.rbuFiring]].numberOfMortars - 1)
		{
			this.firingPhase = 3;
		}
		this.rbuLaunchEffects[this.rbuFiring].Stop();
		this.rbuLaunchEffects[this.rbuFiring].gameObject.SetActive(false);
		this.rbuLaunchEffects[this.rbuFiring].gameObject.SetActive(true);
		this.rbuLaunchEffects[this.rbuFiring].Play();
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0004598C File Offset: 0x00043B8C
	public float GetElevationToDistance(float distance, Vector3 firingPosition)
	{
		float num = 10f;
		if (GameDataManager.optionsFloatSettings[7] == 0f)
		{
			num *= 1.5f;
		}
		float num2 = 2f;
		float num3 = -(firingPosition.y - 1000f);
		float num4 = num * num * num * num - num2 * (num2 * (distance * distance) + 2f * num3 * (num * num));
		num4 = Mathf.Sqrt(num4);
		float num5 = Mathf.Atan((num * num - num4) / (num2 * distance));
		num5 *= 57.29578f;
		if (float.IsNaN(num5))
		{
			num5 = 45f;
		}
		return num5;
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x00045A28 File Offset: 0x00043C28
	public int GetRBUToFire()
	{
		if (this.vesselmovement.parentVessel.vesselai.transformToAttack == null)
		{
			return -1;
		}
		float bearingToTransform = UIFunctions.globaluifunctions.GetBearingToTransform(this.vesselmovement.parentVessel.transform, this.vesselmovement.parentVessel.vesselai.transformToAttack);
		float num = Vector3.Distance(this.vesselmovement.parentVessel.transform.position, this.vesselmovement.parentVessel.vesselai.transformToAttack.position) * GameDataManager.yardsScale;
		for (int i = 0; i < this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes.Length; i++)
		{
			if (bearingToTransform > this.vesselmovement.parentVessel.databaseshipdata.rbuFiringArcMin[i] && bearingToTransform < this.vesselmovement.parentVessel.databaseshipdata.rbuFiringArcMax[i] && this.salvosFired[i] < this.vesselmovement.parentVessel.databaseshipdata.rbuSalvos[i] - 1 && num > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[i]].weaponRange.x && num < UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.rbuLauncherTypes[i]].weaponRange.y && UIFunctions.globaluifunctions.combatai.CheckClearLOSToTarget(this.vesselmovement.parentVessel, base.transform.position, this.targetPosition))
			{
				this.rbuBearingToFire = bearingToTransform;
				this.rbuFiring = i;
				return i;
			}
		}
		return -1;
	}

	// Token: 0x04000A5A RID: 2650
	public VesselMovement vesselmovement;

	// Token: 0x04000A5B RID: 2651
	public Transform[] rbuLaunchers;

	// Token: 0x04000A5C RID: 2652
	public Transform[] rbuPositions;

	// Token: 0x04000A5D RID: 2653
	public Transform[] rbuHubs;

	// Token: 0x04000A5E RID: 2654
	public Transform[] rbuLaunchPositions;

	// Token: 0x04000A5F RID: 2655
	public int[] salvosFired;

	// Token: 0x04000A60 RID: 2656
	public ParticleSystem[] rbuLaunchEffects;

	// Token: 0x04000A61 RID: 2657
	public float rbuBearingToFire;

	// Token: 0x04000A62 RID: 2658
	public int rbuFiring;

	// Token: 0x04000A63 RID: 2659
	public float firingAtPlayerTimer;

	// Token: 0x04000A64 RID: 2660
	public int firingPhase;

	// Token: 0x04000A65 RID: 2661
	public bool rbuInFiringCycle;

	// Token: 0x04000A66 RID: 2662
	public int mortarsFired;

	// Token: 0x04000A67 RID: 2663
	public bool fireAtPosition;

	// Token: 0x04000A68 RID: 2664
	public Vector3 targetPosition;

	// Token: 0x04000A69 RID: 2665
	private bool eventCameraSet;
}
