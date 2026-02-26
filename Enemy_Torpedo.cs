using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class Enemy_Torpedo : MonoBehaviour
{
	// Token: 0x0600078A RID: 1930 RVA: 0x00045C10 File Offset: 0x00043E10
	public void InitialiseEnemyTorpedo()
	{
		this.torpedoStatus = new int[this.torpedoMountAngles.Length];
		this.numberOfTorpedoes = new int[this.vesselmovement.parentVessel.databaseshipdata.torpedoNumbers.Length];
		this.enemyTorpedoTubesReloadingTimers = new float[this.torpedoMounts.Length];
		int num = 0;
		if (!this.submergedTubes)
		{
			num = 1;
		}
		for (int i = 0; i < this.numberOfTorpedoes.Length; i++)
		{
			this.numberOfTorpedoes[i] = this.vesselmovement.parentVessel.databaseshipdata.torpedoNumbers[i];
		}
		for (int j = 0; j < this.torpedoMounts.Length; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.torpedoLaunch[num], this.torpedoMounts[j].position, this.torpedoMounts[j].rotation) as GameObject;
			gameObject.transform.SetParent(this.torpedoMounts[j]);
			gameObject.transform.localPosition = this.torpedoParticlePositions[j];
			gameObject.transform.localRotation = Quaternion.identity;
			this.torpedoClouds[j] = gameObject.GetComponent<ParticleSystem>();
		}
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00045D48 File Offset: 0x00043F48
	private void FixedUpdate()
	{
		if (this.enemyTorpedoTimer > 0f)
		{
			this.enemyTorpedoTimer -= Time.deltaTime;
		}
		this.ReloadFirstTube();
		if (this.fireAtTarget)
		{
			this.fireAtTarget = false;
			if (this.fixedTubes)
			{
				this.FireTorpedo(0);
			}
			else
			{
				this.TorpedoSetup();
			}
		}
		if (!this.fixedTubes && this.launcherFiring >= 0 && this.torpedoMountAngles[this.launcherFiring] != 0f)
		{
			this.RotateTorpedoMount();
		}
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x00045DE0 File Offset: 0x00043FE0
	private int GetFirstFreeTube()
	{
		for (int i = 0; i < this.enemyTorpedoTubesReloadingTimers.Length; i++)
		{
			if (this.enemyTorpedoTubesReloadingTimers[i] <= 0f)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x00045E1C File Offset: 0x0004401C
	private void ReloadFirstTube()
	{
		for (int i = 0; i < this.enemyTorpedoTubesReloadingTimers.Length; i++)
		{
			if (this.enemyTorpedoTubesReloadingTimers[i] > 0f)
			{
				this.enemyTorpedoTubesReloadingTimers[i] -= Time.deltaTime;
				return;
			}
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00045E6C File Offset: 0x0004406C
	private void TorpedoSetup()
	{
		float bearingToTransform = GameDataManager.playervesselsonlevel[0].uifunctions.GetBearingToTransform(this.vesselmovement.parentVessel.transform, this.vesselmovement.parentVessel.vesselai.transformToAttack);
		for (int i = 0; i < this.launcherPositions.Length; i++)
		{
			if (bearingToTransform < 0f)
			{
				if (this.launcherPositions[i] == 0 || this.launcherPositions[i] == -1)
				{
					this.torpedoMountAngles[i] = 359.99f;
					this.launcherFiring = i;
					this.directionFiring = -1f;
					break;
				}
			}
			else if (this.launcherPositions[i] == 0 || this.launcherPositions[i] == 1)
			{
				this.torpedoMountAngles[i] = 0.01f;
				this.launcherFiring = i;
				this.directionFiring = 1f;
				break;
			}
		}
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00045F58 File Offset: 0x00044158
	public void FireTorpedo(int launcher)
	{
		int num = 0;
		if (this.numberOfTorpedoes[num] < 1)
		{
			return;
		}
		if (this.vesselmovement.parentVessel.damagesystem.enemyCurrentFlooding > 0.2f)
		{
			return;
		}
		Vessel parentVessel = this.vesselmovement.parentVessel;
		parentVessel.vesselai.subCanUseActiveSonar = true;
		Transform transformToAttack;
		if (this.snapshot)
		{
			transformToAttack = this.snapshotTransform;
		}
		else
		{
			transformToAttack = this.vesselmovement.parentVessel.vesselai.transformToAttack;
		}
		if (transformToAttack == null)
		{
			return;
		}
		if (parentVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			int firstFreeTube = this.GetFirstFreeTube();
			if (firstFreeTube == -1)
			{
				return;
			}
			this.enemyTorpedoTubesReloadingTimers[firstFreeTube] += parentVessel.databaseshipdata.tubereloadtime;
		}
		int num2 = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].whichNavy == this.vesselmovement.parentVessel.whichNavy)
			{
				num2++;
			}
		}
		if (num2 > UIFunctions.globaluifunctions.combatai.maxWeaponsInPlay)
		{
			return;
		}
		this.numberOfTorpedoes[num]--;
		float num3 = Vector3.Distance(parentVessel.transform.position, transformToAttack.position);
		GameObject gameObject = UnityEngine.Object.Instantiate(this.vesselmovement.parentVessel.databaseshipdata.torpedoGameObjects[num], this.torpedoSpawnPositions[launcher].position, this.torpedoSpawnPositions[launcher].rotation) as GameObject;
		gameObject.SetActive(true);
		Torpedo component = gameObject.GetComponent<Torpedo>();
		component.databaseweapondata = UIFunctions.globaluifunctions.database.databaseweapondata[this.vesselmovement.parentVessel.databaseshipdata.torpedoIDs[num]];
		component.vesselFiredFrom = this.vesselmovement.parentVessel;
		component.onWire = true;
		this.enemyTorpedoTimer += (float)UnityEngine.Random.Range(120, 240);
		bool surface = true;
		if (this.vesselmovement.parentVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			surface = false;
		}
		UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(component.transform, this.vesselmovement.transform, 6f, surface, false, false, component.databaseweapondata.minCameraDistance + 1f, component.databaseweapondata.minCameraDistance, UnityEngine.Random.Range(25f, 60f), false);
		component.noSurfaceTargets = UIFunctions.globaluifunctions.combatai.AreHostileShipsInArea();
		component.InitialiseTorpedo();
		if (component.databaseweapondata.wireGuided)
		{
			if (UnityEngine.Random.value < component.databaseweapondata.wireBreakOnLaunchProbability)
			{
				component.onWire = false;
			}
		}
		else
		{
			component.onWire = false;
		}
		if (this.vesselmovement.parentVessel.databaseshipdata.shipType != "SUBMARINE")
		{
			component.onWire = false;
		}
		this.vesselmovement.parentVessel.uifunctions.playerfunctions.sensormanager.AddTorpedoToArray(component);
		component.tacMapTorpedoIcon.contactText.gameObject.SetActive(false);
		float d = num3 / (UIFunctions.globaluifunctions.database.databaseweapondata[component.databaseweapondata.weaponID].runSpeed / 10f * GameDataManager.globalTranslationSpeed);
		Vector3 vector = transformToAttack.position;
		if (!this.snapshot)
		{
			vector = transformToAttack.position + transformToAttack.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
			vector = Vector3.Lerp(parentVessel.transform.position, vector, 0.8f);
		}
		else
		{
			vector = this.GetSnapShotTargetPosition();
			this.snapshotNumberToFire--;
		}
		this.torpedoClouds[launcher].Play();
		component.launchPosition = base.transform.position;
		component.initialWaypointPosition = vector;
		component.distanceToWaypoint = num3 * UnityEngine.Random.Range(0.8f, 0.9f);
		if (!this.submergedTubes)
		{
			component.cavitationAudioSource.Stop();
			component.isAirborne = true;
			component.actualCurrentSpeed = 0.8f;
			this.torpedoHoldTimer = 3f;
		}
		float value = UnityEngine.Random.value;
		if (value < 0.15f)
		{
			component.searchLeft = true;
			component.snakeSearch = false;
		}
		else if (value < 0.3f)
		{
			component.searchLeft = false;
			component.snakeSearch = false;
		}
		else
		{
			component.searchLeft = false;
			component.snakeSearch = true;
		}
		if (this.snapshot)
		{
			component.searchLeft = false;
			component.snakeSearch = true;
		}
		value = UnityEngine.Random.value;
		component.cruiseYValue = this.vesselmovement.parentVessel.transform.position.y;
		if (value < 0.3f)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerDepth > 0f)
			{
				component.searchYValue = UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerDepth - 0.132f;
			}
			else
			{
				component.searchYValue = 998.446f;
			}
		}
		else if (value < 0.6f)
		{
			component.searchYValue = base.transform.position.y;
		}
		else
		{
			component.searchYValue = 999.6f;
		}
		if (this.vesselmovement.isSubmarine)
		{
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(this.vesselmovement.transform.position);
			float num4 = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.localEulerAngles.y;
			if (num4 < 0f)
			{
				num4 += 360f;
			}
			if (!component.databaseweapondata.swimOut && UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckCanHearTransient(GameDataManager.playervesselsonlevel[0], this.vesselmovement.parentVessel) > 10f)
			{
				string text = string.Empty;
				if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.detectedByPlayer[this.vesselmovement.parentVessel.vesselListIndex])
				{
					UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = this.vesselmovement.parentVessel.vesselListIndex;
					text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "TransientKnown");
					text = text.Replace("<CONTACT>", UIFunctions.globaluifunctions.playerfunctions.GetFullContactName(UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapContact[this.vesselmovement.parentVessel.vesselListIndex].contactText.text, this.vesselmovement.parentVessel.vesselListIndex));
				}
				else
				{
					text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "TransientUnknown");
				}
				text = text.Replace("<BRG>", string.Format("{0:0}", num4));
				GameDataManager.playervesselsonlevel[0].uifunctions.playerfunctions.PlayerMessage(text, GameDataManager.playervesselsonlevel[0].uifunctions.playerfunctions.messageLogColors["Sonar"], "TransientKnown", false);
			}
		}
		else
		{
			component.cruiseYValue = 999.5f;
		}
		if (this.snapshotNumberToFire == 0)
		{
			this.snapshot = false;
		}
		else
		{
			base.StartCoroutine(this.FireAdditionalSnapShot(transformToAttack));
		}
		component.parachuteTransform.gameObject.SetActive(false);
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0004676C File Offset: 0x0004496C
	private IEnumerator FireAdditionalSnapShot(Transform targetTransform)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));
		if (this.fixedTubes)
		{
			this.snapshot = true;
			this.FireTorpedo(0);
		}
		else
		{
			this.snapshot = true;
			this.fireAtTarget = true;
		}
		yield break;
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x00046788 File Offset: 0x00044988
	private Vector3 GetSnapShotTargetPosition()
	{
		float num = (float)UnityEngine.Random.Range(4000, 6000);
		float num2 = UnityEngine.Random.Range(5f, 10f);
		if (this.snapshotNumberToFire == 1)
		{
			num2 *= -1f;
		}
		if (this.snapshotNumberToFire == 3)
		{
			num2 *= 2.5f;
			if (UnityEngine.Random.value < 0.5f)
			{
				num2 *= -1f;
			}
		}
		this.vesselmovement.parentVessel.acoustics.sensorNavigator.LookAt(this.snapshotTransform.position);
		this.vesselmovement.parentVessel.acoustics.sensorNavigator.transform.localRotation = Quaternion.Slerp(this.vesselmovement.parentVessel.acoustics.sensorNavigator.transform.localRotation, Quaternion.Euler(0f, this.vesselmovement.parentVessel.acoustics.sensorNavigator.transform.localEulerAngles.y + num2, 0f), 1f);
		num *= GameDataManager.inverseYardsScale * 0.8f;
		return this.vesselmovement.parentVessel.acoustics.sensorNavigator.position + this.vesselmovement.parentVessel.acoustics.sensorNavigator.forward * num;
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x000468E8 File Offset: 0x00044AE8
	private void RotateTorpedoMount()
	{
		if (this.torpedoStatus[this.launcherFiring] == 0)
		{
			this.torpedoMountAngles[this.launcherFiring] += Time.deltaTime * 10f * this.directionFiring;
			if (this.torpedoMountAngles[this.launcherFiring] > 180f)
			{
				if (this.torpedoMountAngles[this.launcherFiring] <= 315f)
				{
					this.FireTorpedo(this.launcherFiring);
					this.torpedoStatus[this.launcherFiring] = 1;
				}
			}
			else if (this.torpedoMountAngles[this.launcherFiring] >= 45f)
			{
				this.FireTorpedo(this.launcherFiring);
				this.torpedoStatus[this.launcherFiring] = 1;
			}
		}
		else
		{
			if (this.torpedoHoldTimer > 0f)
			{
				this.torpedoHoldTimer -= Time.deltaTime;
				return;
			}
			this.torpedoMountAngles[this.launcherFiring] -= Time.deltaTime * 10f * this.directionFiring;
			if (this.torpedoMountAngles[this.launcherFiring] > 180f)
			{
				if (this.torpedoMountAngles[this.launcherFiring] >= 360f)
				{
					this.torpedoMountAngles[this.launcherFiring] = 0f;
					this.torpedoStatus[this.launcherFiring] = 0;
				}
			}
			else if (this.torpedoMountAngles[this.launcherFiring] <= 0f)
			{
				this.torpedoMountAngles[this.launcherFiring] = 0f;
				this.torpedoStatus[this.launcherFiring] = 0;
			}
		}
		this.torpedoMounts[this.launcherFiring].localRotation = Quaternion.Slerp(this.torpedoMounts[this.launcherFiring].localRotation, Quaternion.Euler(0f, this.torpedoMountAngles[this.launcherFiring], 0f), 1f);
	}

	// Token: 0x04000A6A RID: 2666
	public VesselMovement vesselmovement;

	// Token: 0x04000A6B RID: 2667
	public Transform[] torpedoMounts;

	// Token: 0x04000A6C RID: 2668
	public Transform[] torpedoSpawnPositions;

	// Token: 0x04000A6D RID: 2669
	public Vector3[] torpedoParticlePositions;

	// Token: 0x04000A6E RID: 2670
	public ParticleSystem[] torpedoClouds;

	// Token: 0x04000A6F RID: 2671
	public int[] numberOfTorpedoes;

	// Token: 0x04000A70 RID: 2672
	public float[] torpedoMountAngles;

	// Token: 0x04000A71 RID: 2673
	public int[] torpedoStatus;

	// Token: 0x04000A72 RID: 2674
	public float torpedoHoldTimer;

	// Token: 0x04000A73 RID: 2675
	public int[] launcherPositions;

	// Token: 0x04000A74 RID: 2676
	public bool fixedTubes;

	// Token: 0x04000A75 RID: 2677
	public bool submergedTubes;

	// Token: 0x04000A76 RID: 2678
	public bool fireAtTarget;

	// Token: 0x04000A77 RID: 2679
	public int launcherFiring;

	// Token: 0x04000A78 RID: 2680
	public float directionFiring;

	// Token: 0x04000A79 RID: 2681
	public float enemyTorpedoTimer;

	// Token: 0x04000A7A RID: 2682
	public float[] enemyTorpedoTubesReloadingTimers;

	// Token: 0x04000A7B RID: 2683
	public bool snapshot;

	// Token: 0x04000A7C RID: 2684
	public Transform snapshotTransform;

	// Token: 0x04000A7D RID: 2685
	public int snapshotNumberToFire;

	// Token: 0x04000A7E RID: 2686
	public float snapShotRefreshTime = 300f;

	// Token: 0x04000A7F RID: 2687
	public float snapshotTimer;
}
