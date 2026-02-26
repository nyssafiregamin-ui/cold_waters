using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class VesselAI : MonoBehaviour
{
	// Token: 0x06000B04 RID: 2820 RVA: 0x000996A0 File Offset: 0x000978A0
	private void Start()
	{
		this.icebergPositions = new List<Transform>();
		this.playerVessel = GameDataManager.playervesselsonlevel[0];
		if (this.parentVessel.isSubmarine)
		{
			this.parentVessel.acoustics.usingActiveSonar = false;
		}
		else if (this.sensordata != null)
		{
			this.parentVessel.acoustics.usingActiveSonar = true;
		}
		if (this.parentVessel.databaseshipdata.shipDesignation == "SSBN" || this.parentVessel.databaseshipdata.shipDesignation == "SSGN")
		{
			this.actDefensively = true;
		}
		this.sonarTimer = 20f;
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerStrength > 0f)
		{
			this.layerForSubmarineAI = UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerDepth;
		}
		else
		{
			this.layerForSubmarineAI = UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetlayerDepthInFeet();
			this.layerForSubmarineAI = 1000f - this.layerForSubmarineAI * GameDataManager.feetToUnits;
		}
		if (this.layerForSubmarineAI > 999.3f)
		{
			this.layerForSubmarineAI = 999.3f;
		}
		this.pullUpDepth = (1000f - this.parentVessel.databaseshipdata.actualTestDepth) * 0.6f;
		this.pullUpDepth = 1000f - this.pullUpDepth;
		if (this.parentVessel.databaseshipdata.shipType == "ESCORT")
		{
			this.sprintDriftAllowed = true;
		}
		else if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE" && !this.actDefensively)
		{
			this.sprintDriftAllowed = true;
		}
		if (this.sprintDriftAllowed && UnityEngine.Random.value < 0.1f)
		{
			this.SprintAndDrift();
		}
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00099898 File Offset: 0x00097A98
	public Vector3 PlaceNeutralInPosition()
	{
		this.parentVessel.vesselmovement.isCruising = false;
		base.transform.Translate(Vector3.right * (float)UnityEngine.Random.Range(90, 150));
		base.transform.Translate(Vector3.forward * (float)UnityEngine.Random.Range(90, 150));
		this.sensordata.unitWaypoints = new Vector2[1];
		this.GetNextNeutralWaypoint();
		return base.transform.position;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0009991C File Offset: 0x00097B1C
	public void SwitchToActiveSonar()
	{
		if (this.parentVessel.databaseshipdata.activeSonarID == -1)
		{
			return;
		}
		if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE" && !this.subCanUseActiveSonar)
		{
			return;
		}
		this.parentVessel.acoustics.usingActiveSonar = true;
		this.sensordata.usingPassiveSonar = false;
		this.sensordata.usingActiveSonar = true;
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00099994 File Offset: 0x00097B94
	public void SwitchToPassiveSonar()
	{
		this.parentVessel.acoustics.usingActiveSonar = false;
		this.sensordata.usingPassiveSonar = true;
		this.sensordata.usingActiveSonar = false;
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x000999C0 File Offset: 0x00097BC0
	private void SprintAndDrift()
	{
		if (this.sprintDriftAllowed)
		{
			float num = Vector3.Distance(base.transform.position, UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[this.parentVessel.vesselListIndex].transform.position);
			if (num < 15f && this.parentVessel.vesselmovement.isCruising && UnityEngine.Random.value < 0.03f)
			{
				this.actionTimeToFinish = UnityEngine.Random.Range(60f, 120f);
				this.parentVessel.acoustics.sensorNavigator.transform.parent = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[this.parentVessel.vesselListIndex].transform;
				if (this.parentVessel.vesselai.formationWaypointModifier.z < 0f)
				{
					this.parentVessel.acoustics.sensorNavigator.transform.localPosition = new Vector3(this.parentVessel.vesselai.formationWaypointModifier.x * 2f, 0f, this.parentVessel.vesselai.formationWaypointModifier.z * -2f);
					this.listenPosition = this.parentVessel.acoustics.sensorNavigator.transform.position;
					this.parentVessel.acoustics.sensorNavigator.transform.localPosition = Vector3.zero;
				}
				else
				{
					this.parentVessel.acoustics.sensorNavigator.transform.localPosition = new Vector3(0f, 0f, 90f);
					this.listenPosition = this.parentVessel.acoustics.sensorNavigator.transform.position;
					this.parentVessel.acoustics.sensorNavigator.transform.localPosition = Vector3.zero;
					this.actionTimeToFinish *= 2f;
				}
				this.parentVessel.acoustics.sensorNavigator.transform.parent = this.parentVessel.transform;
				this.takingAction = 5;
				this.SwitchToActiveSonar();
				UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, UnityEngine.Random.Range(5, 7));
				this.parentVessel.vesselmovement.isCruising = false;
			}
		}
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x00099C34 File Offset: 0x00097E34
	private void ActionCheck()
	{
		if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			this.SubmarineAI();
		}
		if (!this.parentVessel.acoustics.usingActiveSonar)
		{
			if (this.sensordata.decibelsLastDetected <= 0f && this.parentVessel.vesselmovement.shipSpeed.z > UnityEngine.Random.Range(1f, 1.5f))
			{
				this.SwitchToActiveSonar();
			}
		}
		else if (this.sensordata.decibelsLastDetected <= 0f && this.parentVessel.vesselmovement.shipSpeed.z < UnityEngine.Random.Range(1f, 1.5f))
		{
			this.SwitchToPassiveSonar();
		}
		if (!UIFunctions.globaluifunctions.combatai.PlayerIsCombatWorthy())
		{
			this.parentVessel.vesselai.takingAction = 0;
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, UnityEngine.Random.Range(3, 7));
			if (!this.parentVessel.vesselmovement.atAnchor || this.isNeutral)
			{
				this.parentVessel.vesselmovement.isCruising = true;
			}
		}
		else if (this.sensordata.playerDetected)
		{
			this.parentVessel.uifunctions.combatai.CheckToAttackPlayer(this.parentVessel);
		}
		else
		{
			this.SprintAndDrift();
		}
		if (this.hasNavalGuns)
		{
			if (this.enemynavalguns.enabled)
			{
				if (this.sensordata.rangeYardsLastDetected < UIFunctions.globaluifunctions.database.databasedepthchargedata[this.parentVessel.databaseshipdata.navalGunTypes[0]].weaponRange.y * 0.5f)
				{
					if (this.takingAction != 9 && this.takingAction != 10)
					{
						this.takingAction = 1;
						this.parentVessel.vesselmovement.isCruising = false;
						UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 6);
						this.actionTimeToFinish = UnityEngine.Random.Range(8f, 30f);
					}
				}
				else if (this.sensordata.rangeYardsLastDetected < UIFunctions.globaluifunctions.database.databasedepthchargedata[this.parentVessel.databaseshipdata.navalGunTypes[0]].weaponRange.y * 0.2f && this.takingAction != 9 && this.takingAction != 10)
				{
					this.takingAction = 2;
					this.actionPosition = GameDataManager.playervesselsonlevel[0].transform.position;
					this.parentVessel.vesselmovement.isCruising = false;
					UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 6);
					this.actionTimeToFinish = UnityEngine.Random.Range(8f, 30f);
				}
				if (this.sensordata.rangeYardsLastDetected > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.parentVessel.databaseshipdata.navalGunTypes[0]].weaponRange.y)
				{
					for (int i = 0; i < this.enemynavalguns.firingPhase.Length; i++)
					{
						this.enemynavalguns.firingPhase[i] = 4;
						this.enemynavalguns.firingAtPlayerTimer[i] = 0f;
					}
				}
			}
			if (GameDataManager.playervesselsonlevel[0].transform.position.y > GameDataManager.playervesselsonlevel[0].databaseshipdata.submergedat && this.sensordata.rangeYardsLastDetected < UIFunctions.globaluifunctions.database.databasedepthchargedata[this.parentVessel.databaseshipdata.navalGunTypes[0]].weaponRange.y && this.sensordata.rangeYardsLastDetected > UIFunctions.globaluifunctions.database.databasedepthchargedata[this.parentVessel.databaseshipdata.navalGunTypes[0]].weaponRange.x)
			{
				if (!this.enemynavalguns.enabled)
				{
					this.enemynavalguns.enabled = true;
				}
				this.transformToAttack = GameDataManager.playervesselsonlevel[0].transform;
				this.enemynavalguns.targetPosition = GameDataManager.playervesselsonlevel[0].transform.position;
				this.enemynavalguns.fireAtPosition = true;
				for (int j = 0; j < this.enemynavalguns.turrets.Length; j++)
				{
					if (this.enemynavalguns.firingPhase[j] == 0)
					{
						this.enemynavalguns.firingPhase[j] = 1;
						this.enemynavalguns.firingAtPlayerTimer[j] = UnityEngine.Random.Range(-4f, 0f);
					}
				}
			}
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0009A0FC File Offset: 0x000982FC
	private void SubmarineAI()
	{
		if (!this.parentVessel.vesselai.blowBallast && !this.parentVessel.vesselai.attemptToSurface)
		{
			if (this.parentVessel.vesselai.torpedoThreat == null && (this.parentVessel.vesselmovement.isCavitating || this.parentVessel.databaseshipdata.towedSonarID != -1) && UnityEngine.Random.value < 0.5f)
			{
				this.parentVessel.vesselmovement.telegraphValue = 2;
			}
			if (this.parentVessel.vesselmovement.percentageSurfaced > 0.5f)
			{
				this.surfacedTime += Time.deltaTime;
				if (this.surfacedTime > 60f)
				{
					this.parentVessel.vesselmovement.diveAngle.x = 0.5f;
					this.surfacedTime = 0f;
				}
			}
		}
		if (this.blowBallast)
		{
			if (this.parentVessel.vesselmovement.percentageSurfaced > 0.8f)
			{
				this.parentVessel.vesselmovement.diveAngle.x = 0f;
			}
			else
			{
				this.parentVessel.vesselmovement.diveAngle.x = -3f;
			}
			return;
		}
		if (this.parentVessel.damagesystem.currentFlooding > 0f)
		{
			if (!this.attemptToSurface)
			{
				UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 6);
				this.parentVessel.vesselmovement.diveAngle.x = -3f;
				this.attemptToSurface = true;
			}
			if (!this.blowBallast && this.parentVessel.damagesystem.currentFlooding > 0.3f)
			{
				if (!this.parentVessel.vesselmovement.blowBallast)
				{
					this.parentVessel.vesselmovement.blowBallast = true;
				}
				this.blowBallast = true;
				if (this.parentVessel.damagesystem.emergencyBlow != null)
				{
					this.parentVessel.damagesystem.emergencyBlow.Play();
					this.parentVessel.damagesystem.emergencyBlow.GetComponent<AudioSource>().Play();
				}
				return;
			}
		}
		this.parentVessel.vesselmovement.ballastAngle.x = 0f;
		float num = 100f;
		if (this.parentVessel.transform.eulerAngles.x > 0f && this.parentVessel.transform.eulerAngles.x < 180f)
		{
			num += (this.parentVessel.transform.eulerAngles.x + this.parentVessel.vesselmovement.shipSpeed.z * 10f) * 10f;
		}
		if (base.transform.position.y > this.tooShallowThreshold && !this.blowBallast && !this.periscopeDepth)
		{
			if (this.parentVessel.vesselmovement.diveAngle.x < 0f)
			{
				this.parentVessel.vesselmovement.diveAngle.x = 0.5f;
			}
			if (this.neverRunDeep)
			{
				this.tooShallowThreshold = 999.8f;
			}
			this.shallowWarning = true;
			return;
		}
		if (base.transform.position.y < this.pullUpDepth + this.parentVessel.vesselmovement.diveAngle.x / 3f)
		{
			if (this.parentVessel.vesselmovement.diveAngle.x == 3f)
			{
				this.parentVessel.vesselmovement.diveAngle.x = -3f;
			}
			else
			{
				this.parentVessel.vesselmovement.diveAngle.x = -0.5f;
			}
			return;
		}
		if (this.depthUnderkeel < num)
		{
			if (this.parentVessel.vesselmovement.diveAngle.x == 3f)
			{
				this.parentVessel.vesselmovement.diveAngle.x = -3f;
			}
			else
			{
				this.parentVessel.vesselmovement.diveAngle.x = -1.5f;
			}
			this.layerForSubmarineAI += 1f;
			if (this.layerForSubmarineAI > 999.3f)
			{
				this.layerForSubmarineAI = 999.3f;
				this.runDeep = false;
				this.neverRunDeep = true;
			}
			this.AvoidTerrain();
			this.terrainWarning = true;
			return;
		}
		if (this.shallowWarning || this.terrainWarning)
		{
			this.shallowWarning = false;
			this.terrainWarning = false;
			this.parentVessel.vesselmovement.diveAngle.x = 0f;
		}
		if (this.neverRunDeep)
		{
			return;
		}
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerStrength > 0f)
		{
			if (this.sensordata.playerDetected)
			{
				if (this.sensordata.decibelsLastDetected > 20f)
				{
					if (this.parentVessel.acoustics.currentlyAboveLayer)
					{
						this.runDeep = true;
					}
					else
					{
						this.runDeep = false;
					}
				}
			}
			else if (UnityEngine.Random.value < 0.1f)
			{
				this.runDeep = !this.runDeep;
			}
		}
		else if (UnityEngine.Random.value < 0.1f)
		{
			this.runDeep = !this.runDeep;
			if (UnityEngine.Random.value < 0.2f)
			{
				this.currentLayerDepth = 999.7f;
				this.periscopeDepth = true;
			}
			else
			{
				this.currentLayerDepth = this.layerForSubmarineAI;
				this.periscopeDepth = false;
			}
		}
		if (!this.runDeep)
		{
			if (base.transform.position.y < this.currentLayerDepth)
			{
				this.parentVessel.vesselmovement.diveAngle.x = -1.5f;
			}
			else
			{
				this.parentVessel.vesselmovement.diveAngle.x = 0f;
			}
		}
		else if (base.transform.position.y > this.currentLayerDepth)
		{
			this.parentVessel.vesselmovement.diveAngle.x = 1.5f;
		}
		else
		{
			this.parentVessel.vesselmovement.diveAngle.x = 0f;
		}
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0009A7B4 File Offset: 0x000989B4
	public void AttackPosition()
	{
		if (this.attackRole == "DEFENDER")
		{
			return;
		}
		if (!this.attackingPlayer)
		{
			if (this.sensordata.timeTrackingPlayer < UnityEngine.Random.Range(120f, 180f))
			{
				string attackType = this.GetAttackType();
				if (attackType == string.Empty)
				{
					this.ClearSensorData();
					return;
				}
				if (attackType == "TORPEDO")
				{
					if (this.enemytorpedo.enemyTorpedoTimer > UIFunctions.globaluifunctions.combatai.torpedoSalvoSize * 120f)
					{
						this.ClearSensorData();
						return;
					}
					foreach (Torpedo torpedo in UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects)
					{
						if (torpedo.vesselFiredFrom == this.parentVessel && torpedo.onWire)
						{
							this.ClearSensorData();
							return;
						}
					}
				}
				else if (attackType == "MISSILE" && this.enemymissile.enemyMissileTimer > 0f)
				{
					this.ClearSensorData();
					return;
				}
			}
			this.attackingPlayer = true;
			this.attackTimer = 0f;
		}
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0009A8F4 File Offset: 0x00098AF4
	private void SubmarineDepthKeeping()
	{
		this.parentVessel.vesselmovement.ballastAngle.x = 0f;
		float num = 100f;
		if (this.parentVessel.transform.eulerAngles.x > 0f && this.parentVessel.transform.eulerAngles.x < 180f)
		{
			num += (this.parentVessel.transform.eulerAngles.x + this.parentVessel.vesselmovement.shipSpeed.z * 10f) * 10f;
		}
		if (base.transform.position.y > this.tooShallowThreshold && !this.blowBallast && !this.periscopeDepth)
		{
			if (this.parentVessel.vesselmovement.diveAngle.x < 0f)
			{
				this.parentVessel.vesselmovement.diveAngle.x = 0.5f;
			}
			if (this.neverRunDeep)
			{
				this.tooShallowThreshold = 999.8f;
			}
			this.shallowWarning = true;
			return;
		}
		if (base.transform.position.y < this.pullUpDepth + this.parentVessel.vesselmovement.diveAngle.x / 3f)
		{
			if (this.parentVessel.vesselmovement.diveAngle.x == 3f)
			{
				this.parentVessel.vesselmovement.diveAngle.x = -3f;
			}
			else
			{
				this.parentVessel.vesselmovement.diveAngle.x = -0.5f;
			}
			return;
		}
		if (this.depthUnderkeel < num)
		{
			if (this.parentVessel.vesselmovement.diveAngle.x == 3f)
			{
				this.parentVessel.vesselmovement.diveAngle.x = -3f;
			}
			else
			{
				this.parentVessel.vesselmovement.diveAngle.x = -1.5f;
			}
			this.layerForSubmarineAI += 1f;
			if (this.layerForSubmarineAI > 999.3f)
			{
				this.layerForSubmarineAI = 999.3f;
				this.runDeep = false;
				this.neverRunDeep = true;
			}
			this.AvoidTerrain();
			this.terrainWarning = true;
			return;
		}
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0009AB70 File Offset: 0x00098D70
	private void ClearSensorData()
	{
		this.sensordata.decibelsTotalDetected = 0f;
		this.sensordata.decibelsLastDetected = 0f;
		this.sensordata.radarTotalDetected = 0f;
		this.sensordata.timeTrackingPlayer = 0f;
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x0009ABC0 File Offset: 0x00098DC0
	private string GetAttackType()
	{
		string text = string.Empty;
		this.transformToAttack = UIFunctions.globaluifunctions.combatai.GetActualPositionToAttack(this.parentVessel.transform);
		float num = Vector3.Distance(base.transform.position, this.transformToAttack.position) * GameDataManager.yardsScale;
		if (this.hasMissile && this.enemymissile.enemyMissileTimer <= 0f && num > UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.missileType].missileFiringRange.x && num < UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.missileType].missileFiringRange.y)
		{
			int num2 = 0;
			for (int i = 0; i < this.enemymissile.numberOfMissiles.Length; i++)
			{
				num2 += this.enemymissile.numberOfMissiles[i];
			}
			if (num2 > 0)
			{
				if (this.parentVessel.databaseshipdata.shipType != "SUBMARINE")
				{
					text = "MISSILE";
				}
				else if (!UIFunctions.globaluifunctions.levelloadmanager.packIcePresent)
				{
					text = "MISSILE";
				}
			}
		}
		if (text == string.Empty && this.CanFireRBU() > -1)
		{
			text = "RBU";
		}
		if (text == string.Empty && this.hasTorpedo && num < UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.torpedoIDs[0]].rangeInYards * 0.8f && this.enemytorpedo.torpedoMounts[0].transform.position.y < 999.9f && this.enemytorpedo.numberOfTorpedoes[0] > 0)
		{
			text = "TORPEDO";
		}
		return text;
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0009ADC4 File Offset: 0x00098FC4
	private void CommenceAttack()
	{
		if (!UIFunctions.globaluifunctions.combatai.playerWasFiredUpon)
		{
			UIFunctions.globaluifunctions.combatai.playerWasFiredUpon = true;
		}
		this.transformToAttack = UIFunctions.globaluifunctions.combatai.GetActualPositionToAttack(this.parentVessel.transform);
		this.ClearSensorData();
		float num = Vector3.Distance(base.transform.position, this.transformToAttack.position) * GameDataManager.yardsScale;
		if (this.hasMissile && num > UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.missileType].missileFiringRange.x && num < UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.missileType].missileFiringRange.y)
		{
			if (this.parentVessel.databaseshipdata.shipType != "SUBMARINE")
			{
				this.enemymissile.fireAtTarget = true;
			}
			else if (UIFunctions.globaluifunctions.combatai.CheckWaterLocationIsValid(base.transform.position))
			{
				this.enemymissile.fireAtTarget = true;
				return;
			}
		}
		if (this.CanFireRBU() > -1)
		{
			this.enemyrbu.targetPosition = this.transformToAttack.position;
			this.enemyrbu.fireAtPosition = true;
		}
		if (this.hasTorpedo && num < UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.torpedoIDs[0]].rangeInYards * 0.8f && this.enemytorpedo.enemyTorpedoTimer < UIFunctions.globaluifunctions.combatai.torpedoSalvoSize * 120f)
		{
			int num2 = 0;
			foreach (Torpedo torpedo in UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects)
			{
				if (torpedo.vesselFiredFrom == this.parentVessel)
				{
					num2++;
				}
			}
			if ((float)num2 * 0.3f < UnityEngine.Random.value && num2 < 3)
			{
				if (this.enemytorpedo.fixedTubes)
				{
					this.enemytorpedo.FireTorpedo(0);
				}
				else
				{
					this.enemytorpedo.fireAtTarget = true;
				}
			}
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0009B02C File Offset: 0x0009922C
	public int CanFireRBU()
	{
		if (!this.hasRBU)
		{
			return -1;
		}
		return this.enemyrbu.GetRBUToFire();
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0009B048 File Offset: 0x00099248
	private void CheckTorpedoes()
	{
		if (this.parentVessel.databaseshipdata.passiveSonarID == -1)
		{
			return;
		}
		if (this.takingAction == 10)
		{
			return;
		}
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
		{
			if (this.parentVessel.uifunctions.playerfunctions.sensormanager.torpedoObjects[i].whichNavy == 0 && UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckSingleSonar(this.parentVessel.gameObject, UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].gameObject, this.parentVessel.databaseshipdata.passiveSonarID, this.parentVessel.databaseshipdata.activeSonarID, 0, this.parentVessel.vesselai.sensordata, false, null))
			{
				if (!this.parentVessel.uifunctions.combatai.warnedFleet && !this.parentVessel.isSubmarine)
				{
					this.parentVessel.uifunctions.combatai.warnedFleet = true;
					this.parentVessel.uifunctions.combatai.positionWarnedFrom = this.parentVessel.uifunctions.playerfunctions.sensormanager.torpedoObjects[i].transform.position;
					this.parentVessel.uifunctions.combatai.warnTimer = UnityEngine.Random.Range(3f, 10f);
				}
				if (this.torpedoThreat == null)
				{
					if (this.parentVessel.uifunctions.playerfunctions.sensormanager.torpedoObjects[i].guidanceActive)
					{
						this.torpedoThreatIsActive = false;
						this.torpedoThreatIsCircular = false;
					}
					else
					{
						this.torpedoThreatIsActive = true;
						if (this.parentVessel.uifunctions.playerfunctions.sensormanager.torpedoObjects[i].snakeSearch)
						{
							this.torpedoThreatIsCircular = false;
						}
						else
						{
							this.torpedoThreatIsCircular = true;
						}
					}
					this.EvadeTorpedo(this.parentVessel.uifunctions.playerfunctions.sensormanager.torpedoObjects[i]);
					this.SwitchToActiveSonar();
				}
			}
		}
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0009B28C File Offset: 0x0009948C
	private void FixedUpdate()
	{
		if (this.parentVessel.vesselmovement.atAnchor)
		{
			return;
		}
		if (this.parentVessel.isSinking || this.parentVessel.databaseshipdata.shipType == "OILRIG")
		{
			base.enabled = false;
			return;
		}
		if (this.isNeutral && this.depthUnderkeel < 30f)
		{
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 1);
			base.enabled = false;
		}
		if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			this.depthKeepingTimer += Time.deltaTime;
			if (this.depthKeepingTimer > 5f)
			{
				this.SubmarineDepthKeeping();
				this.depthKeepingTimer = 0f;
			}
		}
		if (this.vesselIsDecoyed)
		{
			this.decoyTimer += Time.deltaTime;
			if (this.decoyTimer > this.timeDecoyedFor)
			{
				this.decoyTimer = 0f;
				this.vesselIsDecoyed = false;
			}
		}
		this.sonarTimer += Time.deltaTime;
		if (this.attackingPlayer)
		{
			this.attackTimer += Time.deltaTime;
			if (this.sonarTimer > 1f && !this.parentVessel.vesselmovement.pingSound.isPlaying && this.parentVessel.databaseshipdata.activeSonarID != -1)
			{
				UIFunctions.globaluifunctions.combatai.PlaySonarPing(this.parentVessel);
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.DrawPingLine(this.parentVessel.transform, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines[0]);
				this.sonarTimer -= 1f;
				if (Vector3.Distance(base.transform.position, GameDataManager.playervesselsonlevel[0].transform.position) < 15000f * GameDataManager.inverseYardsScale)
				{
					UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("range gating", true);
				}
			}
			if (this.attackTimer > 5f)
			{
				this.attackingPlayer = false;
				this.attackTimer = 0f;
				this.CommenceAttack();
				if (this.parentVessel.databaseshipdata.activeSonarID != -1 && Vector3.Distance(base.transform.position, GameDataManager.playervesselsonlevel[0].transform.position) < 15000f * GameDataManager.inverseYardsScale)
				{
					UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("range gating", false);
				}
			}
		}
		else if (this.sensordata.usingActiveSonar && this.sonarTimer > this.sonarRate && !this.parentVessel.vesselmovement.pingSound.isPlaying)
		{
			UIFunctions.globaluifunctions.combatai.PlaySonarPing(this.parentVessel);
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.DrawPingLine(this.parentVessel.transform, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines[0]);
			this.sonarTimer -= this.sonarRate;
		}
		if (this.icebergPositions.Count != 0)
		{
			Vector3 vector = Vector3.zero;
			int num = 0;
			foreach (Transform transform in this.icebergPositions)
			{
				vector += transform.position;
				num++;
			}
			vector /= (float)num;
			this.parentVessel.acoustics.sensorNavigator.transform.LookAt(vector);
			float num2 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
			float num3 = 120f;
			if (this.icebergPositions.Count > 1)
			{
				num3 = 180f;
			}
			else if (num2 < 180f)
			{
				num3 *= -1f;
			}
			num2 += num3;
			if (num2 > 360f)
			{
				num2 -= 360f;
			}
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			if (num2 > 30f)
			{
				num2 = 30f;
			}
			else if (num2 < -30f)
			{
				num2 = -30f;
			}
			this.parentVessel.vesselmovement.rudderAngle.x = num2 / 10f;
			for (int i = 0; i < this.icebergPositions.Count; i++)
			{
				if (Vector3.Distance(base.transform.position, this.icebergPositions[i].position) > this.distanceToAvoidIceBy)
				{
					this.icebergPositions.Remove(this.icebergPositions[i]);
				}
			}
			if (this.icebergPositions.Count == 0)
			{
				this.distanceToAvoidIceBy = 0f;
				this.avoidingIceberg = false;
			}
			return;
		}
		if (this.avoidingTerrain)
		{
			this.takingAction = 15;
		}
		else if (this.avoidingIceberg)
		{
			this.takingAction = 9;
		}
		if (this.takingAction == 15)
		{
			this.parentVessel.acoustics.sensorNavigator.transform.LookAt(new Vector3(this.deepestWater.x, 1000f, this.deepestWater.y));
			float num4 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
			if (num4 > 180f)
			{
				num4 -= 360f;
			}
			if (num4 > 30f)
			{
				num4 = 30f;
			}
			else if (num4 < -30f)
			{
				num4 = -30f;
			}
			this.parentVessel.vesselmovement.rudderAngle.x = num4 / 10f;
			Vector2 a = new Vector2(base.transform.position.x, base.transform.position.z);
			if (Vector2.Distance(a, this.deepestWater) < 10f)
			{
				this.CompleteAction();
			}
		}
		else
		{
			if (this.takingAction == 9)
			{
				bool flag = false;
				float num5 = Vector3.Distance(this.parentVessel.transform.position, this.hazardThreat.position);
				if (num5 > this.distanceToAvoidHazardBy)
				{
					if (!this.avoidingIceberg)
					{
						flag = true;
					}
					else
					{
						this.AvoidHazard(this.icebergTransform);
					}
				}
				this.parentVessel.acoustics.sensorNavigator.transform.LookAt(this.hazardThreat);
				float y = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
				if (!flag)
				{
					if (y > 180f)
					{
						this.parentVessel.vesselmovement.rudderAngle.x = 3f;
					}
					else
					{
						this.parentVessel.vesselmovement.rudderAngle.x = -3f;
					}
					if (this.avoidingIceberg)
					{
						float num6 = 10f;
						if (this.parentVessel.vesselmovement.shipSpeed.z > 2f)
						{
							num6 = 8f;
						}
						else if (this.parentVessel.vesselmovement.shipSpeed.z > 2.5f)
						{
							num6 = 6f;
						}
						VesselMovement vesselmovement = this.parentVessel.vesselmovement;
						vesselmovement.rudderAngle.x = vesselmovement.rudderAngle.x / num6;
						if (y > 100f && y < 260f)
						{
							flag = true;
							this.avoidingIceberg = false;
						}
					}
				}
				if (flag)
				{
					this.CompleteAction();
				}
				return;
			}
			if (this.takingAction == 10)
			{
				if (this.timeSinceLastNoisemaker > 0f)
				{
					this.timeSinceLastNoisemaker -= Time.deltaTime;
				}
				if (this.torpedoThreat != null)
				{
					bool flag2 = false;
					float num7 = Vector3.Distance(this.parentVessel.transform.position, this.torpedoThreat.position) * GameDataManager.yardsScale;
					if (num7 > this.lastTorpedoDistance)
					{
					}
					this.lastTorpedoDistance = num7;
					float num8 = 6500f;
					if (!this.torpedoThreatIsActive || !this.torpedoThreatIsCircular)
					{
						num8 = 10000f;
					}
					if (num7 > num8)
					{
						flag2 = true;
					}
					this.parentVessel.acoustics.sensorNavigator.transform.LookAt(this.torpedoThreat);
					float num9 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
					float num10 = Vector3.Angle(base.transform.forward, this.torpedoThreat.forward);
					bool flag3 = false;
					if (num9 > 135f && num9 < 225f)
					{
						flag3 = true;
					}
					if (this.timeSinceLastNoisemaker > 0f && !this.torpSearchDirectionDetermined && this.timeSinceLastNoisemaker < 20f)
					{
						this.torpSearchDirectionDetermined = true;
						if (num9 > 0f)
						{
							this.nextEvadeDirection = 1f;
						}
						else
						{
							this.nextEvadeDirection = -1f;
						}
					}
					if (this.timeSinceLastNoisemaker > 0f)
					{
						if (num9 > 180f)
						{
							this.parentVessel.vesselmovement.rudderAngle.x = -3f * this.evadeDirection;
						}
						else
						{
							this.parentVessel.vesselmovement.rudderAngle.x = 3f * this.evadeDirection;
						}
					}
					else if (flag3)
					{
						this.parentVessel.vesselmovement.rudderAngle.x = 0f;
					}
					else
					{
						this.parentVessel.acoustics.sensorNavigator.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, this.parentVessel.acoustics.sensorNavigator.transform.localEulerAngles.y + 180f, 0f), 1f);
						num9 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
						if (num9 > 180f)
						{
							num9 -= 360f;
						}
						if (num9 > 30f)
						{
							num9 = 30f;
						}
						else if (num9 < -30f)
						{
							num9 = -30f;
						}
						this.parentVessel.vesselmovement.rudderAngle.x = num9 / 10f;
					}
					if (this.timeSinceLastNoisemaker <= 0f && this.parentVessel.vesselai.hasNoiseMaker && flag3 && num10 < 90f && num7 < UnityEngine.Random.Range(2500f, 3500f))
					{
						if (this.nextEvadeDirection != 0f)
						{
							this.evadeDirection = this.nextEvadeDirection;
						}
						if (this.parentVessel.vesselmovement.shipSpeed.z > 2.4f)
						{
							Noisemaker component = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVessel.uifunctions.playerfunctions.sensormanager.knuckleID].countermeasureObject, this.parentVessel.vesselmovement.rudder[0].position, Quaternion.identity).GetComponent<Noisemaker>();
							component.gameObject.SetActive(true);
							component.tacMapNoisemakerIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.navyColors[1];
							component.databasecountermeasuredata = UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVessel.uifunctions.playerfunctions.sensormanager.knuckleID];
							component.playerGenerated = false;
							this.parentVessel.uifunctions.playerfunctions.sensormanager.AddNoiseMakerToArray(component);
							this.enemynoisemaker.DropNoisemaker();
						}
						if (this.parentVessel.databaseshipdata.hasnoisemaker)
						{
							this.enemynoisemaker.DropNoisemaker();
						}
						this.timeSinceLastNoisemaker = 30f;
					}
					if (flag2)
					{
						this.CompleteAction();
					}
				}
				else
				{
					this.CompleteAction();
				}
				return;
			}
			if (this.takingAction == 0)
			{
				Vector3 position = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[this.parentVessel.vesselListIndex].transform.position;
				if (this.isNeutral && UIFunctions.globaluifunctions.combatai.usingFormationAI)
				{
					position = new Vector3(this.sensordata.unitWaypoints[0].x, 1000f, this.sensordata.unitWaypoints[0].y);
				}
				float num11 = Vector3.Distance(base.transform.position, position);
				if ((!UIFunctions.globaluifunctions.combatai.usingFormationAI || this.isNeutral) && num11 < 8f)
				{
					this.GetNextWaypoint();
					return;
				}
				this.parentVessel.acoustics.sensorNavigator.transform.LookAt(position);
				float num12 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
				if (num12 > 180f)
				{
					num12 -= 360f;
				}
				if (this.formationWaypointModifier == Vector3.zero)
				{
					this.parentVessel.uifunctions.combatai.formationTurningDirection = num12;
				}
				float num13 = 25f;
				if (num12 > 45f || num12 < -45f)
				{
					if (num11 < 3f && this.parentVessel.uifunctions.combatai.formationTurning)
					{
						UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationGrid.transform.Translate(Vector3.forward);
						num13 = 50f;
					}
					this.parentVessel.vesselmovement.cruiseSpeed = this.parentVessel.uifunctions.combatai.formationCruiseSpeeds[0];
				}
				else if (num11 > 1f)
				{
					this.parentVessel.vesselmovement.cruiseSpeed = this.parentVessel.uifunctions.combatai.formationCruiseSpeeds[2];
					if (num11 > 5f)
					{
						num11 = 5f;
						this.parentVessel.vesselmovement.cruiseSpeed += num11 / 10f;
					}
				}
				else
				{
					this.parentVessel.vesselmovement.cruiseSpeed = this.parentVessel.uifunctions.combatai.formationCruiseSpeeds[1];
				}
				if (this.parentVessel.uifunctions.combatai.formationTurning)
				{
					if (this.parentVessel.uifunctions.combatai.formationTurningDirection < 0f)
					{
						this.parentVessel.vesselmovement.cruiseSpeed += this.formationWaypointModifier.x / 50f;
					}
					else
					{
						this.parentVessel.vesselmovement.cruiseSpeed -= this.formationWaypointModifier.x / 50f;
					}
					if (this.formationWaypointModifier.z > 0f)
					{
						this.parentVessel.vesselmovement.cruiseSpeed += this.formationWaypointModifier.z / 100f;
					}
				}
				if (this.parentVessel.isSubmarine && this.parentVessel.vesselmovement.cruiseSpeed > this.parentVessel.uifunctions.combatai.formationCruiseSpeeds[1])
				{
					this.parentVessel.vesselmovement.cruiseSpeed = this.parentVessel.uifunctions.combatai.formationCruiseSpeeds[1];
				}
				if (num12 > 30f)
				{
					num12 = 30f;
				}
				else if (num12 < -30f)
				{
					num12 = -30f;
				}
				this.parentVessel.vesselmovement.rudderAngle.x = num12 / num13;
			}
			else if (this.takingAction == 1)
			{
				this.parentVessel.acoustics.sensorNavigator.transform.LookAt(this.sensordata.lastKnownTargetPosition);
				float num14 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
				if (num14 > 180f)
				{
					num14 -= 360f;
				}
				if (num14 > 30f)
				{
					num14 = 30f;
				}
				else if (num14 < -30f)
				{
					num14 = -30f;
				}
				this.parentVessel.vesselmovement.rudderAngle.x = num14 / 10f;
				if (UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, this.sensordata.lastKnownTargetPosition) < 5f)
				{
					if (this.sensordata.decibelsLastDetected > 0f)
					{
						this.sensordata.lastKnownTargetPosition = GameDataManager.playervesselsonlevel[0].transform.position;
					}
					else
					{
						this.takingAction = 5;
						this.CompleteAction();
					}
				}
			}
			else if (this.takingAction == 2)
			{
				this.parentVessel.acoustics.sensorNavigator.transform.LookAt(this.actionPosition);
				float num15 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y + 180f;
				if (num15 > 360f)
				{
					num15 -= 360f;
				}
				if (num15 > 180f)
				{
					num15 -= 360f;
				}
				if (num15 > 30f)
				{
					num15 = 30f;
				}
				else if (num15 < -30f)
				{
					num15 = -30f;
				}
				this.parentVessel.vesselmovement.rudderAngle.x = num15 / 10f;
			}
			else if (this.takingAction == 4)
			{
				if (this.parentVessel.vesselmovement.shipSpeed.z < 0.4f)
				{
					UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 2);
				}
			}
			else if (this.takingAction == 5)
			{
				this.parentVessel.acoustics.sensorNavigator.transform.LookAt(this.listenPosition);
				float num16 = this.parentVessel.acoustics.sensorNavigator.localEulerAngles.y;
				if (num16 > 180f)
				{
					num16 -= 360f;
				}
				if (num16 > 30f)
				{
					num16 = 30f;
				}
				else if (num16 < -30f)
				{
					num16 = -30f;
				}
				this.parentVessel.vesselmovement.rudderAngle.x = num16 / 10f;
				float num17 = Vector3.Distance(base.transform.position, this.listenPosition);
				if (num17 < 5f)
				{
					this.CompleteAction();
				}
			}
		}
		this.actionTimer += Time.deltaTime;
		this.torpedoCheckTimer += Time.deltaTime;
		if (this.actionTimeToFinish > 0f)
		{
			this.actionTimeToFinish -= Time.deltaTime;
			if (this.actionTimeToFinish <= 0f)
			{
				this.CompleteAction();
			}
		}
		if (this.torpedoCheckTimer > 10f)
		{
			this.CheckTorpedoes();
			this.torpedoCheckTimer -= UnityEngine.Random.Range(8f, 12f);
		}
		if (this.sensordata != null)
		{
			if (this.actionTimer > 20f)
			{
				this.ActionCheck();
				this.actionTimer = (float)UnityEngine.Random.Range(0, 11);
			}
			if (this.sensordata.playerDetected)
			{
				this.sensordata.timeTrackingPlayer += Time.deltaTime;
			}
			else if (this.sensordata.decibelsTotalDetected > 0f)
			{
				this.sensordata.decibelsTotalDetected -= 0.5f * Time.deltaTime;
			}
		}
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0009C7C0 File Offset: 0x0009A9C0
	public void EvadeTorpedo(Torpedo incomingTorpedo)
	{
		if (this.takingAction != 9)
		{
			if (this.takingAction != 10)
			{
				this.previousAction = this.takingAction;
				this.previousTelegraph = this.parentVessel.vesselmovement.telegraphValue;
			}
			this.takingAction = 10;
			this.parentVessel.vesselmovement.isCruising = false;
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 6);
			this.torpedoThreat = incomingTorpedo.transform;
			this.torpSearchDirectionDetermined = false;
			this.nextEvadeDirection = 0f;
			this.evadeDirection = 1f;
			if (UnityEngine.Random.value < 0.5f)
			{
				this.evadeDirection = -1f;
			}
		}
		if (!incomingTorpedo.missileSnapShotAt && incomingTorpedo.whichNavy != this.parentVessel.whichNavy && this.parentVessel.databaseshipdata.passiveSonarID != -1)
		{
			float num = Vector3.Distance(base.transform.position, incomingTorpedo.transform.position) * GameDataManager.yardsScale;
			if (this.hasMissile && this.enemymissile.snapshotTimer <= 0f && num > UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.missileType].missileFiringRange.x && num < UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.missileType].missileFiringRange.y && this.parentVessel.databaseshipdata.shipType != "SUBMARINE")
			{
				this.enemymissile.snapshot = true;
				this.transformToAttack = incomingTorpedo.transform;
				this.enemymissile.fireAtTarget = true;
				incomingTorpedo.missileSnapShotAt = true;
				this.enemymissile.snapshotTimer = this.enemymissile.snapShotRefreshTime;
				this.enemymissile.snapshotNumberToFire = 2;
			}
		}
		if (!incomingTorpedo.torpedoSnapShotAt && incomingTorpedo.whichNavy != this.parentVessel.whichNavy)
		{
			float num2 = Vector3.Distance(base.transform.position, incomingTorpedo.transform.position) * GameDataManager.yardsScale;
			if (this.hasTorpedo && this.enemytorpedo.snapshotTimer <= 0f && num2 < UIFunctions.globaluifunctions.database.databaseweapondata[this.parentVessel.databaseshipdata.torpedoIDs[0]].rangeInYards * 0.9f)
			{
				this.enemytorpedo.snapshot = true;
				this.enemytorpedo.snapshotTransform = incomingTorpedo.transform;
				if (this.enemytorpedo.fixedTubes)
				{
					incomingTorpedo.torpedoSnapShotAt = true;
					this.enemytorpedo.snapshotTimer = this.enemytorpedo.snapShotRefreshTime;
					int num3 = UnityEngine.Random.Range(1, 7);
					if (num3 < 4)
					{
						this.enemytorpedo.snapshotNumberToFire = 1;
					}
					else if (num3 < 6)
					{
						this.enemytorpedo.snapshotNumberToFire = 2;
					}
					else
					{
						this.enemytorpedo.snapshotNumberToFire = 3;
					}
					this.enemytorpedo.FireTorpedo(0);
				}
				else
				{
					incomingTorpedo.torpedoSnapShotAt = true;
					this.enemytorpedo.snapshotTimer = this.enemytorpedo.snapShotRefreshTime;
					if (!this.sensordata.playerDetected)
					{
						this.enemytorpedo.snapshotNumberToFire = 3;
					}
					else
					{
						this.enemytorpedo.snapshotNumberToFire = UnityEngine.Random.Range(1, 4);
					}
					this.enemytorpedo.fireAtTarget = true;
				}
			}
		}
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0009CB54 File Offset: 0x0009AD54
	public void AvoidHazard(Transform otherObjectTransform)
	{
		if (this.takingAction != 9)
		{
			this.previousAction = this.takingAction;
			this.previousTelegraph = this.parentVessel.vesselmovement.telegraphValue;
		}
		this.takingAction = 9;
		this.parentVessel.vesselmovement.isCruising = false;
		UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 5);
		this.hazardThreat = otherObjectTransform;
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0009CBC8 File Offset: 0x0009ADC8
	public void AvoidTerrain()
	{
		this.avoidingTerrain = true;
		float num = 0f;
		float[] array = new float[2];
		LayerMask layerMask = 1073741824;
		float num2 = 100f;
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				Vector3 origin = new Vector3(this.parentVessel.transform.position.x + num2 * (float)i, 1000f, this.parentVessel.transform.position.z + num2 * (float)j);
				RaycastHit raycastHit;
				if (Physics.Raycast(origin, -Vector3.up, out raycastHit, 2.66f, UIFunctions.globaluifunctions.combatai.terrainMinesMask) && raycastHit.distance > num)
				{
					num = raycastHit.distance;
					array[0] = origin.x;
					array[1] = origin.z;
				}
			}
		}
		this.deepestWater = new Vector2(array[0], array[1]);
		this.previousAction = this.takingAction;
		this.takingAction = 15;
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0009CCF4 File Offset: 0x0009AEF4
	public void GetNextWaypoint()
	{
		if (this.parentVessel.databaseshipdata.shipType == "OILRIG")
		{
			return;
		}
		if (this.isNeutral)
		{
			this.GetNextNeutralWaypoint();
			return;
		}
		if (!this.reverseWaypoints)
		{
			this.waypointNumber++;
			if (this.waypointNumber == this.sensordata.unitWaypoints.Length)
			{
				this.waypointNumber = 0;
			}
		}
		else
		{
			this.waypointNumber--;
			if (this.waypointNumber == -1)
			{
				this.waypointNumber = this.sensordata.unitWaypoints.Length - 1;
			}
		}
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[this.parentVessel.vesselListIndex].transform.position = new Vector3(this.sensordata.unitWaypoints[this.waypointNumber].x, 1000f, this.sensordata.unitWaypoints[this.waypointNumber].y);
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0009CE0C File Offset: 0x0009B00C
	private void GetNextNeutralWaypoint()
	{
		this.sensordata.unitWaypoints = new Vector2[1];
		Vector3 vector = base.transform.position + base.transform.forward * UnityEngine.Random.Range(100f, 200f);
		vector.x += UnityEngine.Random.Range(-2f, 2f);
		vector.z += UnityEngine.Random.Range(-2f, 2f);
		this.sensordata.unitWaypoints[0] = new Vector2(vector.x, vector.z);
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0009CEC0 File Offset: 0x0009B0C0
	private void CompleteAction()
	{
		if (this.parentVessel.vesselmovement.atAnchor)
		{
			return;
		}
		if (this.takingAction == 4)
		{
			this.takingAction = 0;
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 5);
			this.parentVessel.vesselmovement.isCruising = true;
		}
		else if (this.takingAction == 5)
		{
			this.takingAction = 4;
			this.parentVessel.vesselmovement.isCruising = false;
			this.SwitchToPassiveSonar();
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 1);
			this.actionTimeToFinish = UnityEngine.Random.Range(90f, 120f);
			float num = 0.5f;
			if (this.parentVessel.vesselai.formationWaypointModifier.z < -50f)
			{
				if (this.parentVessel.vesselai.formationWaypointModifier.x < 0f)
				{
					this.parentVessel.vesselmovement.rudderAngle.x = -num;
				}
				else
				{
					this.parentVessel.vesselmovement.rudderAngle.x = num;
				}
			}
			else
			{
				this.parentVessel.vesselmovement.rudderAngle.x = num;
				if (UnityEngine.Random.value < 0.5f)
				{
					this.parentVessel.vesselmovement.rudderAngle.x = -num;
				}
			}
		}
		else if (this.takingAction == 10 || this.takingAction == 9)
		{
			this.takingAction = this.previousAction;
			if (this.takingAction == 0 && !this.isNeutral)
			{
				this.parentVessel.vesselmovement.isCruising = true;
			}
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, this.previousTelegraph);
			this.torpedoThreat = null;
		}
		else if (this.takingAction == 15)
		{
			this.avoidingTerrain = false;
			this.takingAction = 0;
		}
		if (this.takingAction == 0)
		{
			if (this.attackRole == "HUNTER" || this.attackRole == "ATTACKER")
			{
				this.takingAction = 1;
				if (!this.parentVessel.isSubmarine)
				{
					if (this.sensordata.rangeYardsLastDetected > 10000f)
					{
						UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 6);
					}
					else if (this.sensordata.rangeYardsLastDetected < 6000f)
					{
						UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, 2);
					}
					else
					{
						UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, UnityEngine.Random.Range(3, 5));
					}
				}
			}
			this.actionTimeToFinish = UnityEngine.Random.Range(8f, 30f);
		}
	}

	// Token: 0x0400117B RID: 4475
	public Vessel parentVessel;

	// Token: 0x0400117C RID: 4476
	private Vessel playerVessel;

	// Token: 0x0400117D RID: 4477
	public SensorData sensordata;

	// Token: 0x0400117E RID: 4478
	public bool actDefensively;

	// Token: 0x0400117F RID: 4479
	public bool hasTorpedo;

	// Token: 0x04001180 RID: 4480
	public Enemy_Torpedo enemytorpedo;

	// Token: 0x04001181 RID: 4481
	public bool hasMissile;

	// Token: 0x04001182 RID: 4482
	public Enemy_Missile enemymissile;

	// Token: 0x04001183 RID: 4483
	public bool hasNavalGuns;

	// Token: 0x04001184 RID: 4484
	public Enemy_Guns enemynavalguns;

	// Token: 0x04001185 RID: 4485
	public bool hasRBU;

	// Token: 0x04001186 RID: 4486
	public Enemy_RBU enemyrbu;

	// Token: 0x04001187 RID: 4487
	public bool hasNoiseMaker;

	// Token: 0x04001188 RID: 4488
	public Enemy_Noisemaker enemynoisemaker;

	// Token: 0x04001189 RID: 4489
	public bool hasMissileDefense;

	// Token: 0x0400118A RID: 4490
	public Enemy_AntiMissileGuns enemymissiledefense;

	// Token: 0x0400118B RID: 4491
	public bool attackingPlayer;

	// Token: 0x0400118C RID: 4492
	public float attackTimer;

	// Token: 0x0400118D RID: 4493
	public float sonarTimer;

	// Token: 0x0400118E RID: 4494
	public float sonarRate = 20f;

	// Token: 0x0400118F RID: 4495
	public Vector3 listenPosition;

	// Token: 0x04001190 RID: 4496
	public int takingAction;

	// Token: 0x04001191 RID: 4497
	public int previousAction;

	// Token: 0x04001192 RID: 4498
	public int previousTelegraph;

	// Token: 0x04001193 RID: 4499
	public float actionTimer;

	// Token: 0x04001194 RID: 4500
	public float actionTimeToFinish;

	// Token: 0x04001195 RID: 4501
	public string attackRole;

	// Token: 0x04001196 RID: 4502
	public Vector3 actionPosition;

	// Token: 0x04001197 RID: 4503
	public float depthKeepingTimer;

	// Token: 0x04001198 RID: 4504
	public Transform transformToAttack;

	// Token: 0x04001199 RID: 4505
	public bool sprintDriftAllowed;

	// Token: 0x0400119A RID: 4506
	public Vector3 formationWaypointModifier;

	// Token: 0x0400119B RID: 4507
	public Transform torpedoThreat;

	// Token: 0x0400119C RID: 4508
	public Transform hazardThreat;

	// Token: 0x0400119D RID: 4509
	public bool torpedoThreatIsActive;

	// Token: 0x0400119E RID: 4510
	public bool torpedoThreatIsCircular;

	// Token: 0x0400119F RID: 4511
	public float torpedoEvadeTurnAmount;

	// Token: 0x040011A0 RID: 4512
	public float torpEvadeDirection;

	// Token: 0x040011A1 RID: 4513
	public float hazardEvadeDirection;

	// Token: 0x040011A2 RID: 4514
	public float torpedoCheckTimer;

	// Token: 0x040011A3 RID: 4515
	public float lastTorpedoDistance;

	// Token: 0x040011A4 RID: 4516
	public float timeSinceLastNoisemaker;

	// Token: 0x040011A5 RID: 4517
	public float evadeDirection;

	// Token: 0x040011A6 RID: 4518
	public float nextEvadeDirection;

	// Token: 0x040011A7 RID: 4519
	public bool torpSearchDirectionDetermined;

	// Token: 0x040011A8 RID: 4520
	public bool reverseWaypoints;

	// Token: 0x040011A9 RID: 4521
	public int waypointNumber;

	// Token: 0x040011AA RID: 4522
	public float depthUnderkeel;

	// Token: 0x040011AB RID: 4523
	public bool avoidingIceberg;

	// Token: 0x040011AC RID: 4524
	public float distanceToAvoidHazardBy;

	// Token: 0x040011AD RID: 4525
	public float distanceToAvoidIceBy;

	// Token: 0x040011AE RID: 4526
	public Transform icebergTransform;

	// Token: 0x040011AF RID: 4527
	public List<Transform> icebergPositions;

	// Token: 0x040011B0 RID: 4528
	public bool avoidingTerrain;

	// Token: 0x040011B1 RID: 4529
	public Vector2 deepestWater;

	// Token: 0x040011B2 RID: 4530
	public int speedAction;

	// Token: 0x040011B3 RID: 4531
	public bool attemptToSurface;

	// Token: 0x040011B4 RID: 4532
	public bool blowBallast;

	// Token: 0x040011B5 RID: 4533
	public bool runDeep;

	// Token: 0x040011B6 RID: 4534
	public bool neverRunDeep;

	// Token: 0x040011B7 RID: 4535
	public float layerForSubmarineAI;

	// Token: 0x040011B8 RID: 4536
	public float pullUpDepth;

	// Token: 0x040011B9 RID: 4537
	public bool terrainWarning;

	// Token: 0x040011BA RID: 4538
	public bool shallowWarning;

	// Token: 0x040011BB RID: 4539
	public float tooShallowThreshold = 999.5f;

	// Token: 0x040011BC RID: 4540
	public float currentLayerDepth;

	// Token: 0x040011BD RID: 4541
	public bool periscopeDepth;

	// Token: 0x040011BE RID: 4542
	public float surfacedTime;

	// Token: 0x040011BF RID: 4543
	public bool subCanUseActiveSonar;

	// Token: 0x040011C0 RID: 4544
	public bool vesselIsDecoyed;

	// Token: 0x040011C1 RID: 4545
	public float timeDecoyedFor;

	// Token: 0x040011C2 RID: 4546
	public float decoyTimer;

	// Token: 0x040011C3 RID: 4547
	public bool isNeutral;

	// Token: 0x040011C4 RID: 4548
	public bool isDocked;
}
