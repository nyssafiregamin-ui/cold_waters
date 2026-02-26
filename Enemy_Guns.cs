using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class Enemy_Guns : MonoBehaviour
{
	// Token: 0x06000770 RID: 1904 RVA: 0x000433E4 File Offset: 0x000415E4
	private void Start()
	{
		base.enabled = false;
		this.timer = 0f;
		this.firingAtPlayerTimer = new float[this.turrets.Length];
		this.firingPhase = new int[this.turrets.Length];
		this.shellsFired = new int[this.turrets.Length];
		this.gunTrainRate = 1f;
		this.gunElevationRate = 1f;
		for (int i = 0; i < this.firingAtPlayerTimer.Length; i++)
		{
			this.firingAtPlayerTimer[i] = UnityEngine.Random.Range(-4f, 0f);
			this.shellsFired[i] = 0;
		}
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0004348C File Offset: 0x0004168C
	private void FixedUpdate()
	{
		if (UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.percentageSurfaced <= 0f || !UIFunctions.globaluifunctions.combatai.PlayerIsCombatWorthy())
		{
			for (int i = 0; i < this.firingPhase.Length; i++)
			{
				if (this.firingPhase[i] != 4)
				{
					this.firingPhase[i] = 4;
					this.firingAtPlayerTimer[i] = 0f;
				}
			}
		}
		this.wantedGunElevation = this.GetElevationToDistance(0, Vector3.Distance(this.turrets[0].transform.position, this.targetPosition), this.turrets[0].transform.position);
		this.gunBearingToFire = UIFunctions.globaluifunctions.GetBearingToTransform(this.vesselmovement.parentVessel.transform, GameDataManager.playervesselsonlevel[0].transform);
		int num = 0;
		for (int j = 0; j < this.turrets.Length; j++)
		{
			this.GunCycle(j);
			if (this.firingPhase[j] == 4 && this.firingAtPlayerTimer[j] > 20f)
			{
				num++;
			}
		}
		if (num == this.turrets.Length)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x000435D0 File Offset: 0x000417D0
	private void GunCycle(int gunIndex)
	{
		if (this.firingPhase[gunIndex] != 0)
		{
			this.firingAtPlayerTimer[gunIndex] += Time.deltaTime;
			if (this.firingPhase[gunIndex] == 1 || this.firingPhase[gunIndex] == 2)
			{
				float num = Vector3.Distance(this.turrets[gunIndex].transform.position, GameDataManager.playervesselsonlevel[0].transform.position);
				float d = num / UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[gunIndex]].velocity;
				Transform transform = GameDataManager.playervesselsonlevel[0].transform;
				if (transform != null)
				{
					this.targetPosition = transform.position + transform.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
				}
				this.targetPosition.y = 1000f;
				float num2 = num * GameDataManager.yardsScale;
				if (num2 < 100f)
				{
					this.firingAtPlayerTimer[gunIndex] = 0f;
					this.firingPhase[gunIndex] = 4;
					return;
				}
				if (this.firingPhase[gunIndex] == 1)
				{
					if (this.firingAtPlayerTimer[gunIndex] > 5f)
					{
						bool flag = true;
						if (GameDataManager.playervesselsonlevel[0].vesselmovement.percentageSurfaced < 0.5f && this.vesselmovement.parentVessel.vesselai.sensordata.radarTotalDetected <= 0f)
						{
							flag = false;
						}
						else if (!UIFunctions.globaluifunctions.combatai.PlayerIsCombatWorthy())
						{
							flag = false;
						}
						else if (!UIFunctions.globaluifunctions.combatai.CheckClearLOSToTarget(this.vesselmovement.parentVessel, base.transform.position, this.targetPosition))
						{
							flag = false;
						}
						if (flag)
						{
							this.firingPhase[gunIndex] = 2;
							this.firingAtPlayerTimer[gunIndex] = 0f;
						}
						else
						{
							this.firingAtPlayerTimer[gunIndex] = 0f;
							this.firingPhase[gunIndex] = 4;
						}
					}
				}
				else if (this.firingPhase[gunIndex] == 2 && this.firingAtPlayerTimer[gunIndex] > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[gunIndex]].rateOfFire)
				{
					this.firingAtPlayerTimer[gunIndex] = 0f;
					float dist = Vector3.Distance(this.turrets[gunIndex].transform.position, this.targetPosition);
					this.FireGun(gunIndex, this.targetPosition, dist);
				}
			}
			else if (this.firingPhase[gunIndex] == 3 && this.firingAtPlayerTimer[gunIndex] > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[gunIndex]].reloadTime)
			{
				this.firingPhase[gunIndex] = 1;
			}
			if (this.firingPhase[gunIndex] == 4)
			{
				this.turrets[gunIndex].transform.localRotation = Quaternion.Slerp(this.turrets[gunIndex].transform.localRotation, Quaternion.Euler(0f, this.vesselmovement.parentVessel.databaseshipdata.navalGunRestAngle[gunIndex], 0f), 1f * Time.deltaTime);
				this.barrels[gunIndex].transform.localRotation = Quaternion.RotateTowards(this.barrels[gunIndex].transform.localRotation, Quaternion.Euler(0f, 0f, 0f), this.gunElevationRate * Time.deltaTime);
			}
			else
			{
				this.turrets[gunIndex].transform.localRotation = Quaternion.Slerp(this.turrets[gunIndex].transform.localRotation, Quaternion.Euler(--0f, this.gunBearingToFire, 0f), this.gunTrainRate * Time.deltaTime);
				this.barrels[gunIndex].transform.localRotation = Quaternion.RotateTowards(this.barrels[gunIndex].transform.localRotation, Quaternion.Euler(-this.wantedGunElevation, 0f, 0f), this.gunElevationRate * Time.deltaTime);
			}
			if (!this.CanGunFaceBearing(this.gunBearingToFire, gunIndex))
			{
				if (this.firingPhase[gunIndex] != 4)
				{
					this.firingAtPlayerTimer[gunIndex] = 0f;
					this.firingPhase[gunIndex] = 4;
				}
			}
			else if (this.firingPhase[gunIndex] == 4)
			{
				this.firingAtPlayerTimer[gunIndex] = UnityEngine.Random.Range(-4f, 0f);
				this.firingPhase[gunIndex] = 1;
			}
		}
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00043A8C File Offset: 0x00041C8C
	private void FireGun(int gunIndex, Vector3 targetPosition, float dist)
	{
		GameObject gameObject = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[gunIndex]].depthChargeObject, this.muzzlePositions[gunIndex].position, Quaternion.identity);
		gameObject.SetActive(true);
		float num = dist * 0.02f;
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		targetPosition.x += UnityEngine.Random.Range(-num, num);
		targetPosition.z += UnityEngine.Random.Range(-num, num);
		float elevationToDistance = this.GetElevationToDistance(gunIndex, Vector3.Distance(gameObject.transform.position, targetPosition), gameObject.transform.position);
		gameObject.transform.LookAt(targetPosition);
		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(-elevationToDistance, gameObject.transform.eulerAngles.y, 0f), 1f);
		Projectile_DepthCharge component = gameObject.GetComponent<Projectile_DepthCharge>();
		component.depthChargeID = this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[gunIndex];
		component.velocity = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].velocity;
		if (GameDataManager.optionsFloatSettings[7] == 0f)
		{
			component.velocity *= 1.5f;
		}
		component.isNavalShell = true;
		component.Start();
		component.FixedUpdate();
		this.shellsFired[gunIndex]++;
		if (this.shellsFired[gunIndex] > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[gunIndex]].numberOfMortars - 1)
		{
			this.shellsFired[gunIndex] = 0;
			this.firingPhase[gunIndex] = 3;
		}
		GameObject gameObject2 = ObjectPoolManager.CreatePooled(this.vesselmovement.parentVessel.databaseshipdata.navalGunParticleEffect, this.muzzlePositions[gunIndex].position, this.muzzlePositions[gunIndex].rotation);
		gameObject2.transform.SetParent(this.muzzlePositions[gunIndex]);
		ObjectPoolManager.CreatePooled(this.vesselmovement.parentVessel.databaseshipdata.navalGunSmokeEffect, this.muzzlePositions[gunIndex].position, this.muzzlePositions[gunIndex].rotation);
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00043CF4 File Offset: 0x00041EF4
	public float GetElevationToDistance(int gunIndex, float distance, Vector3 firingPosition)
	{
		float num = UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[gunIndex]].velocity;
		if (GameDataManager.optionsFloatSettings[7] == 0f)
		{
			num *= 1.5f;
		}
		float num2 = 0.1427f;
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

	// Token: 0x06000775 RID: 1909 RVA: 0x00043DB4 File Offset: 0x00041FB4
	public int[] GetGunsToFire()
	{
		int[] array = new int[this.turrets.Length];
		float bearingToTransform = UIFunctions.globaluifunctions.GetBearingToTransform(this.vesselmovement.parentVessel.transform, GameDataManager.playervesselsonlevel[0].transform);
		float num = Vector3.Distance(this.vesselmovement.parentVessel.transform.position, GameDataManager.playervesselsonlevel[0].transform.position) * GameDataManager.yardsScale;
		this.gunBearingToFire = bearingToTransform;
		if (num > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[0]].weaponRange.x && num < UIFunctions.globaluifunctions.database.databasedepthchargedata[this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes[0]].weaponRange.y)
		{
			for (int i = 0; i < this.vesselmovement.parentVessel.databaseshipdata.navalGunTypes.Length; i++)
			{
				if (this.CanGunFaceBearing(bearingToTransform, i))
				{
					array[i] = 1;
				}
				else
				{
					array[i] = 0;
				}
			}
		}
		return array;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00043EE0 File Offset: 0x000420E0
	private bool withinAngles(int i, float y)
	{
		if (this.vesselmovement.parentVessel.databaseshipdata.navalGunRestAngle[i] != 180f)
		{
			if (y > this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMin[i] && y < this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMax[i])
			{
				return true;
			}
		}
		else if (this.vesselmovement.parentVessel.databaseshipdata.rearArcFiring[i])
		{
			if (y < this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMax[i] || y > this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMin[i])
			{
				return true;
			}
		}
		else if (y < this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMin[i] && y > this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMax[i])
		{
			return true;
		}
		return false;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00043FEC File Offset: 0x000421EC
	private bool CanGunFaceBearing(float bearing, int i)
	{
		if (this.vesselmovement.parentVessel.databaseshipdata.navalGunRestAngle[i] != 180f)
		{
			if (bearing > this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMin[i] && bearing < this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMax[i])
			{
				return true;
			}
		}
		else if (this.vesselmovement.parentVessel.databaseshipdata.rearArcFiring[i])
		{
			if (bearing < this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMax[i] || bearing > this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMin[i])
			{
				return true;
			}
		}
		else if (bearing < this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMin[i] && bearing > this.vesselmovement.parentVessel.databaseshipdata.navalGunFiringArcMax[i])
		{
			return true;
		}
		return false;
	}

	// Token: 0x04000A2E RID: 2606
	public VesselMovement vesselmovement;

	// Token: 0x04000A2F RID: 2607
	public Transform[] turrets;

	// Token: 0x04000A30 RID: 2608
	public Transform[] barrels;

	// Token: 0x04000A31 RID: 2609
	public Transform[] muzzlePositions;

	// Token: 0x04000A32 RID: 2610
	public bool firing;

	// Token: 0x04000A33 RID: 2611
	public float timer;

	// Token: 0x04000A34 RID: 2612
	public bool fireAtPosition;

	// Token: 0x04000A35 RID: 2613
	public Vector3 targetPosition;

	// Token: 0x04000A36 RID: 2614
	public float[] firingAtPlayerTimer;

	// Token: 0x04000A37 RID: 2615
	public float gunBearingToFire;

	// Token: 0x04000A38 RID: 2616
	public int[] firingPhase;

	// Token: 0x04000A39 RID: 2617
	public int[] shellsFired;

	// Token: 0x04000A3A RID: 2618
	public bool firingAtPlayer;

	// Token: 0x04000A3B RID: 2619
	public AudioSource audiosource;

	// Token: 0x04000A3C RID: 2620
	public float wantedGunElevation;

	// Token: 0x04000A3D RID: 2621
	public float gunTrainRate;

	// Token: 0x04000A3E RID: 2622
	public float gunElevationRate;
}
