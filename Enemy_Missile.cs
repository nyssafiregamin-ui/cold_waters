using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class Enemy_Missile : MonoBehaviour
{
	// Token: 0x06000779 RID: 1913 RVA: 0x00044124 File Offset: 0x00042324
	public void InitialiseEnemyMissile()
	{
		this.numberOfMissiles = new int[this.vesselmovement.parentVessel.databaseshipdata.missilesPerLauncher.Length];
		for (int i = 0; i < this.numberOfMissiles.Length; i++)
		{
			this.numberOfMissiles[i] = this.vesselmovement.parentVessel.databaseshipdata.missilesPerLauncher[i];
		}
		for (int j = 0; j < this.missileLaunchParticles.Length; j++)
		{
			int num = 1;
			if (this.missileLaunchers[j].localPosition.x < 0f)
			{
				num = 0;
			}
			if (this.missileLaunchers[j].localPosition.x > 0f)
			{
				num = 2;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.missileLaunch[num], this.missileLaunchers[j].position, this.missileLaunchers[j].rotation) as GameObject;
			gameObject.transform.SetParent(this.missileLaunchers[j]);
			gameObject.transform.localPosition = this.missileLaunchParticlePositions[j];
			gameObject.transform.localRotation = Quaternion.identity;
			this.missileLaunchParticles[j] = gameObject.GetComponent<ParticleSystem>();
		}
		this.firingPhaseTimes = new float[]
		{
			6f,
			3f,
			6f
		};
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00044294 File Offset: 0x00042494
	private void FixedUpdate()
	{
		if (this.snapShotRefreshTime > 0f)
		{
			this.snapShotRefreshTime -= Time.deltaTime;
		}
		if (this.enemyMissileTimer > 0f)
		{
			this.enemyMissileTimer -= Time.deltaTime;
		}
		if (this.firingPhase != 0)
		{
			this.LauncherCycle();
			return;
		}
		if (this.fireTimer > 0f)
		{
			this.fireTimer -= Time.deltaTime;
			if (this.fireTimer == 0f)
			{
				this.fireAtTarget = true;
			}
		}
		if (this.fireAtTarget)
		{
			this.fireAtTarget = false;
			this.numberFired++;
			if (UnityEngine.Random.value < 0.5f && this.numberFired < 3)
			{
				this.fireTimer = UnityEngine.Random.Range(3f, 9f);
			}
			this.FireMissile();
			if (this.numberFired == 2)
			{
				this.numberFired = 0;
				this.fireTimer = 0f;
			}
		}
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x000443A4 File Offset: 0x000425A4
	private void LauncherCycle()
	{
		this.trainTimer -= Time.deltaTime;
		if (this.firingPhase == 1)
		{
			if (this.vesselmovement.parentVessel.databaseshipdata.missileLauncherElevates[this.launcherToFire])
			{
				this.missileLaunchers[this.launcherToFire].transform.localRotation = Quaternion.RotateTowards(this.missileLaunchers[this.launcherToFire].transform.localRotation, Quaternion.Euler(-this.vesselmovement.parentVessel.databaseshipdata.missileLauncherElevationMax[this.launcherToFire], 0f, 0f), this.launcherElevationRate * Time.deltaTime);
			}
		}
		else if (this.firingPhase == 3 && this.vesselmovement.parentVessel.databaseshipdata.missileLauncherElevates[this.launcherToFire])
		{
			this.missileLaunchers[this.launcherToFire].transform.localRotation = Quaternion.RotateTowards(this.missileLaunchers[this.launcherToFire].transform.localRotation, Quaternion.Euler(-this.vesselmovement.parentVessel.databaseshipdata.missileLauncherElevationMin[this.launcherToFire], 0f, 0f), this.launcherElevationRate * Time.deltaTime);
		}
		if (this.trainTimer < 0f)
		{
			if (this.firingPhase == 3)
			{
				this.firingPhase = 0;
				return;
			}
			if (this.firingPhase == 1)
			{
				Transform targetTransform = this.vesselmovement.parentVessel.vesselai.transformToAttack;
				targetTransform = GameDataManager.playervesselsonlevel[0].transform;
				this.LaunchMissile(targetTransform);
			}
			this.trainTimer = this.firingPhaseTimes[this.firingPhase];
			this.firingPhase++;
		}
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00044574 File Offset: 0x00042774
	private void LaunchMissile(Transform targetTransform)
	{
		if (this.vesselmovement.parentVessel.isSinking)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].whichNavy == this.vesselmovement.parentVessel.whichNavy)
			{
				num++;
			}
		}
		if (num > UIFunctions.globaluifunctions.combatai.maxWeaponsInPlay)
		{
			return;
		}
		this.vesselmovement.parentVessel.vesselai.subCanUseActiveSonar = true;
		this.numberOfMissiles[this.launcherToFire]--;
		float num2 = Vector3.Distance(this.vesselmovement.parentVessel.transform.position, targetTransform.position);
		GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.databaseweapondata[this.vesselmovement.parentVessel.databaseshipdata.missileType].weaponObject, this.missileLaunchers[this.launcherToFire].position, this.missileLaunchers[this.launcherToFire].rotation) as GameObject;
		gameObject.SetActive(true);
		Torpedo component = gameObject.GetComponent<Torpedo>();
		component.databaseweapondata = UIFunctions.globaluifunctions.database.databaseweapondata[this.vesselmovement.parentVessel.databaseshipdata.missileType];
		UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(gameObject.transform, null, 8f, true, false, false, component.databaseweapondata.minCameraDistance + 1f, component.databaseweapondata.minCameraDistance, UnityEngine.Random.Range(25f, 50f), true);
		component.vesselFiredFrom = this.vesselmovement.parentVessel;
		component.InitialiseTorpedo();
		this.missileLaunchParticles[this.launcherToFire].gameObject.SetActive(false);
		this.missileLaunchParticles[this.launcherToFire].gameObject.SetActive(true);
		this.missileLaunchParticles[this.launcherToFire].Play();
		this.vesselmovement.parentVessel.uifunctions.playerfunctions.sensormanager.AddTorpedoToArray(component);
		Vector3 b = targetTransform.position;
		if (!this.snapshot)
		{
			float d = num2 / (UIFunctions.globaluifunctions.database.databaseweapondata[component.databaseweapondata.weaponID].activeRunSpeed / 10f * GameDataManager.globalTranslationSpeed);
			b = targetTransform.position + targetTransform.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
		}
		else
		{
			b = this.GetSnapShotTargetPosition();
			this.snapshotNumberToFire--;
		}
		b.y = 1000f;
		Vector2 vector = new Vector2(0.8f, 1f);
		if (this.vesselmovement.parentVessel.vesselai.sensordata.radarTotalDetected > 0f)
		{
			vector = new Vector2(0.9f, 1f);
		}
		component.launchPosition = base.transform.position;
		component.initialWaypointPosition = Vector3.Lerp(this.vesselmovement.parentVessel.transform.position, b, UnityEngine.Random.Range(vector.x, vector.y));
		component.distanceToWaypoint = Vector3.Distance(this.vesselmovement.parentVessel.transform.position, component.initialWaypointPosition);
		if (component.distanceToWaypoint < 33f)
		{
			component.distanceToWaypoint = 33f;
		}
		if (this.snapshotNumberToFire == 0)
		{
			this.snapshot = false;
		}
		else
		{
			base.StartCoroutine(this.FireSecondSnapShot(targetTransform));
		}
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0004495C File Offset: 0x00042B5C
	private IEnumerator FireSecondSnapShot(Transform targetTransform)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));
		this.LaunchMissile(targetTransform);
		yield break;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00044988 File Offset: 0x00042B88
	private Vector3 GetSnapShotTargetPosition()
	{
		float num = (float)UnityEngine.Random.Range(7000, 9000);
		if (!this.vesselmovement.parentVessel.vesselai.sensordata.playerDetected)
		{
			if (this.snapshotNumberToFire == 1)
			{
				num = (float)UnityEngine.Random.Range(10000, 13000);
			}
			this.vesselmovement.parentVessel.acoustics.sensorNavigator.LookAt(this.vesselmovement.parentVessel.vesselai.transformToAttack.position);
			num = Mathf.Clamp(num, UIFunctions.globaluifunctions.database.databaseweapondata[this.vesselmovement.parentVessel.databaseshipdata.missileType].missileFiringRange.x, UIFunctions.globaluifunctions.database.databaseweapondata[this.vesselmovement.parentVessel.databaseshipdata.missileType].missileFiringRange.y);
			num *= GameDataManager.inverseYardsScale;
			return this.vesselmovement.parentVessel.acoustics.sensorNavigator.position + this.vesselmovement.parentVessel.acoustics.sensorNavigator.forward * num;
		}
		this.vesselmovement.parentVessel.vesselai.transformToAttack = GameDataManager.playervesselsonlevel[0].transform;
		num = Vector3.Distance(base.transform.position, this.vesselmovement.parentVessel.vesselai.transformToAttack.position);
		float d = num / (UIFunctions.globaluifunctions.database.databaseweapondata[this.vesselmovement.parentVessel.databaseshipdata.missileType].activeRunSpeed / 10f * GameDataManager.globalTranslationSpeed);
		if (this.vesselmovement.parentVessel.vesselai.sensordata.decibelsLastDetected > 10f)
		{
			num += (float)UnityEngine.Random.Range(-1000, 1000);
			this.snapshotNumberToFire--;
			return this.vesselmovement.parentVessel.vesselai.transformToAttack.position + this.vesselmovement.parentVessel.vesselai.transformToAttack.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
		}
		if (this.snapshotNumberToFire == 2)
		{
			num += (float)UnityEngine.Random.Range(-2000, 2000);
		}
		else
		{
			num += (float)UnityEngine.Random.Range(-4000, 4000);
		}
		return this.vesselmovement.parentVessel.vesselai.transformToAttack.position + this.vesselmovement.parentVessel.vesselai.transformToAttack.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00044C90 File Offset: 0x00042E90
	private void FireMissile()
	{
		this.launcherToFire = 0;
		float bearingToTransform = GameDataManager.playervesselsonlevel[0].uifunctions.GetBearingToTransform(this.vesselmovement.parentVessel.transform, this.vesselmovement.parentVessel.vesselai.transformToAttack);
		if (this.numberOfMissiles.Length > 1)
		{
			this.launcherToFire = 1;
			if (bearingToTransform < 0f)
			{
				this.launcherToFire = 0;
			}
			if (this.numberOfMissiles[this.launcherToFire] < 1)
			{
				if (this.launcherToFire == 0)
				{
					this.launcherToFire = 1;
				}
				else
				{
					this.launcherToFire = 0;
				}
			}
		}
		if (this.numberOfMissiles[this.launcherToFire] < 1)
		{
			return;
		}
		Transform transformToAttack = this.vesselmovement.parentVessel.vesselai.transformToAttack;
		if (transformToAttack == null)
		{
			return;
		}
		this.enemyMissileTimer = (float)UnityEngine.Random.Range(60, 180);
		if (this.vesselmovement.parentVessel.databaseshipdata.missileLauncherElevates != null)
		{
			if (this.vesselmovement.parentVessel.databaseshipdata.missileLauncherElevates[this.launcherToFire] || this.vesselmovement.parentVessel.databaseshipdata.missileLauncherPivots[this.launcherToFire])
			{
				this.trainTimer = this.firingPhaseTimes[this.firingPhase];
				this.firingPhase = 1;
			}
		}
		else
		{
			this.LaunchMissile(transformToAttack);
		}
	}

	// Token: 0x04000A3F RID: 2623
	public VesselMovement vesselmovement;

	// Token: 0x04000A40 RID: 2624
	public Transform[] missileLaunchers;

	// Token: 0x04000A41 RID: 2625
	public Vector3[] missileLaunchParticlePositions;

	// Token: 0x04000A42 RID: 2626
	public ParticleSystem[] missileLaunchParticles;

	// Token: 0x04000A43 RID: 2627
	public int[] numberOfMissiles;

	// Token: 0x04000A44 RID: 2628
	public bool fireAtTarget;

	// Token: 0x04000A45 RID: 2629
	public int numberFired;

	// Token: 0x04000A46 RID: 2630
	public float fireTimer;

	// Token: 0x04000A47 RID: 2631
	public float enemyMissileTimer;

	// Token: 0x04000A48 RID: 2632
	public Transform[] missileAzimuthTransforms;

	// Token: 0x04000A49 RID: 2633
	public Transform[] missileElevationTransforms;

	// Token: 0x04000A4A RID: 2634
	public int firingPhase;

	// Token: 0x04000A4B RID: 2635
	public float trainTimer;

	// Token: 0x04000A4C RID: 2636
	public int launcherToFire;

	// Token: 0x04000A4D RID: 2637
	public float launcherElevationRate = 5f;

	// Token: 0x04000A4E RID: 2638
	public float launcherPivotRate = 5f;

	// Token: 0x04000A4F RID: 2639
	public float[] firingPhaseTimes;

	// Token: 0x04000A50 RID: 2640
	public bool snapshot;

	// Token: 0x04000A51 RID: 2641
	public int snapshotNumberToFire;

	// Token: 0x04000A52 RID: 2642
	public float snapShotRefreshTime = 300f;

	// Token: 0x04000A53 RID: 2643
	public float snapshotTimer;
}
